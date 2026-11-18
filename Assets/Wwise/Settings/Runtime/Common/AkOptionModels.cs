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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AK.Wwise.Unity.Logging;

namespace Wwise.SDK
{
    /// <summary>
    /// Visibility of the Option. This is used as a suggestion to UIs as to what should be presented.
    /// This is only a visual effect, hidden options should still be read/written.
    /// </summary>
    public enum AkOptionVisibility
    {
        /// Visible
        Default,
        /// Hidden
        Hidden,
        /// Only shown in an "Advanced" page.
        Advanced
    }

    /// <summary>
    /// Describes a group of related options (e.g. "SoundEngine", "MemoryManager").
    /// </summary>
    public class AkOptionNamespace
    {
        public string Name = "";
        public string? DisplayName;
        public AkOptionVisibility? Visibility;
        public string? Doc;
        public string? ConditionalMacro;
        public int? KeyStart;
        public int? NumOptions;
        public List<AkOptionEntry> Options = new List<AkOptionEntry>();

        /// <summary>
        /// Retrieve the options as a dictionary.
        /// </summary>
        /// <returns>The options, with Option.Name -> Option </returns>
        public Dictionary<string, AkOptionEntry> OptionsByName()
        {
            return new Dictionary<string, AkOptionEntry>(@Options.Select(x =>
                new KeyValuePair<string, AkOptionEntry>(x.Name, x)));
        }
    }

    /// <summary>
    /// Describes a single option within a namespace.
    /// </summary>
    public class AkOptionEntry
    {
        public string Name = "";
        public string? DisplayName;
        public AkOptionVisibility? Visibility;
        public object? MinValue;
        public object? MaxValue;
        public string? Type;
        public int? MaxLength;
        public string? Doc;
        public object? DefaultValue;
        public List<AkOptionAllowedValue>? AllowedValues;
        public uint? Key;
    }

    public static class AkOptionExtensions
    {
        /// <summary>
        /// Retrieve the options for all namespaces.
        /// <param name="namespaces"> The namespaces to iterate.</param>
        /// </summary>
        public static IEnumerable<AkOptionEntry> AllOptions(this IEnumerable<AkOptionNamespace> namespaces)
        {
            return namespaces.SelectMany(@namespace => @namespace.Options);
        }

        /// <summary>
        /// Retrieve the options for all namespaces, with the namespace name attached.
        /// <param name="namespaces">The namespaces to iterate over.</param>
        /// <returns>A tuple with the combined namespace name, option name, and option </returns>
        /// </summary>
        public static IEnumerable<(string namespaceName, string optionName, AkOptionEntry option)>
            AllOptionsWithNames(this IEnumerable<AkOptionNamespace> namespaces)
        {
            foreach (var @namespace in namespaces)
            {
                foreach (var option in @namespace.Options)
                {
                    yield return (@namespace.Name, option.Name, option);
                }
            }
        }

        /// <summary>
        /// Index the namespaces by name.
        /// <param name="namespaces">The namespaces to index.</param>
        /// </summary>
        public static Dictionary<string, AkOptionNamespace> ByName(this IEnumerable<AkOptionNamespace> namespaces)
        {
            return namespaces.ToDictionary(n => n.Name);
        }

        /// <summary>
        /// Build a nested lookup keyed by namespace name, then option name.
        /// <param name="namespaces">The namespaces to index.</param>
        /// </summary>
        public static Dictionary<string, Dictionary<string, AkOptionEntry>> ToOptionMap(
            this IEnumerable<AkOptionNamespace> namespaces)
        {
            return namespaces.ToDictionary(n => n.Name, n => n.OptionsByName());
        }

