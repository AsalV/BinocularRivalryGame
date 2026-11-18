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
using System.Globalization;
using System.Linq;
using AK.Wwise.Unity.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings.Editor.Common.UI
{
    /// <summary>
    /// Settings view for Wwise AkOptions and Runtime Settings
    /// Builds and styles elements only. read and write of option data goes through
    /// <see cref="WwiseSettingsViewModel"/>.
    /// </summary>
    public class WwiseSettingsView : SettingsProvider
    {
        private const string EngineSettingsPath = "Project/Wwise/SoundEngine Init";
        private const string RuntimeSettingsPath = "Project/Wwise/Runtime Settings";
        private const float LabelColumnWidth = 280f;
        private const float PlatformColumnWidth = 150f;
        private const float TooltipMaxWidth = 300f;
        private const float NamespaceTooltipCenteringCorrection = 10f;
        // Fixed row heights keep the static label column visually aligned with the
        // horizontally-scrolling cell table.
        private const float RowHeight = 22f;
        private const float PlatformSpecificSettingOffset = 30f;
        private const float PlatformSpecificSettingWidth = 140f;
        private const float RevertButtonSize = 18f;
        private const float RevertIconSize = 16f;
        private const float RevertButtonGapFromWidget = 2f;
        private const float RevertButtonTrailingGap = 16f;
        private const float RevertButtonRightOffset = RevertButtonTrailingGap - 2f;
        private const float RevertOverlayGutter = RevertButtonGapFromWidget + RevertButtonSize + RevertButtonRightOffset;

        // EditorGUI.k_OverrideMarginColor (prefab override margin)
        private static readonly Color OverrideColor = new Color32(0x01, 0x99, 0xEB, 0xBF);
        // --unity-colors-label-text-focus
        private static readonly Color OverrideTextColorDark = new Color32(0x81, 0xB4, 0xFF, 0xFF);
        private static readonly Color OverrideTextColorLight = new Color32(0x00, 0x3C, 0x88, 0xFF);
        private static Color OverrideTextColor => EditorGUIUtility.isProSkin ? OverrideTextColorDark : OverrideTextColorLight;
        private static readonly Color MissingColor = new Color(0.5f, 0.5f, 0.5f);
        private static readonly Color PlatformSpecificBarColor = new Color(0.5f, 0.5f, 0.5f);

        private const float OverrideBarWidth = 3f;

        // The backing store this page reads/writes: Sound Engine options or runtime settings.
        private readonly IAkOptionDataSource _source;

        // Used in Runtime Settings Page
        public bool ShowAdvancedByDefault { get; set; }
        public bool ExpandAllByDefault { get; set; }

        public WwiseSettingsView(string path, SettingsScope scopes, IAkOptionDataSource source, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
            _source = source;
        }

        [SettingsProvider]
        public static SettingsProvider CreateEngineProvider()
        {
            return new WwiseSettingsView(EngineSettingsPath, SettingsScope.Project, new AkOptionStoreSource())
            {
                label = "SoundEngine Options"
            };
        }

        [SettingsProvider]
        public static SettingsProvider CreateRuntimeProvider()
        {
            return new WwiseSettingsView(RuntimeSettingsPath, SettingsScope.Project, new WwiseRuntimeSettingsSource())
            {
                label = "Runtime Settings",
                ShowAdvancedByDefault = true,
                ExpandAllByDefault = true,
            };
        }

        private class MasterHorizontalScroll
        {
            private VisualElement _row;
            private ScrollView _masterView;
            private VisualElement _spacer;
            private readonly List<(VisualElement viewport, VisualElement content)> _registered = new List<(VisualElement viewport, VisualElement content)>();
            private float _offset;

            public VisualElement CreateElement()
            {
                _row = new VisualElement { style = { flexDirection = FlexDirection.Row, flexShrink = 0 } };
                _row.Add(new VisualElement { style = { flexShrink = 0, width = LabelColumnWidth } });

                _masterView = new ScrollView(ScrollViewMode.Horizontal)
                {
                    style = { flexGrow = 1, flexShrink = 1 }
                };
                _masterView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                _spacer = new VisualElement { style = { height = 1, width = 0 } };
                _masterView.Add(_spacer);
                _masterView.horizontalScroller.valueChanged += OnMasterScrolled;
                _row.Add(_masterView);

                _row.style.display = DisplayStyle.None;

                return _row;
            }

            public void Register(VisualElement viewport, VisualElement content)
            {
                content.style.left = -_offset;
                _registered.Add((viewport, content));
                viewport.RegisterCallback<GeometryChangedEvent>(_ => RecomputeRange());
                content.RegisterCallback<GeometryChangedEvent>(_ => RecomputeRange());
                RecomputeRange();
            }

            private void OnMasterScrolled(float newValue)
            {
                _offset = newValue;
                ApplyOffsetToChildren();
            }

            private void ApplyOffsetToChildren()
            {
                foreach (var (viewport, content) in _registered)
                {
                    var maxOffset = Mathf.Max(0f, content.layout.width - viewport.layout.width);
                    content.style.left = -Mathf.Min(_offset, maxOffset);
                }
            }

            private void RecomputeRange()
            {
                var maxContentWidth = 0f;
                var viewportWidth = 0f;
                foreach (var (viewport, content) in _registered)
                {
                    maxContentWidth = Mathf.Max(maxContentWidth, content.layout.width);
                    viewportWidth = Mathf.Max(viewportWidth, viewport.layout.width);
                }
                _spacer.style.width = maxContentWidth;

                var needsScroll = viewportWidth > 0f && maxContentWidth > viewportWidth + 1f;
                _row.style.display = needsScroll ? DisplayStyle.Flex : DisplayStyle.None;

                var maxOffset = Mathf.Max(0f, maxContentWidth - viewportWidth);
                if (_offset > maxOffset)
                {
                    _offset = maxOffset;
                    _masterView.scrollOffset = new Vector2(_offset, 0f);
                }
                ApplyOffsetToChildren();
            }
        }

        // Created fresh on each OnActivate so re-opening the page doesn't carry stale state.
        private WwiseSettingsViewModel _viewModel;
        private WwiseOptionUndoRedo _undo;

        private bool _forceWidgetRefresh;
        private WwiseSettingsViewIndex _index;
        private PlatformMaskField _mask;
        private MasterHorizontalScroll _masterScroll;

        // The VisualElement for the page's root
        private VisualElement _root;

        // Platforms whose column was shown at the last apply.
        private readonly HashSet<string> _shownColumns = new HashSet<string>();

        // The bottom "Platform-specific namespaces" wrapper and its contents, kept for tint refresh.
        private Foldout _platformSpecificWrapper;
        private List<NamespaceEntry> _platformSpecificNamespaces = new List<NamespaceEntry>();

        // One entry per rendered platform-specific group.
        private readonly List<(VisualElement element, NamespaceEntry entry, List<OptionInfo> options)>
            _platformSpecificGroups = new List<(VisualElement, NamespaceEntry, List<OptionInfo>)>();

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            rootElement.Clear();

            try
            {
                BuildPage(searchContext, rootElement);
            }
            catch (Exception e)
            {
                WwiseLogger.LogFormat(LogLevel.Error, "Failed to build the {0} settings page: {1}", label, e);
                rootElement.Clear();
                rootElement.Add(new Label($"Failed to build the {label} page. See the Console for details."));
            }
        }

        private void BuildPage(string searchContext, VisualElement rootElement)
        {
            _viewModel = new WwiseSettingsViewModel(_source, AkOptionPathGetter.RegisteredPlatformIds,
                                                    AkOptionPathGetter.EditorHostPlatformId, label);
            if (_viewModel.Platforms.Count == 0)
            {
                rootElement.Add(new Label("No platforms registered."));
                return;
            }

            _viewModel.OptionChanged += OnModelOptionChanged;
            _undo = new WwiseOptionUndoRedo(ReplayOptionChanges);

            _index = new WwiseSettingsViewIndex();
            _root = rootElement;
            _platformSpecificWrapper = null;
            _platformSpecificGroups.Clear();

            var platforms = _viewModel.Platforms;
            var union = _viewModel.Namespaces;

            var toolbarState = new ToolbarState(settingsPath, ShowAdvancedByDefault, ExpandAllByDefault);
            _viewModel.ShowAdvanced = toolbarState.ShowAdvanced;

            _viewModel.SetHiddenPlatforms(toolbarState.HiddenPlatformIds);
            _viewModel.SearchText = searchContext ?? string.Empty;
            _viewModel.FiltersChanged += ApplyVisibility;

            _shownColumns.Clear();
            _shownColumns.UnionWith(platforms
                .Select(p => p.IntegrationPlatformId)
                .Where(_viewModel.IsColumnVisible));

            _masterScroll = new MasterHorizontalScroll();

            var toolbar = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    flexShrink = 0,
                }
            };

            _mask = PlatformMaskField.ForViewModel(_viewModel);
            toolbar.Add(_mask.CreateElement());

            toolbar.Add(WwiseSettingsToolbarButtons.MakeAdvancedToggle(_viewModel));

            var expandAllBtn = WwiseSettingsToolbarButtons.MakeIconButton(
                WwiseSettingsToolbarButtons.ExpandAllIcon, "Expand All",
                () => SetAllFoldouts(rootElement, expand: true));
            toolbar.Add(expandAllBtn);

            var collapseAllBtn = WwiseSettingsToolbarButtons.MakeIconButton(
                WwiseSettingsToolbarButtons.CollapseAllIcon, "Collapse All",
                () => SetAllFoldouts(rootElement, expand: false));
            toolbar.Add(collapseAllBtn);

            var searchBar = WwiseSettingsToolbarButtons.MakeSearchField(_viewModel, $"Search in {label}...");
            searchBar.style.width = new Length(33, LengthUnit.Percent);
            searchBar.style.minWidth = 300;
            toolbar.Add(searchBar);

            var spacer = new VisualElement { style = { flexGrow = 1 } };
            toolbar.Add(spacer);

            var resetAllBtn = new Button(ResetAllOverrides)
            {
                text = "Reset All",
                tooltip = "Reset All",
            };
            resetAllBtn.style.marginTop = 4;
            resetAllBtn.style.marginBottom = 4;
            resetAllBtn.style.flexShrink = 0;
            toolbar.Add(resetAllBtn);

            rootElement.Add(toolbar);

            // Vertical-only at the page level, horizontal scrolling is custom.
            var scroll = new ScrollView(ScrollViewMode.Vertical)
            {
                style = { flexGrow = 1 },
            };
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            rootElement.Add(scroll);

            rootElement.Add(_masterScroll.CreateElement());

            // Split top-level namespaces: Cross-platform at the top, platform specific at the bottom.
            var crossNamespaces = union.Where(m => m.CrossPlatform.Count > 0).ToList();
            _platformSpecificNamespaces = union.Where(m => m.CrossPlatform.Count == 0).ToList();

            foreach (var namespaceEntry in crossNamespaces)
            {
                AddNamespaceFoldout(scroll, namespaceEntry, platforms, false);
            }

            if (_platformSpecificNamespaces.Count > 0)
            {
                var wrapper = new Foldout
                {
                    value = false,
                };
                wrapper.style.flexShrink = 0;
                _platformSpecificWrapper = wrapper;

                RefreshPlatformSpecificWrapper();

                TintFoldoutHeader(wrapper, _viewModel.AnyOverrideIn(_platformSpecificNamespaces));
                scroll.Add(wrapper);

                foreach (var namespaceEntry in _platformSpecificNamespaces)
                {
                    AddNamespaceFoldout(wrapper, namespaceEntry, platforms, true);
                }
            }

            if (toolbarState.ExpandAll)
            {
                SetAllFoldouts(rootElement, expand: true);
            }
            else
            {
                // Reopen what was open last time.
                foreach (var namespaceName in toolbarState.ExpandedNamespaces)
                {
                    if (_index.TryGetNamespaceFoldout(namespaceName, out var foldout))
                    {
                        foldout.value = true;
                    }
                }
            }
            ApplyVisibility();
        }

        public override void OnDeactivate()
        {
            var toolbarState = new ToolbarState(settingsPath);

            // A search force-expands its matches, so capturing then would record the search's
            // expansion rather than the user's.
            var searching = _viewModel?.HasActiveSearch ?? false;
            if (_index != null && !searching)
            {
                toolbarState.ExpandedNamespaces = new HashSet<string>(
                    _index.NamespaceFoldouts.Where(kv => kv.Value.value).Select(kv => kv.Key));
                toolbarState.ExpandAll = false;
            }

            if (_viewModel != null)
            {
                _viewModel.OptionChanged -= OnModelOptionChanged;
                _viewModel.FiltersChanged -= ApplyVisibility;
                toolbarState.ShowAdvanced = _viewModel.ShowAdvanced;
                toolbarState.HiddenPlatformIds = new HashSet<string>(_viewModel.HiddenPlatformIds);
            }

            // Drops the page's entries from the Editor's global undo stack since the next activate
            // reloads from disk.
            _undo?.Dispose();
            _undo = null;
        }

        // Re-applies the viewmodel's filter state to the page.
        private void ApplyVisibility()
        {
            if (_index == null || _root == null)
            {
                return;
            }

            foreach (var p in _viewModel.Platforms)
            {
                var id = p.IntegrationPlatformId;
                var visible = _viewModel.IsColumnVisible(id);
                if (visible == _shownColumns.Contains(id))
                {
                    continue;
                }

                var display = visible ? DisplayStyle.Flex : DisplayStyle.None;
                foreach (var e in _index.GetColumn(id))
                {
                    e.style.display = display;
                }
                if (visible)
                {
                    _shownColumns.Add(id);
                }
                else
                {
                    _shownColumns.Remove(id);
                }
            }

            // A search has to open what it matches: rows build on first expand, so a collapsed
            // namespace has none to filter. Without a search, expansion is the user's own.
            var forceExpand = _viewModel.HasActiveSearch;

            foreach (var entry in _viewModel.Namespaces)
            {
                if (!_index.TryGetNamespaceFoldout(entry.NamespaceName, out var foldout))
                {
                    continue;
                }

                var namespaceVisible = _viewModel.IsNamespaceVisible(entry);
                foldout.style.display = namespaceVisible ? DisplayStyle.Flex : DisplayStyle.None;
                if (namespaceVisible && forceExpand && !foldout.value)
                {
                    foldout.value = true;
                }

                foreach (var info in entry.AllOptions)
                {
                    var key = new OptionKey(entry.NamespaceName, info.Name);
                    if (!_index.Rows.TryGetValue(key, out var rows))
                    {
                        continue;
                    }

                    var rowVisible = _viewModel.IsRowVisible(entry, info);
                    SetRowDisplay(rows.labelRow, rows.cellRow, rowVisible);
                    if (rowVisible && forceExpand)
                    {
                        // A match can sit inside a collapsed wrapper
                        ExpandAncestorFoldouts(rows.labelRow ?? rows.cellRow);
                    }
                }
            }

            foreach (var group in _platformSpecificGroups)
            {
                RefreshPlatformSpecificGroup(group);
            }

            RefreshPlatformSpecificWrapper();
            _mask?.Repaint();
        }

        // Registers a platform-specific group and gives it its starting visibility.
        private void TrackSpecificGroup(VisualElement element, NamespaceEntry entry, List<OptionInfo> options)
        {
            var group = (element, entry, options);
            _platformSpecificGroups.Add(group);
            RefreshPlatformSpecificGroup(group);
        }

        // A group shows only while it still holds a row that passes the filters, so hiding the
        // last platform it covers takes the header and bar with it.
        private void RefreshPlatformSpecificGroup(
            (VisualElement element, NamespaceEntry entry, List<OptionInfo> options) group)
        {
            var anyVisible = group.options.Any(o => _viewModel.IsRowVisible(group.entry, o));
            group.element.style.display = anyVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void SetRowDisplay(VisualElement labelRow, VisualElement cellRow, bool visible)
        {
            var display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (labelRow != null)
            {
                labelRow.style.display = display;
            }
            if (cellRow != null)
            {
                cellRow.style.display = display;
            }
        }

        private static void ExpandAncestorFoldouts(VisualElement element)
        {
            for (var node = element?.parent; node != null; node = node.parent)
            {
                if (node is Foldout f && !f.value)
                {
                    f.value = true;
                }
            }
        }

        // Both the label and the visibility of the bottom wrapper follow how many of its
        // namespaces currently pass the filters.
        private void RefreshPlatformSpecificWrapper()
        {
            if (_platformSpecificWrapper == null)
            {
                return;
            }
            var visibleCount = _platformSpecificNamespaces.Count(_viewModel.IsNamespaceVisible);
            _platformSpecificWrapper.text = $"Platform-specific namespaces ({visibleCount})";
            _platformSpecificWrapper.style.display = visibleCount == 0 ? DisplayStyle.None : DisplayStyle.Flex;
        }

        // Restyle exactly the cells the change touched, then refresh the foldout tints above them.
        private void OnModelOptionChanged(OptionKey key, IReadOnlyCollection<string> changedPlatformIds)
        {
            foreach (var platformId in changedPlatformIds)
            {
                if (!_index.TryGetCell(platformId, key, out var widget) || widget == null)
                {
                    // Namespace not expanded yet, it will read current state when it builds.
                    continue;
                }
                var p = _viewModel.GetPlatform(platformId);
                if (p == null)
                {
                    continue;
                }

                // Don't push a value into the widget the user is typing in.
                if (_forceWidgetRefresh || !HasFocusWithin(widget))
                {
                    SetWidgetValueWithoutNotify(widget, _viewModel.GetValue(platformId, key.Namespace, key.Option));
                }
                ApplyOverrideStyle(widget, _viewModel.IsOverridden(platformId, key.Namespace, key.Option),
                                   p, key.Namespace, key.Option);
            }

            if (_index.TryGetNamespaceFoldout(key.Namespace, out var namespaceFoldout))
            {
                TintFoldoutHeader(namespaceFoldout, _viewModel.NamespaceHasAnyOverride(key.Namespace));
            }
            if (_platformSpecificWrapper != null)
            {
                TintFoldoutHeader(_platformSpecificWrapper, _viewModel.AnyOverrideIn(_platformSpecificNamespaces));
            }
        }

        private void SetOption(string platformId, string namespaceName, string optionName, object newValue)
        {
            _undo.Push(_viewModel.SetOption(platformId, namespaceName, optionName, newValue),
                       $"Wwise: set {namespaceName}.{optionName}");
        }

        private void RevertToDefault(string platformId, string namespaceName, string optionName)
        {
            RunForceRefreshAction(() =>
                _undo.Push(_viewModel.RevertToDefault(platformId, namespaceName, optionName),
                           $"Wwise: revert {namespaceName}.{optionName}"));
        }

        private void ApplyValueAcrossRow(string platformId, string namespaceName, string optionName,
                                         List<string> targetPlatformIds)
        {
            RunForceRefreshAction(() =>
                _undo.Push(_viewModel.ApplyValueAcrossRow(platformId, namespaceName, optionName, targetPlatformIds),
                           $"Wwise: apply {namespaceName}.{optionName} to all platforms"));
        }

        // Undo/redo replay.
        private void ReplayOptionChanges(List<WwiseOptionChange> changes, bool isRedo)
        {
            RunForceRefreshAction(() =>
            {
                foreach (var change in changes)
                {
                    _viewModel.SetOption(change.PlatformId, change.NamespaceName, change.OptionName,
                                         isRedo ? change.NewValue : change.PreviousValue);
                }
            });
        }

        private void RunForceRefreshAction(Action write)
        {
            _forceWidgetRefresh = true;
            try
            {
                write();
            }
            finally
            {
                _forceWidgetRefresh = false;
            }
        }

        private static bool HasFocusWithin(VisualElement widget)
        {
            var focused = widget.panel?.focusController?.focusedElement as VisualElement;
            return focused != null && (focused == widget || widget.Contains(focused));
        }

        private void ResetAllOverrides()
        {
            var totalOverrides = _viewModel.TotalOverrideCount;
            if (totalOverrides == 0)
            {
                return;
            }

            var message =
                $"This clears every project override across all platforms for \"{label}\" ({totalOverrides} option(s) total).";

            // Orphans have no current default to restore
            var orphanedCount = _viewModel.OrphanedOverrideCount;
            if (orphanedCount > 0)
            {
                message += $"\n\n{orphanedCount} of these cannot be restored by Undo.";
            }

            var confirmed = EditorUtility.DisplayDialog("Reset All Overrides", message, "Reset All", "Cancel");
            if (!confirmed)
            {
                return;
            }

            RunForceRefreshAction(() =>
                _undo.Push(_viewModel.ResetAllOverrides(), "Wwise: reset all option overrides"));
        }

        private void AddNamespaceFoldout(VisualElement parent, NamespaceEntry namespaceEntry,
                                         IReadOnlyList<PlatformState> platforms, bool platformSpecific)
        {
            var captured = namespaceEntry;
            var namespaceFoldOut = new Foldout
            {
                text = captured.PlatformDisplayName ?? captured.DisplayName,
                value = false,
            };
            namespaceFoldOut.style.flexShrink = 0;
            namespaceFoldOut.style.unityFontStyleAndWeight = FontStyle.Bold;
            namespaceFoldOut.style.borderBottomWidth = 1;
            namespaceFoldOut.style.borderBottomColor = PlatformSpecificBarColor;
            if (!_viewModel.IsNamespaceVisible(captured))
            {
                namespaceFoldOut.style.display = DisplayStyle.None;
            }
            TintFoldoutHeader(namespaceFoldOut, captured.ModifiedOptionCount > 0);
            AttachNamespaceTooltip(namespaceFoldOut, captured.NamespaceName, platforms);

            // Lazy populate on first expand.
            var built = false;
            void EnsureBuilt()
            {
                if (built)
                {
                    return;
                }
                built = true;
                PopulateNamespace(namespaceFoldOut, captured, platforms);
            }
            namespaceFoldOut.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    EnsureBuilt();
                }
            });

            parent.Add(namespaceFoldOut);
            _index.AddNamespace(captured.NamespaceName, namespaceFoldOut);
        }

        private static void AttachNamespaceTooltip(Foldout foldout, string namespaceName,
                                                   IReadOnlyList<PlatformState> platforms)
        {
            var firstnamespace = platforms
                .Select(p => p.EffectiveDefaults.FirstOrDefault(n => n.Name == namespaceName))
                .FirstOrDefault(m => m != null && !string.IsNullOrEmpty(m.Doc));
            if (firstnamespace == null)
            {
                return;
            }

            var header = foldout.Q<Toggle>(className: "unity-foldout__toggle");
            var headerLabel = header?.Q<Label>();
            if (headerLabel == null)
            {
                return;
            }

            headerLabel.RegisterCallback<TooltipEvent>(evt =>
            {
                var tooltipStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("Tooltip");
                var content = new GUIContent(firstnamespace.Doc);
                var tooltipWidth = tooltipStyle != null
                    ? tooltipStyle.CalcSize(content).x
                    : TooltipMaxWidth;
                tooltipWidth = Mathf.Min(tooltipWidth, TooltipMaxWidth);

                var rect = headerLabel.worldBound;
                rect.x += tooltipWidth / 2f - NamespaceTooltipCenteringCorrection;
                rect.width = 0f;
                evt.rect = rect;
                evt.tooltip = firstnamespace.Doc;
                evt.StopImmediatePropagation();
            });
        }

        private void PopulateNamespace(Foldout foldout, NamespaceEntry namespaceEntry,
                                       IReadOnlyList<PlatformState> platforms)
        {
            var namespaceName = namespaceEntry.NamespaceName;

            var mergedByPlatform = platforms.ToDictionary(p => p, p => p.Merged);

            if (namespaceEntry.CrossPlatform.Count > 0)
            {
                BuildMatrix(foldout, namespaceEntry.CrossPlatform, platforms, mergedByPlatform, namespaceEntry);
            }

            if (namespaceEntry.PlatformSpecificGroups.Count == 0)
            {
                return;
            }

            // Platform-specific
            VisualElement platformSpecificContainer;
            if (namespaceEntry.CrossPlatform.Count > 0)
            {
                var specificWrapper = new Foldout { text = "Platform-specific", value = false };
                specificWrapper.style.flexShrink = 0;
                foldout.Add(specificWrapper);
                platformSpecificContainer = specificWrapper;
            }
            else
            {
                platformSpecificContainer = foldout;
            }

            var platformById = platforms.ToDictionary(p => p.IntegrationPlatformId);
            foreach (var group in namespaceEntry.PlatformSpecificGroups)
            {
                var supportingPlatforms = group.SupportSet
                    .Where(id => platformById.ContainsKey(id))
                    .Select(id => platformById[id])
                    .ToList();
                if (supportingPlatforms.Count == 0)
                {
                    continue;
                }

                if (supportingPlatforms.Count == 1)
                {
                    var p = supportingPlatforms[0];

                    // If the namespace foldout's header matches the platform ID or its SDK name, skip the inner foldout and
                    // inline the rows.
                    VisualElement container;
                    if (p.MatchesPlatform(namespaceName))
                    {
                        container = platformSpecificContainer;
                    }
                    else
                    {
                        var platformFoldout = new Foldout { text = p.DisplayName, value = true };
                        platformFoldout.style.flexShrink = 0;
                        platformSpecificContainer.Add(platformFoldout);
                        container = platformFoldout;
                    }

                    VisualElement rows = new VisualElement();
                    rows.style.borderTopWidth = 3;
                    rows.style.borderTopColor = PlatformSpecificBarColor;
                    rows.style.maxWidth = LabelColumnWidth + PlatformColumnWidth + PlatformSpecificSettingOffset;
                    foreach (var info in group.Options)
                    {
                        var row = BuildPlatformSpecificRow(p, mergedByPlatform[p], namespaceName, info);
                        if (!_viewModel.IsRowVisible(namespaceEntry, info))
                        {
                            row.style.display = DisplayStyle.None;
                        }
                        rows.Add(row);
                        // Single-platform rows have only one element,
                        // so register with cellRow = null so only the one gets toggled.
                        _index.AddRow(namespaceName, info.Name, row, null);
                    }

                    container.Add(rows);
                    // When the rows are inlined straight into the shared container there is no
                    // per-group foldout, so the bar wrapping them stands in for the group.
                    TrackSpecificGroup(ReferenceEquals(container, platformSpecificContainer) ? rows : container,
                                       namespaceEntry, group.Options);
                }
                else
                {
                    // Multi-platform group (related platforms): one column per supporting platform. The foldout
                    // header names the platforms instead of using a generic label.
                    var groupFoldout = new Foldout
                    {
                        text = string.Join(", ", supportingPlatforms.Select(p => p.DisplayName)),
                        value = true,
                    };
                    groupFoldout.style.flexShrink = 0;
                    platformSpecificContainer.Add(groupFoldout);

                    BuildMatrix(groupFoldout, group.Options, supportingPlatforms, mergedByPlatform, namespaceEntry);
                    TrackSpecificGroup(groupFoldout, namespaceEntry, group.Options);
                }
            }
        }

        // Render a matrix with the given options as rows and the given platforms as columns.
        // Used for both the main cross-platform matrix and per-group mini-matrices.
        private void BuildMatrix(VisualElement container, List<OptionInfo> options,
                                 IReadOnlyList<PlatformState> platforms,
                                 Dictionary<PlatformState, List<AkOptionNamespace>> mergedByPlatform,
                                 NamespaceEntry namespaceEntry)
        {
            var namespaceName = namespaceEntry.NamespaceName;

            var outer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexShrink = 0,
                }
            };
            container.Add(outer);

            var labelColumn = new VisualElement
            {
                style =
                {
                    flexShrink = 0,
                    flexGrow = 0,
                }
            };
            outer.Add(labelColumn);

            var cellViewport = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    overflow = Overflow.Hidden,
                }
            };
            outer.Add(cellViewport);

            var cellTable = new VisualElement
            {
                style =
                {
                    flexShrink = 0,
                    position = Position.Relative,
                    alignSelf = Align.FlexStart
                }
            };
            cellViewport.Add(cellTable);
            _masterScroll.Register(cellViewport, cellTable);

            // Header row: spacer in the label column, platform IDs in the cell table.
            labelColumn.Add(MakeRowOfHeight(RowHeight, LabelColumnWidth));

            var cellHeaderRow = MakeRow();
            cellHeaderRow.style.height = RowHeight;
            foreach (var p in platforms)
            {
                var headerCell = MakeFixedLabel(p.DisplayName, PlatformColumnWidth, isHeader: true);
                _index.AddColumnElement(p.IntegrationPlatformId, headerCell);
                if (!_viewModel.IsColumnVisible(p.IntegrationPlatformId))
                {
                    headerCell.style.display = DisplayStyle.None;
                }
                cellHeaderRow.Add(headerCell);
            }
            cellTable.Add(cellHeaderRow);

            // Resolve each platform's namespace -> option-platform lookup once
            var optionsByPlatform = platforms.ToDictionary(
                p => p,
                p => mergedByPlatform[p].FirstOrDefault(n => n.Name == namespaceName)?.OptionsByName());

            var matrixPlatformIds = platforms.Select(p => p.IntegrationPlatformId).ToList();

            foreach (var info in options)
            {
                var labelRow = MakeRowOfHeight(RowHeight, LabelColumnWidth);
                var nameLabel = MakeFixedLabel(info.DisplayName, LabelColumnWidth, isHeader: false);
                nameLabel.tooltip = string.IsNullOrEmpty(info.Doc) ? info.Name : info.Doc;
                if (info.IsAdvanced)
                {
                    // Italic on the label so advanced rows stay recognizable when "Show advanced"
                    // is on.
                    nameLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
                }
                labelRow.Add(nameLabel);
                labelColumn.Add(labelRow);

                var cellRow = MakeRow();
                cellRow.style.height = RowHeight;
                foreach (var p in platforms)
                {
                    cellRow.Add(BuildCell(p, optionsByPlatform[p], namespaceName, info.Name, matrixPlatformIds));
                }
                cellTable.Add(cellRow);

                SetRowDisplay(labelRow, cellRow, _viewModel.IsRowVisible(namespaceEntry, info));

                _index.AddRow(namespaceName, info.Name, labelRow, cellRow);
            }
        }

        // A flex-row container of fixed height/width, used as a left-column row that aligns
        // with the cell table's rows on the right.
        private static VisualElement MakeRowOfHeight(float height, float width)
        {
            return new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    flexShrink = 0,
                    height = height,
                    width = width,
                    minWidth = width,
                    maxWidth = width,
                }
            };
        }

        private static VisualElement MakeRow()
        {
            return new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    flexShrink = 0,
                }
            };
        }

        private static Label MakeFixedLabel(string text, float width, bool isHeader)
        {
            return new Label(text)
            {
                style =
                {
                    width = width,
                    minWidth = width,
                    maxWidth = width,
                    paddingLeft = 4,
                    paddingRight = 4,
                    overflow = Overflow.Hidden,
                    textOverflow = TextOverflow.Ellipsis,
                    unityFontStyleAndWeight = isHeader ? FontStyle.Bold : FontStyle.Normal,
                }
            };
        }

        private VisualElement BuildCell(PlatformState p, Dictionary<string, AkOptionEntry> optionsByName,
                                        string namespaceName, string optionName,
                                        List<string> matrixPlatformIds)
        {
            var platformId = p.IntegrationPlatformId;
            var cell = new VisualElement
            {
                style =
                {
                    width = PlatformColumnWidth,
                    minWidth = PlatformColumnWidth,
                    maxWidth = PlatformColumnWidth,
                    height = RowHeight,
                    paddingLeft = 2,
                    paddingRight = 2,
                    overflow = Overflow.Hidden,
                    borderLeftWidth = OverrideBarWidth,
                    borderLeftColor = Color.clear,
                }
            };
            _index.AddColumnElement(platformId, cell);
            if (!_viewModel.IsColumnVisible(platformId))
            {
                cell.style.display = DisplayStyle.None;
            }

            // Option may not exist on every platform
            AkOptionEntry entry = null;
            optionsByName?.TryGetValue(optionName, out entry);
            if (entry == null)
            {
                cell.Add(new Label("\x2014") { style = { color = MissingColor, unityTextAlign = TextAnchor.MiddleCenter } });
                return cell;
            }

            var widget = WwiseSettingsUIWidgets.Create(entry,
                newValue => SetOption(platformId, namespaceName, optionName, newValue),
                showLabel: false);

            widget.style.flexGrow = 1;
            widget.style.marginRight = RevertOverlayGutter;

            // Add to the cell first so ApplyOverrideStyle can reach widget.parent (the cell)
            cell.Add(widget);
            _index.AddCell(platformId, namespaceName, optionName, widget);
            ApplyOverrideStyle(widget, _viewModel.IsOverridden(platformId, namespaceName, optionName),
                               p, namespaceName, optionName, isInitialBuild: true);
            AttachRevertMenu(cell, p, namespaceName, optionName, applyMatrixPlatformIds: matrixPlatformIds);
            return cell;
        }

        // Single-editor row for an option that only exists on one platform.
        private VisualElement BuildPlatformSpecificRow(PlatformState p, List<AkOptionNamespace> merged,
                                               string namespaceName, OptionInfo info)
        {
            var platformId = p.IntegrationPlatformId;
            var row = MakeRow();

            var nameLabel = MakeFixedLabel(info.DisplayName, LabelColumnWidth, isHeader: false);
            nameLabel.tooltip = string.IsNullOrEmpty(info.Doc) ? info.Name : info.Doc;
            if (info.IsAdvanced)
            {
                nameLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            }
            row.Add(nameLabel);

            var @namespace = merged.FirstOrDefault(n => n.Name == namespaceName);
            var entry = @namespace?.Options.FirstOrDefault(o => o.Name == info.Name);
            if (entry == null)
            {
                row.Add(new Label("(missing)") { style = { color = MissingColor } });
                return row;
            }

            var widget = WwiseSettingsUIWidgets.Create(entry,
                newValue => SetOption(platformId, namespaceName, info.Name, newValue),
                showLabel: false);

            widget.style.width = PlatformSpecificSettingWidth;
            widget.style.flexGrow = 0;
            // Reserve room on the right for the revert overlay
            widget.style.marginRight = RevertOverlayGutter;

            var valueContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    minWidth = PlatformColumnWidth + RevertOverlayGutter,
                    borderLeftWidth = OverrideBarWidth,
                    borderLeftColor = Color.clear,
                }
            };
            valueContainer.Add(widget);

            // Add to the row first so ApplyOverrideStyle can reach widget.parent (valueContainer)
            // and, if needed, lazily add the revert button there.
            row.Add(valueContainer);
            _index.AddCell(platformId, namespaceName, info.Name, widget);
            ApplyOverrideStyle(widget, _viewModel.IsOverridden(platformId, namespaceName, info.Name),
                               p, namespaceName, info.Name, isInitialBuild: true);
            AttachRevertMenu(row, p, namespaceName, info.Name);
            return row;
        }

        // Build the revert button lazily
        private void ApplyOverrideStyle(VisualElement widget, bool isOverridden, PlatformState p,
                                        string namespaceName, string optionName,
                                        bool isInitialBuild = false)
        {
            if (isInitialBuild && !isOverridden)
            {
                return;
            }

            // Style the *parent* cell/row container, not the widget itself: Unity's Toggle and
            // similar fields don't visibly render a border applied to their root element, and
            // the cell has fixed dimensions so a background/border is always visible regardless
            // of widget type. The widget itself gets text tinting for descendant value elements.
            var container = widget.parent ?? widget;
            container.style.borderLeftColor = isOverridden ? new StyleColor(OverrideColor) : new StyleColor(Color.clear);

            // Bold and tint every descendant TextElement so the displayed value (Label, value text in
            // LongField/DoubleField/TextField/DropdownField) is styled regardless of widget type.
            var color = isOverridden ? new StyleColor(OverrideTextColor) : new StyleColor(StyleKeyword.Null);
            widget.style.color = color;
            widget.Query<TextElement>().ForEach(t =>
            {
                t.style.color = color;
                if (isOverridden)
                {
                    t.style.unityFontStyleAndWeight = FontStyle.Bold;
                }
                else
                {
                    t.style.unityFontStyleAndWeight = StyleKeyword.Null;
                }
            });

            // The revert icon lives in the same container as the widget.
            var revertButton = container.Q<Button>(name: RevertButtonName);
            if (revertButton == null)
            {
                if (!isOverridden)
                {
                    return;
                }
                revertButton = MakeRevertButton(p, namespaceName, optionName);
                container.Add(revertButton);
            }
            revertButton.style.display = isOverridden ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // Name on the per-cell revert button so ApplyOverrideStyle can find and toggle it.
        private const string RevertButtonName = "matrix-revert-button";

        private static Texture2D GetRevertIcon()
        {
            return EditorGUIUtility.IconContent("Refresh").image as Texture2D;
        }

        // A small overlay button on the trailing edge of a cell. Hidden by default; shown only
        // there is currently a project override
        private Button MakeRevertButton(PlatformState p, string namespaceName, string optionName)
        {
            var platformId = p.IntegrationPlatformId;
            var defaultEntry = _viewModel.GetDefaultEntry(platformId, namespaceName, optionName);
            var tooltip = defaultEntry != null ? $"Revert to default ({defaultEntry.DefaultValue})" : "Revert to default";
            var btn = new Button(() => RevertToDefault(platformId, namespaceName, optionName))
            {
                name = RevertButtonName,
                tooltip = tooltip,
            };
            var icon = GetRevertIcon();
            if (icon != null)
            {
                btn.style.backgroundImage = new StyleBackground(icon);
                btn.style.backgroundSize = new BackgroundSize(RevertIconSize, RevertIconSize);
                btn.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
                btn.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
                btn.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            }
            var s = btn.style;
            s.position = Position.Absolute;
            s.right = RevertButtonRightOffset;
            s.top = (RowHeight - RevertButtonSize) / 2f;
            s.width = RevertButtonSize;
            s.height = RevertButtonSize;
            s.paddingLeft = 0;
            s.paddingRight = 0;
            s.paddingTop = 0;
            s.paddingBottom = 0;
            s.marginLeft = 0;
            s.marginRight = 0;
            s.marginTop = 0;
            s.marginBottom = 0;
            s.display = DisplayStyle.None;
            return btn;
        }

        private static void TintFoldoutHeader(Foldout foldout, bool isOverridden)
        {
            var header = foldout.Q<Toggle>(className: "unity-foldout__toggle");
            var headerLabel = header?.Q<Label>();
            if (headerLabel != null)
            {
                if (isOverridden)
                {
                    headerLabel.style.color = OverrideTextColor;
                    headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                }
                else
                {
                    headerLabel.style.color = StyleKeyword.Null;
                    headerLabel.style.unityFontStyleAndWeight = StyleKeyword.Null;
                }
            }
        }

        private static void SetAllFoldouts(VisualElement root, bool expand)
        {
            if (!expand)
            {
                root.Query<Foldout>().ForEach(f => f.value = false);
                return;
            }

            // Looped because namespace foldouts populate lazily. Each pass that flips one open
            // adds nested foldouts that the next pass picks up. Bounded as a safety net.
            for (var pass = 0; pass < 5; pass++)
            {
                var changed = false;
                root.Query<Foldout>().ForEach(f =>
                {
                    if (!f.value)
                    {
                        f.value = true;
                        changed = true;
                    }
                });
                if (!changed)
                {
                    break;
                }
            }
        }

        // Right-click "Revert to default" on a cell/row.
        private void AttachRevertMenu(VisualElement target, PlatformState p,
                                      string namespaceName, string optionName,
                                      List<string> applyMatrixPlatformIds = null)
        {
            var platformId = p.IntegrationPlatformId;
            target.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                var isOverridden = _viewModel.IsOverridden(platformId, namespaceName, optionName);
                evt.menu.AppendAction(
                    "Revert to default",
                    _ => RevertToDefault(platformId, namespaceName, optionName),
                    isOverridden ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                if (applyMatrixPlatformIds != null && applyMatrixPlatformIds.Count > 1)
                {
                    evt.menu.AppendSeparator(string.Empty);
                    evt.menu.AppendAction(
                        "Apply to all",
                        _ => ApplyValueAcrossRow(platformId, namespaceName, optionName,
                                                 applyMatrixPlatformIds));
                }
            }));

            // Ctrl/Cmd+click -> "Revert to default". TrickleDown so
            // it's seen before the widget turns the click into a value edit.
            target.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0 || !evt.actionKey)
                {
                    return;
                }
                if (!_viewModel.IsOverridden(platformId, namespaceName, optionName))
                {
                    return;
                }

                RevertToDefault(platformId, namespaceName, optionName);
                evt.StopPropagation();
                // Suppress focus
