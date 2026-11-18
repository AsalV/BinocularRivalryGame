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
using UnityEngine.UIElements;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings.Editor.Common.UI
{
    /// <summary>
    /// UI widgets to allow display/editing of AkOption init settings.
    /// </summary>
    internal static class WwiseSettingsUIWidgets
    {
        // Slider drags fire one ChangeEvent per pixel of movement. Without debounce,
        // each tick would hit the disk via SetOption → WriteProjectOverrides.
        // Same approach as the toolbar's search-bar debounce.
        private const long SliderDebounceMs = 100;

        public static VisualElement Create(AkOptionEntry option, Action<object> onChanged = null, bool showLabel = true)
        {
            var label = showLabel ? option.DisplayName : null;

            VisualElement widget = option.Type switch
            {
                "Boolean" => CreateBoolean(option, label, onChanged),
                "Integer" => CreateInteger(option, label, onChanged),
                "Real" => CreateReal(option, label, onChanged),
                "String" => CreateString(option, label, onChanged),
                "ChannelConfig" => CreateChannelConfig(option, label, onChanged),
                "Pointer" => null,
                _ => new Label($"{option.DisplayName} : (unsupported type {option.Type})")
            };

            if (widget != null && !string.IsNullOrEmpty(option.Doc))
            {
                widget.tooltip = option.Doc;
            }

            return widget;
        }

        private static Toggle CreateBoolean(AkOptionEntry option, string label, Action<object> onChanged)
        {
            var widget = new Toggle(label) { value = option.DefaultValue is true };
            if (onChanged != null)
            {
                widget.RegisterValueChangedCallback(evt => onChanged(evt.newValue));
            }

            return widget;
        }

        private static VisualElement CreateInteger(AkOptionEntry option, string label, Action<object> onChanged)
        {
            var rawValue = option.DefaultValue is long l ? l : 0L;

            // If we have a list of AllowedValues, then we will present them as a dropdown
            if (option.AllowedValues != null && option.AllowedValues.Count > 0)
            {
                var dropdown = new AllowedValuesDropdownField<long>(label, option.AllowedValues, v => Convert.ToInt64(v));
                dropdown.SetValueWithoutNotify(rawValue);
                if (onChanged != null)
                {
                    dropdown.RegisterValueChangedCallback(evt => onChanged(evt.newValue));
                }

                return dropdown;
            }

            VisualElement widget;
            var min = option.MinValue as long?;
            var max = option.MaxValue as long?;
            var optionValue = ClampToRange(rawValue, min, max);

            // Use a slider when the bounds are defined, and fit in an int32
            if (min is long sliderMin && max is long sliderMax
                                      && sliderMin >= int.MinValue && sliderMax <= int.MaxValue)
            {
                var slider = new SliderInt(label, (int)sliderMin, (int)sliderMax)
                    { value = (int)optionValue, showInputField = true };
                widget = slider;
                if (onChanged != null)
                {
                    IVisualElementScheduledItem pending = null;
                    slider.RegisterValueChangedCallback(evt =>
                    {
                        var newValue = (long)evt.newValue;
                        pending?.Pause();
                        pending = slider.schedule.Execute(() => onChanged(newValue)).StartingIn(SliderDebounceMs);
                    });
                }
            }
            // Otherwise use a LongField
            else
            {
                // isDelayed: only commit on Enter/blur, not on every keystroke.
                var field = new LongField(label) { value = optionValue, isDelayed = true };
                SetRangeValidation(field, min, max);
                widget = field;
                if (onChanged != null)
                {
                    field.RegisterValueChangedCallback(evt => onChanged(evt.newValue));
                }
            }

            return widget;
        }

        private static VisualElement CreateReal(AkOptionEntry option, string label, Action<object> onChanged)
        {
            VisualElement widget;
            var min = option.MinValue as double?;
            var max = option.MaxValue as double?;
            var optionValue = ClampToRange(option.DefaultValue is double d ? d : 0d, min, max);
            // Slider is float-bounded; only use it when the option's range actually fits.
            // If either of the bounds are out of range or null, we will use a DoubleField instead.
            if (min is double sliderMin && max is double sliderMax
                                        && sliderMin >= float.MinValue && sliderMax <= float.MaxValue)
            {
                var slider = new Slider(label, (float)sliderMin, (float)sliderMax)
                    { value = (float)optionValue, showInputField = true };
                widget = slider;
                if (onChanged != null)
                {
                    IVisualElementScheduledItem pending = null;
                    slider.RegisterValueChangedCallback(evt =>
                    {
                        var newValue = (double)evt.newValue;
                        pending?.Pause();
                        pending = slider.schedule.Execute(() => onChanged(newValue)).StartingIn(SliderDebounceMs);
                    });
                }
            }
            else
            {
                // isDelayed: only commit on Enter/blur, not on every keystroke.
                var field = new DoubleField(label) { value = optionValue, isDelayed = true };
                SetRangeValidation(field, min, max);
                widget = field;
                if (onChanged != null)
                {
                    field.RegisterValueChangedCallback(evt => onChanged(evt.newValue));
                }
            }

            return widget;
        }

        private static VisualElement CreateString(AkOptionEntry option, string label, Action<object> onChanged)
        {
            var defaultStr = option.DefaultValue as string ?? string.Empty;

            // If we have a list of AllowedValues, then we will present them as a dropdown
            if (option.AllowedValues != null && option.AllowedValues.Count > 0)
            {
                var dropdown = new AllowedValuesDropdownField<string>(label, option.AllowedValues, v => Convert.ToString(v));
                dropdown.SetValueWithoutNotify(defaultStr);
                if (onChanged != null)
                {
                    dropdown.RegisterValueChangedCallback(evt => onChanged(evt.newValue));
                }

                return dropdown;
            }

            // Otherwise we use a plain TextField
            var text = new TextField(label) { isDelayed = true };
            if (option.MaxLength is int maxLength && maxLength > 0)
            {
                text.maxLength = maxLength;
            }
            text.value = defaultStr;
            if (onChanged != null)
            {
                // Use the InputEvent to capture the last user-created value
                string lastInputData = null;
                text.RegisterCallback<InputEvent>(evt => lastInputData = evt.newData);
                text.RegisterValueChangedCallback(_ =>
                {
                    if (lastInputData == null)
                    {
                        return;
                    }

                    onChanged(lastInputData);
                    lastInputData = null;
                });
            }

            return text;
        }

        private static VisualElement CreateChannelConfig(AkOptionEntry option, string label, Action<object> onChanged)
        {
            var optionValue = option.DefaultValue is long l ? l : 0L;

            var field = new ChannelConfigField(label);
            // Explicit call (not the `value` setter) so the preset/custom display is synced
            // even when optionValue happens to equal the field's zero-initialized default.
            field.SetValueWithoutNotify(optionValue);

            if (onChanged != null)
            {
                field.RegisterValueChangedCallback(evt => onChanged(evt.newValue));
            }

            return field;
        }


        private static readonly Dictionary<string, long> ChannelConfigNames = new()
        {
            { "Default", 0 },
            { "1.0", 16641 },
            { "2.0", 12546 },
            { "2.1", 45315 },
            { "3.0", 28931 },
            { "4.0", 6304004 },
            { "5.1", 6353158 },
            { "7.1", 6549768 },
            { "4.0.4", 761278728 },
            { "5.1.2", 90239240 },
            { "5.1.4", 761327882 },
            { "7.1.2", 90435850 },
            { "7.1.4", 761524492 },
            { "Ambisonics 1st order", 516 },
            { "Ambisonics 2nd order", 521 },
            { "Ambisonics 3rd order", 528 },
            { "Ambisonics 4th order", 537 },
            { "Ambisonics 5th order", 548 },
            { "Auro 10.1", 769716491 },
            { "Auro 11.1", 803270924 },
            { "Auro 13.1", 803467534 },
            { "LFE", 33025 }
        };

        private const string ChannelConfigContainerClass = "wwise-channel-config-container";

        // ChannelConfig is serialized as an integer bitmask.
        // Presents a dropdown of common channel configurations. When the user picks "Custom",
        // the dropdown is replaced by a LongField + a "return to presets" button. Clicking the
        // button swaps back to the dropdown, falling back to the first preset ("Default")
        // if the current value does not match any known preset.
        private class ChannelConfigField : BaseField<long>
        {
            private readonly DropdownField _dropdown;
            private readonly LongField _customField;
            private readonly Button _returnBtn;

            // Set while an internal handler is explicitly deciding the display mode.
            // Without this, SetValueWithoutNotify's preset-matching below would second-guess that choice
            // whenever the in-progress value happens to numerically match a known preset.
            private bool _suppressDisplaySync;

            public ChannelConfigField(string label)
                : this(label, new VisualElement { style = { flexDirection = FlexDirection.Row, flexGrow = 1 } })
            {
            }

            private ChannelConfigField(string label, VisualElement content) : base(label, content)
            {
                AddToClassList(ChannelConfigContainerClass);

                var choices = new System.Collections.Generic.List<string>(ChannelConfigNames.Keys) { "Custom" };

                _dropdown = new DropdownField(choices, 0) { style = { flexGrow = 1, minWidth = 0 } };
                _customField = new LongField { isDelayed = true, style = { flexGrow = 1, minWidth = 0 } };
                _returnBtn = new Button { tooltip = "Return to presets" };
                _returnBtn.text = "Preset";
                _returnBtn.style.flexShrink = 0;

                content.Add(_dropdown);
                content.Add(_customField);
                content.Add(_returnBtn);

                _dropdown.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue == "Custom")
                    {
                        ShowCustom();
                        ApplyValue(_customField.value, suppressDisplaySync: true);
                        return;
                    }

                    if (ChannelConfigNames.TryGetValue(evt.newValue, out var configValue))
                    {
                        ApplyValue(configValue, suppressDisplaySync: false);
                    }
                });

                _customField.RegisterValueChangedCallback(evt => ApplyValue(evt.newValue, suppressDisplaySync: true));

                _returnBtn.clicked += () =>
                {
                    // Fall back to the first preset if the custom value doesn't match any.
                    var presetName = FindPresetName(_customField.value) ?? choices[0];
                    if (ChannelConfigNames.TryGetValue(presetName, out var presetValue))
                    {
                        ApplyValue(presetValue, suppressDisplaySync: false);
                    }
                };
            }

            public override void SetValueWithoutNotify(long newValue)
            {
                base.SetValueWithoutNotify(newValue);
                if (_suppressDisplaySync)
                {
                    return;
                }

                _customField.SetValueWithoutNotify(newValue);
                var presetName = FindPresetName(newValue);
                if (presetName != null)
                {
                    _dropdown.SetValueWithoutNotify(presetName);
                    ShowDropdown();
                }
                else
                {
                    ShowCustom();
                }
            }

            private void ApplyValue(long newValue, bool suppressDisplaySync)
            {
                var previous = value;
                _suppressDisplaySync = suppressDisplaySync;
                try
                {
                    SetValueWithoutNotify(newValue);
                }
                finally
                {
                    _suppressDisplaySync = false;
                }

                if (previous != newValue)
                {
                    using (var evt = ChangeEvent<long>.GetPooled(previous, newValue))
                    {
                        evt.target = this;
                        SendEvent(evt);
                    }
                }
            }

            private static string FindPresetName(long configValue)
            {
                foreach (var kvp in ChannelConfigNames)
                {
                    if (kvp.Value == configValue)
                    {
                        return kvp.Key;
                    }
                }
                return null;
            }

            private void ShowDropdown()
            {
                _dropdown.style.display = DisplayStyle.Flex;
                _customField.style.display = DisplayStyle.None;
                _returnBtn.style.display = DisplayStyle.None;
            }

            private void ShowCustom()
            {
                _dropdown.style.display = DisplayStyle.None;
                _customField.style.display = DisplayStyle.Flex;
                _returnBtn.style.display = DisplayStyle.Flex;
            }
        }

        // Option with a defined list of possible values. The dropdown shows the
        // DisplayNames, but the underlying value is what is loaded/saved
        private class AllowedValuesDropdownField<T> : BaseField<T>
        {
            private readonly DropdownField _dropdown;
            private readonly List<string> _displayNames;
            private readonly List<T> _values;

            public AllowedValuesDropdownField(string label, List<AkOptionAllowedValue> allowedValues, Func<object, T> convert)
                : this(label, new DropdownField { style = { flexGrow = 1, minWidth = 0 } }, allowedValues, convert)
            {
            }

            private AllowedValuesDropdownField(string label, DropdownField dropdown, List<AkOptionAllowedValue> allowedValues, Func<object, T> convert)
                : base(label, dropdown)
            {
                _dropdown = dropdown;
                _values = allowedValues.Select(v => convert(v.Value)).ToList();
                _displayNames = allowedValues.Select(v => v.DisplayName ?? Convert.ToString(v.Value)).ToList();
                _dropdown.choices = _displayNames;

                _dropdown.RegisterValueChangedCallback(evt =>
                {
                    // Reject anything outside the allowed values.
                    var index = _displayNames.IndexOf(evt.newValue);
                    if (index < 0)
                    {
                        _dropdown.SetValueWithoutNotify(evt.previousValue);
                        return;
                    }

                    value = _values[index];
                });
            }

            public override void SetValueWithoutNotify(T newValue)
            {
                base.SetValueWithoutNotify(newValue);
                // Values outside the allowed list (e.g. from a hand-edited JSON) are shown as-is and flagged,
                // so the stored value isn't misrepresented.
                var index = _values.IndexOf(newValue);
                if (index >= 0)
                {
                    _dropdown.SetValueWithoutNotify(_displayNames[index]);
                    _dropdown.tooltip = null;
                }
                else
                {
                    var valueStr = Convert.ToString(newValue, CultureInfo.InvariantCulture);
                    _dropdown.SetValueWithoutNotify($"Invalid ({valueStr})");
                    _dropdown.tooltip = $"{valueStr} is not a valid option. Select a value from the list.";
                }
            }
        }

        // UI Toolkit has no builtin clamping, so we need a generic clamping function.
        private static T ClampToRange<T>(T value, T? min, T? max) where T : struct, IComparable<T>
        {
            if (min.HasValue && value.CompareTo(min.Value) < 0)
            {
                value = min.Value;
            }

            if (max.HasValue && value.CompareTo(max.Value) > 0)
            {
                value = max.Value;
            }

            return value;
        }

        // UI Toolkit has no builtin clamping. Cancel changes that are out of range, then re-dispatch the clamped value.
        private static void SetRangeValidation<T>(BaseField<T> field, T? min, T? max)
            where T : struct, IComparable<T>
        {
            if (min == null && max == null)
            {
                return;
            }

            field.RegisterCallback<ChangeEvent<T>>(evt =>
            {
                if (evt.target != field)
                {
                    return;
                }

                var clamped = ClampToRange(evt.newValue, min, max);
                if (clamped.CompareTo(evt.newValue) != 0)
                {
                    evt.StopImmediatePropagation();
                    field.value = clamped;
                }
            }, TrickleDown.TrickleDown);
        }
    }
}