        /// <summary>
        /// Merge multiple layers into a single list without modifying any input.
        /// Each layer can add new namespaces/options or override fields in existing ones.
        /// Later layers take precedence over earlier ones, except for Type and Key. In the case of
        /// a mismatched type or key, the merge is rejected.
        /// <paramref name="first"/> The first layer to merge
        /// <paramref name="rest"/> The layers to merge on top of <paramref name="first"/>
        /// </summary>
        public static List<AkOptionNamespace> Merge(
            this List<AkOptionNamespace> first,
            params List<AkOptionNamespace>[]? rest)
        {
            var result = new Dictionary<string, AkOptionNamespace>();
            if (rest == null)
            {
                return first;
            }

            foreach (var layer in rest.Prepend(first))
            {
                if (layer == null) continue;
                foreach (var @namespace in layer)
                {
                    if (!result.TryGetValue(@namespace.Name, out var merged))
                    {
                        merged = new AkOptionNamespace
                        {
                            Name = @namespace.Name,
                            Options = new List<AkOptionEntry>()
                        };
                        result[@namespace.Name] = merged;
                    }

                    merged.DisplayName = @namespace.DisplayName ?? merged.DisplayName;
                    merged.Visibility = @namespace.Visibility ?? merged.Visibility;
                    merged.Doc = @namespace.Doc ?? merged.Doc;
                    merged.ConditionalMacro = @namespace.ConditionalMacro ?? merged.ConditionalMacro;
                    merged.KeyStart = @namespace.KeyStart ?? merged.KeyStart;
                    merged.NumOptions = @namespace.NumOptions ?? merged.NumOptions;

                    var mergedOptionsByName = merged.Options.ToDictionary(o => o.Name);
                    foreach (var opt in @namespace.Options)
                    {
                        mergedOptionsByName.TryGetValue(opt.Name, out var existing);

                        if (existing?.Key is not null && opt.Key is not null && existing.Key != opt.Key)
                        {
                            WwiseLogger.LogFormat(LogLevel.Error,
                                "AkOption {0}.{1}: Key mismatch between merge layers (<{2}> vs <{3}>)." +
                                "Likely a stale override after a Wwise upgrade." +
                                "The override is ignored, keeping <{2}>.",
                                @namespace.Name, opt.Name, existing.Key, opt.Key);
                            continue;
                        }

                        if (existing?.Type is not null && opt.Type is not null && existing.Type != opt.Type)
                        {
                            WwiseLogger.LogFormat(LogLevel.Error,
                                "AkOption {0}.{1}: Type mismatch between merge layers (<{2}> vs <{3}>)." +
                                "Likely a stale override after a Wwise upgrade." +
                                "The override is ignored, keeping <{2}>.",
                                @namespace.Name, opt.Name, existing?.Type, opt.Type);
                            continue;
                        }

                        var newOpt = new AkOptionEntry
                        {
                            Name = opt.Name,
                            DisplayName = opt.DisplayName ?? existing?.DisplayName,
                            Visibility = opt.Visibility ?? existing?.Visibility,
                            MinValue = opt.MinValue ?? existing?.MinValue,
                            MaxValue = opt.MaxValue ?? existing?.MaxValue,
                            Type = opt.Type ?? existing?.Type,
                            MaxLength = opt.MaxLength ?? existing?.MaxLength,
                            Doc = opt.Doc ?? existing?.Doc,
                            DefaultValue = opt.DefaultValue ?? existing?.DefaultValue,
                            AllowedValues = opt.AllowedValues ?? existing?.AllowedValues,
                            Key = opt.Key ?? existing?.Key
                        };

                        if (existing != null)
                        {
                            merged.Options[merged.Options.IndexOf(existing)] = newOpt;
                        }
                        else
                        {
                            merged.Options.Add(newOpt);
                        }
                    }
                }
            }

            return result.Values.ToList();
        }

        /// <summary>Set (or override) a String option by key, returning a new list with the value applied on top.</summary>
        public static List<AkOptionNamespace> SetOption<TKey>(this List<AkOptionNamespace> options, string namespaceName, string optionName, TKey key, string value) where TKey : System.Enum
            => SetOptionRaw(options, namespaceName, optionName, key, "String", value);