#if UNITY_2023_2_OR_NEWER
                (evt.target as VisualElement)?.focusController?.IgnoreEvent(evt);
#else
                evt.PreventDefault();
#endif
            }, TrickleDown.TrickleDown);
        }

        private static void SetWidgetValueWithoutNotify(VisualElement widget, object value)
        {
            if (value == null)
            {
                return;
            }
            try
            {
                switch (widget)
                {
                    case Toggle t when value is bool b:
                        t.SetValueWithoutNotify(b);
                        break;
                    case BaseField<long> lf:
                        lf.SetValueWithoutNotify(Convert.ToInt64(value, CultureInfo.InvariantCulture));
                        break;
                    case SliderInt si:
                        si.SetValueWithoutNotify((int)Convert.ToInt64(value, CultureInfo.InvariantCulture));
                        // Unfocus the Slider to remove the focus on the TextField to properly update the value
                        si.Blur();
                        break;
                    case DoubleField df:
                        df.SetValueWithoutNotify(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        break;
                    case Slider s:
                        s.SetValueWithoutNotify((float)Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        // Unfocus the Slider to remove the focus on the TextField to properly update the value
                        s.Blur();
                        break;
                    case BaseField<string> sf when value is string s:
                        sf.SetValueWithoutNotify(s);
                        break;
                }
            }
            catch (Exception ex)
            {
                WwiseLogger.LogFormat(LogLevel.Warning,
                    "Could not reset widget value of type <{0}> for {1}: {2}. " +
                    "The override has been cleared on disk; reopen the page to see the default value.",
                    value.GetType().Name, widget?.GetType().Name, ex.Message);
            }
        }
    }
}
