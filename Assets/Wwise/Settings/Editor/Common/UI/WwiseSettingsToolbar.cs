/*******************************************************************************
The content of this file includes portions of the proprietary AUDIOKINETIC Wwise
Technology released in source code form as part of the game integration package.
The content of this file may not be used without valid licenses to the
AUDIOKINETIC Wwise Technology.
Note that the use of the game engine is subject to the Unity(R) Terms of
Service at https://unity3d.com/legal/terms-of-service

License Usage

Licensees holding valid licenses to the AUDIOKINETIC Wwise Technology may use
this file in accordance with the end user license agreement provided with the
software or, alternatively, in accordance with the terms contained
in a written agreement between you and Audiokinetic Inc.
Copyright (c) 2026 Audiokinetic Inc.
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AK.Wwise.Unity.Settings.Editor.Common.UI
{
    // Persists the toolbar state when switching settings providers, separately for each page.
    internal class ToolbarState
    {
        private readonly string _scope;
        private readonly bool _defaultShowAdvanced;
        private readonly bool _defaultExpandAll;

        public ToolbarState(string scope, bool defaultShowAdvanced = false, bool defaultExpandAll = false)
        {
            _scope = scope;
            _defaultShowAdvanced = defaultShowAdvanced;
            _defaultExpandAll = defaultExpandAll;
        }

        public bool ShowAdvanced
        {
            get => SessionState.GetBool(Key("ShowAdvanced"), _defaultShowAdvanced);
            set => SessionState.SetBool(Key("ShowAdvanced"), value);
        }

        public bool ExpandAll
        {
            get => SessionState.GetBool(Key("ExpandAll"), _defaultExpandAll);
            set => SessionState.SetBool(Key("ExpandAll"), value);
        }

        // SessionState will only store a string, so store comma-separated strings.
        public HashSet<string> HiddenPlatformIds
        {
            get
            {
                var raw = SessionState.GetString(Key("HiddenPlatformIds"), string.Empty);
                return string.IsNullOrEmpty(raw) ? new HashSet<string>() : new HashSet<string>(raw.Split(','));
            }
            set => SessionState.SetString(Key("HiddenPlatformIds"), string.Join(",", value));
        }

        // Namespaces the user had open. Not captured while a search is active.
        public HashSet<string> ExpandedNamespaces
        {
            get
            {
                var raw = SessionState.GetString(Key("ExpandedNamespaces"), string.Empty);
                return string.IsNullOrEmpty(raw) ? new HashSet<string>() : new HashSet<string>(raw.Split(','));
            }
            set => SessionState.SetString(Key("ExpandedNamespaces"), string.Join(",", value));
        }

        private string Key(string suffix) => $"Wwise.Settings.{_scope}.{suffix}";
    }

    // Toolbar controls shared by the settings pages.
    internal static class WwiseSettingsToolbarButtons
    {
        private const long SearchDebounceMs = 150;
        private const float AdvancedToggleMinWidth = 105;

        // --unity-colors-button-background-pressed
        private static readonly Color ToggleOnColorDark = new Color32(0x46, 0x60, 0x7C, 0xFF);
        private static readonly Color ToggleOnColorLight = new Color32(0x96, 0xC3, 0xFB, 0xFF);
        private static Color ToggleOnColor => EditorGUIUtility.isProSkin ? ToggleOnColorDark : ToggleOnColorLight;

        // Same icons AkWwiseBrowser uses for its own expand-all/collapse-all toolbar buttons.
        private const string BrowserIconsPath = "Assets/Wwise/API/Editor/WwiseWindows/BrowserIcons/";
        private static Texture2D _expandAllIconCache;
        private static Texture2D _collapseAllIconCache;

        public static Texture2D ExpandAllIcon => LoadIcon(ref _expandAllIconCache, "GraphNode_ExpandAll.png");

        public static Texture2D CollapseAllIcon => LoadIcon(ref _collapseAllIconCache, "GraphNode_CollapseAll.png");

        private static Texture2D LoadIcon(ref Texture2D cache, string fileName)
        {
            if (cache == null)
            {
                cache = AssetDatabase.LoadAssetAtPath<Texture2D>(BrowserIconsPath + fileName);
            }
            return cache;
        }

        // Icon-only toolbar button, matching AkWwiseBrowser's expand/collapse buttons.
        public static Button MakeIconButton(Texture2D icon, string tooltip, Action onClick,
                                            float size = 20, float iconSize = 16)
        {
            var btn = new Button(onClick) { tooltip = tooltip };
            if (icon != null)
            {
                btn.style.backgroundImage = new StyleBackground(icon);
                btn.style.backgroundSize = new BackgroundSize(iconSize, iconSize);
                btn.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
                btn.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
                btn.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            }
            btn.style.width = size;
            btn.style.height = size;
            btn.style.marginTop = 4;
            btn.style.marginBottom = 4;
            btn.style.flexShrink = 0;
            return btn;
        }

        // "Show advanced" toggle.
        public static VisualElement MakeAdvancedToggle(WwiseSettingsViewModel viewModel)
        {
            var button = new Button { tooltip = "Toggle advanced options" };
            button.style.marginTop = 4;
            button.style.marginBottom = 4;
            button.style.flexShrink = 0;
            button.style.minWidth = AdvancedToggleMinWidth;

            void ApplyActiveStyle()
            {
                var on = viewModel.ShowAdvanced;
                button.text = on ? "Hide advanced" : "Show advanced";
                button.style.backgroundColor = on
                    ? new StyleColor(ToggleOnColor)
                    : new StyleColor(StyleKeyword.Null);
                button.style.unityFontStyleAndWeight = on ? FontStyle.Bold : FontStyle.Normal;
            }
            ApplyActiveStyle();

            button.clicked += () =>
            {
                viewModel.ShowAdvanced = !viewModel.ShowAdvanced;
                ApplyActiveStyle();
            };
            return button;
        }

        public static VisualElement MakeSearchField(WwiseSettingsViewModel viewModel, string placeholderText)
        {
            var search = new ToolbarSearchField();
            search.style.marginTop = 4;
            search.style.marginBottom = 4;
            search.style.marginLeft = 4;
            search.style.marginRight = 4;
#if UNITY_6000_0_OR_NEWER
            search.placeholderText = placeholderText;
#endif
            search.SetValueWithoutNotify(viewModel.SearchText);

            IVisualElementScheduledItem pending = null;
            search.RegisterValueChangedCallback(evt =>
            {
                // Debounce: only publish the query once typing pauses.
                var text = evt.newValue;
                pending?.Pause();
                pending = search.schedule
                    .Execute(() => viewModel.SearchText = text)
                    .StartingIn(SearchDebounceMs);
            });
            return search;
        }
    }

    // Dropdown summarizing platform column visibility; its popup stays open while toggling.
    // This uses IMGUI, we don't have the equivalent in UIElement
    internal class PlatformMaskField
    {
        private readonly List<(string Id, string Label)> _orderedPlatforms;
        private readonly Func<string, bool> _isVisible;
        private readonly Action<string> _toggle;
        private readonly Action _showAll;
        private readonly Action _hideAll;
        private IMGUIContainer _container;

        public PlatformMaskField(IEnumerable<(string Id, string Label)> orderedPlatforms, Func<string, bool> isVisible,
                                 Action<string> toggle, Action showAll, Action hideAll)
        {
            _orderedPlatforms = orderedPlatforms.ToList();
            _isVisible = isVisible;
            _toggle = toggle;
            _showAll = showAll;
            _hideAll = hideAll;
        }

        // Wiring for a frontend whose column visibility is the viewmodel's platform filter.
        public static PlatformMaskField ForViewModel(WwiseSettingsViewModel viewModel)
        {
            return new PlatformMaskField(viewModel.Platforms.Select(p => (AkOptionRegistryPlatform: p.IntegrationPlatformId, p.DisplayName)),
                                         viewModel.IsColumnVisible,
                                         id => viewModel.SetPlatformVisible(id, !viewModel.IsColumnVisible(id)),
                                         () => viewModel.SetAllPlatformsVisible(true),
                                         () => viewModel.SetAllPlatformsVisible(false));
        }

        public VisualElement CreateElement()
        {
            var choices = _orderedPlatforms.ToArray();
            _container = new IMGUIContainer(() =>
            {
                var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight,
                    EditorStyles.popup, GUILayout.Width(170));
                if (EditorGUI.DropdownButton(rect, new GUIContent(ComputeSummaryLabel()), FocusType.Keyboard, EditorStyles.popup))
                {
                    UnityEditor.PopupWindow.Show(rect, new Popup(this, choices));
                }
            });
            _container.style.width = 174;
            _container.style.flexShrink = 0;
            _container.style.marginTop = 4;
            _container.style.marginBottom = 4;
            _container.style.marginLeft = 4;
            _container.style.marginRight = 4;
            return _container;
        }

        // Call after changing visibility from outside the popup (e.g. a header context menu) so
        // the button's summary text catches up.
        public void Repaint()
        {
            _container?.MarkDirtyRepaint();
        }

        // Self-labeling summary shown on the dropdown button.
        private string ComputeSummaryLabel()
        {
            var visible = _orderedPlatforms.Where(p => _isVisible(p.Id)).ToList();
            if (visible.Count == 0)
            {
                return "No Platforms";
            }
            if (visible.Count == _orderedPlatforms.Count)
            {
                return "All Platforms";
            }
            if (visible.Count == 1)
            {
                return visible[0].Label;
            }
            return "Multiple Platforms";
        }

        private class Popup : PopupWindowContent
        {
            private readonly PlatformMaskField _owner;
            private readonly (string Id, string Label)[] _choices;
            private Vector2 _scroll;

            public Popup(PlatformMaskField owner, (string Id, string Label)[] choices)
            {
                _owner = owner;
                _choices = choices;
            }

            public override Vector2 GetWindowSize()
            {
                var rowPitch = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var rows = Mathf.Min(_choices.Length + 2, 14);
                var width = 150f;
                foreach (var choice in _choices)
                {
                    width = Mathf.Max(width, EditorStyles.label.CalcSize(new GUIContent(choice.Label)).x + 30f);
                }
                if (_choices.Length + 2 > 14)
                {
                    width += 16f;
                }
                return new Vector2(width, rows * rowPitch + 6f);
            }

            public override void OnGUI(Rect rect)
            {
                _scroll = EditorGUILayout.BeginScrollView(_scroll, GUIStyle.none, GUI.skin.verticalScrollbar);

                var allVisible = _choices.All(c => _owner._isVisible(c.Id));
                if (EditorGUILayout.ToggleLeft("All Platforms", allVisible) && !allVisible)
                {
                    _owner._showAll();
                }

                foreach (var (id, label) in _choices)
                {
                    var wasOn = _owner._isVisible(id);
                    var nowOn = EditorGUILayout.ToggleLeft(label, wasOn);
                    if (nowOn != wasOn)
                    {
                        _owner._toggle(id);
                    }
                }

                EditorGUILayout.EndScrollView();
            }
        }
    }
}