        /// <summary>Set (or override) a Boolean option by key, returning a new list with the value applied on top.</summary>
        public static List<AkOptionNamespace> SetOption<TKey>(this List<AkOptionNamespace> options, string namespaceName, string optionName, TKey key, bool value) where TKey : System.Enum
            => SetOptionRaw(options, namespaceName, optionName, key, "Boolean", value);

        /// <summary>Set (or override) an Integer option by key, returning a new list with the value applied on top.</summary>
        public static List<AkOptionNamespace> SetOption<TKey>(this List<AkOptionNamespace> options, string namespaceName, string optionName, TKey key, long value) where TKey : System.Enum
            => SetOptionRaw(options, namespaceName, optionName, key, "Integer", value);

        /// <summary>Set (or override) an Integer option by key from an unsigned value, returning a new list with the value applied on top.</summary>
        public static List<AkOptionNamespace> SetOption<TKey>(this List<AkOptionNamespace> options, string namespaceName, string optionName, TKey key, ulong value) where TKey : System.Enum
            => SetOptionRaw(options, namespaceName, optionName, key, "Integer", value);

        /// <summary>Set (or override) a Real option by key, returning a new list with the value applied on top.</summary>
        public static List<AkOptionNamespace> SetOption<TKey>(this List<AkOptionNamespace> options, string namespaceName, string optionName, TKey key, double value) where TKey : System.Enum
            => SetOptionRaw(options, namespaceName, optionName, key, "Real", value);

        private static List<AkOptionNamespace> SetOptionRaw<TKey>(List<AkOptionNamespace> options, string namespaceName, string optionName, TKey key, string type, object value) where TKey : System.Enum
        {
            var layer = new List<AkOptionNamespace>
            {
                new()
                {
                    Name = namespaceName,
                    Options = new List<AkOptionEntry>
                    {
                        new() { Name = optionName, Key = Convert.ToUInt32(key), Type = type, DefaultValue = value }
                    }
                }
            };
            return options.Merge(layer);
        }

        /// <summary>
        /// Merge the keys from one map into another.
        /// </summary>
        /// <param name="srcOptions">The map whose values we wish to take, except the keys. Existing keys will be overriden.</param>
        /// <param name="srcKeys">The map which contains the corresponding keys for <paramref name="srcOptions"/></param>
        /// <returns> A new list of <see cref="AkOptionNamespace"/> with the options from <paramref name="srcOptions"/>  and the keys from  <paramref name="srcKeys"/>.
        /// </returns>
        public static List<AkOptionNamespace> MergeKeysAndTypes(List<AkOptionNamespace> srcOptions, List<AkOptionNamespace> srcKeys)
        {
            var srcKeysByName = srcKeys.ToDictionary(n => n.Name);
            var result = new List<AkOptionNamespace>();
            foreach (var srcNamespace in srcOptions)
            {
                if (!srcKeysByName.TryGetValue(srcNamespace.Name, out var keyNamespace))
                {
                    WwiseLogger.LogFormat(LogLevel.Warning, "Missing namespace {0} from options map with keys, no keys will be written.", srcNamespace.Name);
                    continue;
                }

                var mergedNamespace = new AkOptionNamespace
                {
                    Name = srcNamespace.Name,
                    DisplayName = srcNamespace.DisplayName,
                    Visibility = srcNamespace.Visibility,
                    Doc = srcNamespace.Doc,
                    ConditionalMacro = srcNamespace.ConditionalMacro,
                    KeyStart = srcNamespace.KeyStart,
                    NumOptions = srcNamespace.NumOptions,
                    Options = new List<AkOptionEntry>()
                };
                result.Add(mergedNamespace);

                var keyOptionsByName = keyNamespace.OptionsByName();
                foreach (var srcOption in srcNamespace.Options)
                {
                    if (!keyOptionsByName.TryGetValue(srcOption.Name, out var optionWithKeyType))
                    {
                        WwiseLogger.LogFormat(LogLevel.Warning, "Missing option <{0}> from namespace <{1}>, no key will be written.", srcOption.Name, srcNamespace.Name);
                        continue;
                    }

                    mergedNamespace.Options.Add(new AkOptionEntry
                    {
                        Name = srcOption.Name,
                        DisplayName = srcOption.DisplayName,
                        Visibility = srcOption.Visibility,
                        MinValue = srcOption.MinValue,
                        MaxValue = srcOption.MaxValue,
                        Type = optionWithKeyType.Type,
                        MaxLength = optionWithKeyType.MaxLength,
                        Doc = srcOption.Doc,
                        DefaultValue = srcOption.DefaultValue,
                        AllowedValues = srcOption.AllowedValues,
                        Key = optionWithKeyType.Key
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Normalize C# uint/ushort/int to long, and float must become double.
        /// </summary>
        private static object NormalizeToType(object value, object reference)
        {
            if (reference is long)
                return Convert.ToInt64(value);
            if (reference is double)
                return Convert.ToDouble(value);
            return value;
        }

        /// <summary>
        /// Write a new value for an option to  <paramref name="destNamespaces"/>, only if it has changed from the defaults in <paramref name="srcNamespaces"/>.
        /// </summary>
        /// <param name="srcNamespaces">The existing default options.</param>
        /// <param name="destNamespaces">The result: namespaces that contain only the new option overrides.</param>
        /// <param name="namespaceName">The name of the namespace in which to find the option.</param>
        /// <param name="optionName">The name of the overriden option.</param>
        /// <param name="newValue">The value to write. It will only be written if different from the existing option <remarks>We only write the new value and the old type; no keys or other fields will be written.</remarks>.</param>
        public static void AppendOptionOverride(List<AkOptionNamespace> srcNamespaces, List<AkOptionNamespace> destNamespaces, string namespaceName, string optionName, object newValue)
        {
            var defaultNamespace = srcNamespaces.FirstOrDefault(n => n.Name == namespaceName);
            if (defaultNamespace == null)
            {
                WwiseLogger.LogFormat(LogLevel.Verbose, "AkOption overrides: SDK defaults missing namespace {0}; skipping {0}.{1}.", namespaceName, optionName);
                return;
            }

            var defaultOption = defaultNamespace.Options.FirstOrDefault(o => o.Name == optionName);
            if (defaultOption == null)
            {
                WwiseLogger.LogFormat(LogLevel.Warning, "AkOption overrides: SDK defaults missing {0}.{1}; skipping override emission for this field.", namespaceName, optionName);
                return;
            }
            if (defaultOption.DefaultValue == null)
            {
                WwiseLogger.LogFormat(LogLevel.Warning, "AkOption overrides: SDK defaults missing {0}.{1} DefaultValue; skipping override emission for this field.", namespaceName, optionName);
                return;
            }

            var normalized = NormalizeToType(newValue, defaultOption.DefaultValue);
            if (Equals(normalized, defaultOption.DefaultValue))
                return;

            var destNamespace = destNamespaces.FirstOrDefault(n => n.Name == namespaceName);
            if (destNamespace == null)
            {
                destNamespace = new AkOptionNamespace { Name = namespaceName, Options = new List<AkOptionEntry>() };
                destNamespaces.Add(destNamespace);
            }

            destNamespace.Options.RemoveAll(o => o.Name == optionName);
            destNamespace.Options.Add(new AkOptionEntry
            {
                Name = optionName,
                Type = defaultOption.Type,
                DefaultValue = normalized
            });

            WwiseLogger.LogFormat(LogLevel.VeryVerbose, "AkOption overrides: {0}.{1} changed to <{2}> (Default <{3}>).", namespaceName, optionName, normalized, defaultOption.DefaultValue);
        }
    }

    /// <summary>
    /// Describes a permitted value for enum-style string options.
    /// </summary>
    public class AkOptionAllowedValue
    {
        public object? Value;
        public string? DisplayName;
        public string? Doc;
    }
}
