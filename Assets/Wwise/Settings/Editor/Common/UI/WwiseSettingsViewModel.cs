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
using AK.Wwise.Unity.Logging;
using Wwise.SDK;
using Visibility = Wwise.SDK.AkOptionVisibility;

namespace AK.Wwise.Unity.Settings.Editor.Common.UI
{
    // Each platform's editable state is held for the lifetime of the open view.
    // Overrides is mutated in-place when a cell changes, then persisted.
    internal class PlatformState
    {
        public string IntegrationPlatformId;
        public string WwiseSDKPlatformName;
        public string DisplayName;
        // SDK + Unity-integration overrides
        public List<AkOptionNamespace> EffectiveDefaults;
        // project overrides, mutated by cell edits
        public List<AkOptionNamespace> Overrides;

        private List<AkOptionNamespace> _merged;

        public List<AkOptionNamespace> Merged => _merged ??= EffectiveDefaults.Merge(Overrides);

        public void InvalidateMerged()
        {
            _merged = null;
        }

        // True if the given name is one of this platform's names: its Integration ID or its SDK name.
        public bool MatchesPlatform(string name)
        {
            return string.Equals(IntegrationPlatformId, name, StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrEmpty(WwiseSDKPlatformName)
                    && string.Equals(WwiseSDKPlatformName, name, StringComparison.OrdinalIgnoreCase));
        }
    }

    // View's state for an option
    internal class OptionInfo
    {
        public string Name;
        public string DisplayName;
        public bool IsAdvanced;
        public string Doc;
        public List<string> SupportingPlatformIds = new();
        public List<string> OverridingPlatformIds = new();
        public bool HasAnyOverride => OverridingPlatformIds.Count > 0;
    }

    // A group of related platforms sharing a set of options.
    internal class PlatformSpecificGroup
    {
        public List<string> SupportSet;
        public List<OptionInfo> Options = new();
    }

    // View's state for a namespace
    internal class NamespaceEntry
    {
        public string NamespaceName;
        public string DisplayName;
        public string PlatformDisplayName;
        public bool IsAdvanced;
        public List<OptionInfo> CrossPlatform = new List<OptionInfo>
        {
            Capacity = 0
        };
        public List<PlatformSpecificGroup> PlatformSpecificGroups = new List<PlatformSpecificGroup>();

        // Every option in this namespace
        public IEnumerable<OptionInfo> AllOptions =>
            CrossPlatform.Concat(PlatformSpecificGroups.SelectMany(g => g.Options));

        // Number of options in this namespace that have at least one project-level override on some platform.
        public int ModifiedOptionCount => AllOptions.Count(o => o.HasAnyOverride);
    }

    // Identifies one row: an option within a namespace.
    internal readonly struct OptionKey : IEquatable<OptionKey>
    {
        public readonly string Namespace;
        public readonly string Option;

        public OptionKey(string @namespace, string option)
        {
            Namespace = @namespace;
            Option = option;
        }

        public bool Equals(OptionKey other) => Namespace == other.Namespace && Option == other.Option;
        public override bool Equals(object obj) => obj is OptionKey other && Equals(other);
        public override int GetHashCode() => (Namespace, Option).GetHashCode();
        public override string ToString() => $"{Namespace}.{Option}";
    }

    /// <summary>
    /// Model for the settings view: loads data for the Wwise Settings,
    /// and handles interactions between the data store and the view.
    /// (This is a presentation model, not strictly MVVM.)
    /// </summary>
    internal class WwiseSettingsViewModel
    {
        private readonly IAkOptionDataSource _source;
        private readonly string _displayName;
        private readonly List<PlatformState> _platforms;
        private readonly List<NamespaceEntry> _namespaces;
        private readonly Dictionary<string, PlatformState> _platformsById;
        private readonly Dictionary<OptionKey, OptionInfo> _optionsByKey = new Dictionary<OptionKey, OptionInfo>();
        private readonly Dictionary<OptionKey, string> _searchIndex = new Dictionary<OptionKey, string>();

        /// <summary>Raised for each (namespace, option) whose value or override state changed.</summary>
        public event Action<OptionKey, IReadOnlyCollection<string>> OptionChanged;

        /// <param name="source">The datasource (e.g. Runtime Settings)</param>
        /// <param name="platformIds">Registered platforms to load. Passed in rather than read from
        /// <see cref="AkOptionPathGetter.RegisteredPlatformIds"/> so tests can supply a fixed set.</param>
        /// <param name="editorHostPlatformId">Hoisted to the front of <see cref="Platforms"/>, so the
        /// platform the Editor is running on is the leftmost column. May be null.</param>
        /// <param name="displayName">Page label, used in log messages.</param>
        public WwiseSettingsViewModel(IAkOptionDataSource source, IEnumerable<string> platformIds,
                                      string editorHostPlatformId, string displayName)
        {
            _source = source;
            _displayName = displayName;

            _platforms = platformIds
                .Select(id => LoadPlatform(id, source))
                .OrderBy(p => p.IntegrationPlatformId, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (editorHostPlatformId != null)
            {
                var idx = _platforms.FindIndex(p => p.IntegrationPlatformId == editorHostPlatformId);
                if (idx > 0)
                {
                    var hoist = _platforms[idx];
                    _platforms.RemoveAt(idx);
                    _platforms.Insert(0, hoist);
                }
            }

            _platformsById = _platforms.ToDictionary(p => p.IntegrationPlatformId);
            _namespaces = BuildOptionUnion(_platforms);

            foreach (var entry in _namespaces)
            {
                foreach (var info in entry.AllOptions)
                {
                    var key = new OptionKey(entry.NamespaceName, info.Name);
                    _optionsByKey[key] = info;
                    _searchIndex[key] =
                        $"{entry.NamespaceName}\n{entry.DisplayName}\n{info.Name}\n{info.DisplayName}";
                }
            }
        }

        public IReadOnlyList<PlatformState> Platforms => _platforms;
        public IReadOnlyList<NamespaceEntry> Namespaces => _namespaces;

        public PlatformState GetPlatform(string platformId)
        {
            return _platformsById.TryGetValue(platformId, out var p) ? p : null;
        }

        /// <summary>The entry a cell renders: type, range, allowed values and current value.</summary>
        public AkOptionEntry GetEntry(string platformId, string namespaceName, string optionName)
        {
            var p = GetPlatform(platformId);
            var @namespace = p?.Merged.FirstOrDefault(n => n.Name == namespaceName);
            return @namespace?.Options.FirstOrDefault(o => o.Name == optionName);
        }

        /// <summary>The value a cell currently shows.</summary>
        public object GetValue(string platformId, string namespaceName, string optionName)
        {
            var p = GetPlatform(platformId);
            if (p == null)
            {
                return null;
            }
            var entry = FindOption(p.Overrides, namespaceName, optionName)
                     ?? FindOption(p.EffectiveDefaults, namespaceName, optionName);
            return entry?.DefaultValue;
        }

        /// <summary>The shipped/integration default, ignoring any project override.</summary>
        public AkOptionEntry GetDefaultEntry(string platformId, string namespaceName, string optionName)
        {
            var p = GetPlatform(platformId);
            return p == null ? null : FindOption(p.EffectiveDefaults, namespaceName, optionName);
        }

        public bool IsOverridden(string platformId, string namespaceName, string optionName)
        {
            var p = GetPlatform(platformId);
            return p != null && IsOverridden(p.Overrides, namespaceName, optionName);
        }

        /// <summary>True if any visible option in this namespace is overridden on some platform.</summary>
        public bool NamespaceHasAnyOverride(string namespaceName)
        {
            var entry = _namespaces.FirstOrDefault(n => n.NamespaceName == namespaceName);
            return entry != null && entry.ModifiedOptionCount > 0;
        }

        /// <summary>True if any of the given namespaces has a visible override on any platform.</summary>
        public bool AnyOverrideIn(IEnumerable<NamespaceEntry> entries)
        {
            return entries.Any(e => e.ModifiedOptionCount > 0);
        }

        public int TotalOverrideCount => _platforms.Sum(p => p.Overrides.Sum(n => n.Options.Count));

        /// <summary>
        /// Overrides whose option no longer exists in the effective defaults, i.e. removed or
        /// renamed by an SDK update.
        /// </summary>
        public int OrphanedOverrideCount =>
            _platforms.Sum(p => p.Overrides.AllOptionsWithNames()
                                 .Count(o => FindOption(p.EffectiveDefaults, o.namespaceName, o.optionName) == null));

        private static AkOptionEntry FindOption(List<AkOptionNamespace> namespaces, string namespaceName, string optionName)
        {
            return namespaces.FirstOrDefault(n => n.Name == namespaceName)?
                             .Options.FirstOrDefault(o => o.Name == optionName);
        }

        internal static bool IsOverridden(List<AkOptionNamespace> overrides, string namespaceName, string optionName)
        {
            return overrides.Any(n => n.Name == namespaceName && n.Options.Any(o => o.Name == optionName));
        }

        private string _searchText = string.Empty;
        private bool _showAdvanced;
        private readonly HashSet<string> _hiddenPlatformIds = new HashSet<string>();

        /// <summary>Raised when filter state changes, so a view can re-apply visibility.</summary>
        public event Action FiltersChanged;

        public string SearchText
        {
            get => _searchText;
            set
            {
                var incoming = value ?? string.Empty;
                if (_searchText == incoming)
                {
                    return;
                }
                _searchText = incoming;
                FiltersChanged?.Invoke();
            }
        }

        public bool ShowAdvanced
        {
            get => _showAdvanced;
            set
            {
                if (_showAdvanced == value)
                {
                    return;
                }
                _showAdvanced = value;
                FiltersChanged?.Invoke();
            }
        }

        /// <summary>
        /// True while searching, so a view can tell a namespace expansion the
        /// user chose from one the search forced.
        /// </summary>
        public bool HasActiveSearch => _searchText.Trim().Length > 0;

        public IReadOnlyCollection<string> HiddenPlatformIds => _hiddenPlatformIds;

        public void SetHiddenPlatforms(IEnumerable<string> platformIds)
        {
            var incoming = new HashSet<string>(platformIds ?? Enumerable.Empty<string>());
            if (incoming.SetEquals(_hiddenPlatformIds))
            {
                return;
            }
            _hiddenPlatformIds.Clear();
            _hiddenPlatformIds.UnionWith(incoming);
            FiltersChanged?.Invoke();
        }

        public void SetPlatformVisible(string platformId, bool visible)
        {
            var hidden = new HashSet<string>(_hiddenPlatformIds);
            if (visible)
            {
                hidden.Remove(platformId);
            }
            else
            {
                hidden.Add(platformId);
            }
            SetHiddenPlatforms(hidden);
        }

        public void SetOnlyPlatformVisible(string platformId)
        {
            SetHiddenPlatforms(_platforms
                .Select(p => p.IntegrationPlatformId)
                .Where(id => id != platformId));
        }

        public void SetAllPlatformsVisible(bool visible)
        {
            SetHiddenPlatforms(visible
                ? Enumerable.Empty<string>()
                : _platforms.Select(p => p.IntegrationPlatformId));
        }

        public bool IsColumnVisible(string platformId)
        {
            return !_hiddenPlatformIds.Contains(platformId);
        }

        /// <summary>True if the option matches the current query, or no query is set.</summary>
        public bool MatchesSearch(string namespaceName, string optionName)
        {
            var query = _searchText.Trim();
            if (query.Length == 0)
            {
                return true;
            }
            return _searchIndex.TryGetValue(new OptionKey(namespaceName, optionName), out var haystack)
                && haystack.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public bool IsRowVisible(NamespaceEntry entry, OptionInfo option)
        {
            if (!_showAdvanced && (entry.IsAdvanced || option.IsAdvanced))
            {
                return false;
            }
            if (!option.SupportingPlatformIds.Any(IsColumnVisible))
            {
                return false;
            }
            return MatchesSearch(entry.NamespaceName, option.Name);
        }

        /// <summary>True if any option in this namespace passes the filters.</summary>
        public bool IsNamespaceVisible(NamespaceEntry entry)
        {
            return entry.AllOptions.Any(o => IsRowVisible(entry, o));
        }


        // Returned by a write that found nothing to do, so callers can read Count without a null check.
        private static readonly List<WwiseOptionChange> NoChanges = new List<WwiseOptionChange>();

        /// <summary>Set one platform's value for an option, persisting the result.</summary>
        /// <returns>The write that was applied, or an empty list if there was none.</returns>
        public List<WwiseOptionChange> SetOption(string platformId, string namespaceName, string optionName, object newValue)
        {
            var p = GetPlatform(platformId);
            if (p == null)
            {
                return NoChanges;
            }
            var previousValue = GetValue(platformId, namespaceName, optionName);
            WriteOption(p, namespaceName, optionName, newValue);
            RaiseChanged(namespaceName, optionName, new[] { platformId });

            // If no change, do not put on the undo stack.
            return Equals(previousValue, newValue)
                ? NoChanges
                : new List<WwiseOptionChange>
                {
                    new WwiseOptionChange(platformId, namespaceName, optionName, previousValue, newValue)
                };
        }

        /// <summary>Clear one platform's override, restoring the effective default.</summary>
        /// <returns>The write that was applied, or an empty list if there was no override.</returns>
        public List<WwiseOptionChange> RevertToDefault(string platformId, string namespaceName, string optionName)
        {
            var p = GetPlatform(platformId);
            var defaultEntry = p == null ? null : FindOption(p.EffectiveDefaults, namespaceName, optionName);
            if (defaultEntry == null)
            {
                return NoChanges;
            }

            var existingOverride = FindOption(p.Overrides, namespaceName, optionName);
            var previousValue = existingOverride?.DefaultValue;

            var defaultValue = defaultEntry.DefaultValue;
            WriteOption(p, namespaceName, optionName, defaultValue);
            RaiseChanged(namespaceName, optionName, new[] { platformId });

            WwiseLogger.LogFormat(LogLevel.VeryVerbose,
                "AkOption overrides: {0}.{1} reverted to default <{2}> (was <{3}>).",
                namespaceName, optionName, defaultValue, previousValue);

            // Reverting a cell that was already at its default writes nothing back, so there is
            // nothing to take back either.
            return existingOverride == null
                ? NoChanges
                : new List<WwiseOptionChange>
                {
                    new WwiseOptionChange(platformId, namespaceName, optionName, previousValue, defaultValue)
                };
        }

        /// <summary>
        /// Copy one platform's current value for an option onto the other platforms of a matrix.
        /// </summary>
        /// <returns>One entry per platform whose value actually changed.</returns>
        public List<WwiseOptionChange> ApplyValueAcrossRow(string sourcePlatformId, string namespaceName, string optionName,
                                                           IEnumerable<string> targetPlatformIds)
        {
            var value = GetValue(sourcePlatformId, namespaceName, optionName);
            if (value == null)
            {
                return NoChanges;
            }

            var changes = new List<WwiseOptionChange>();
            var written = new List<string>();
            foreach (var targetId in targetPlatformIds)
            {
                if (targetId == sourcePlatformId)
                {
                    continue;
                }
                var target = GetPlatform(targetId);
                if (target == null)
                {
                    continue;
                }
                // The option may not exist on this platform
                if (FindOption(target.EffectiveDefaults, namespaceName, optionName) == null)
                {
                    continue;
                }

                var previousValue = GetValue(targetId, namespaceName, optionName);
                WriteOption(target, namespaceName, optionName, value);
                written.Add(targetId);
                if (!Equals(previousValue, value))
                {
                    changes.Add(new WwiseOptionChange(targetId, namespaceName, optionName, previousValue, value));
                }
            }

            if (written.Count > 0)
            {
                RaiseChanged(namespaceName, optionName, written);
            }

            WwiseLogger.LogFormat(LogLevel.VeryVerbose,
                "AkOption overrides: {0}.{1} applied to {2} platform(s) from {3}.",
                namespaceName, optionName, written.Count, sourcePlatformId);

            return changes;
        }

        /// <summary>Clear every project override on every platform.</summary>
        /// <returns>One entry per override cleared.</returns>
        public List<WwiseOptionChange> ResetAllOverrides()
        {
            var totalOverrides = TotalOverrideCount;
            if (totalOverrides == 0)
            {
                return NoChanges;
            }

            // Collect what is about to be cleared so each affected row can be told which platforms
            // to refresh.
            var changes = new List<WwiseOptionChange>();
            var affected = new Dictionary<OptionKey, List<string>>();
            foreach (var p in _platforms)
            {
                if (p.Overrides.Count == 0)
                {
                    continue;
                }
                foreach (var (namespaceName, optionName, option) in p.Overrides.AllOptionsWithNames())
                {
                    var key = new OptionKey(namespaceName, optionName);
                    if (!affected.TryGetValue(key, out var ids))
                    {
                        ids = new List<string>();
                        affected[key] = ids;
                    }
                    ids.Add(p.IntegrationPlatformId);

                    // An override with no current default is orphaned
                    var defaultEntry = FindOption(p.EffectiveDefaults, namespaceName, optionName);
                    if (defaultEntry != null)
                    {
                        changes.Add(new WwiseOptionChange(p.IntegrationPlatformId, namespaceName, optionName,
                                                          option.DefaultValue, defaultEntry.DefaultValue));
                    }
                }

                p.Overrides.Clear();
                p.InvalidateMerged();
                _source.WriteProjectOverrides(p.IntegrationPlatformId, p.Overrides);
            }

            foreach (var info in _optionsByKey.Values)
            {
                info.OverridingPlatformIds.Clear();
            }

            WwiseLogger.LogFormat(LogLevel.VeryVerbose,
                "AkOption overrides: cleared all {0} override(s) for \"{1}\".", totalOverrides, _displayName);

            foreach (var kv in affected)
            {
                OptionChanged?.Invoke(kv.Key, kv.Value);
            }

            return changes;
        }

        // Mutate Overrides, persist, invalidate the merge cache, and keep the derived override
        // counts current. Every write should go through here.
        private void WriteOption(PlatformState p, string namespaceName, string optionName, object newValue)
        {
            // Drop any existing override; AppendOptionOverride re-adds only if newValue differs
            // from the effective default.
            var existingNamespace = p.Overrides.FirstOrDefault(n => n.Name == namespaceName);
            if (existingNamespace != null && existingNamespace.Options.Any(o => o.Name == optionName))
            {
                existingNamespace.Options.RemoveAll(o => o.Name == optionName);
                if (existingNamespace.Options.Count == 0)
                {
                    p.Overrides.Remove(existingNamespace);
                }
            }

            AkOptionExtensions.AppendOptionOverride(p.EffectiveDefaults, p.Overrides, namespaceName, optionName, newValue);
            _source.WriteProjectOverrides(p.IntegrationPlatformId, p.Overrides);
            p.InvalidateMerged();

            if (_optionsByKey.TryGetValue(new OptionKey(namespaceName, optionName), out var info))
            {
                var isOverridden = IsOverridden(p.Overrides, namespaceName, optionName);
                var platformId = p.IntegrationPlatformId;
                if (isOverridden && !info.OverridingPlatformIds.Contains(platformId))
                {
                    info.OverridingPlatformIds.Add(platformId);
                }
                else if (!isOverridden)
                {
                    info.OverridingPlatformIds.Remove(platformId);
                }
            }
        }

        private void RaiseChanged(string namespaceName, string optionName, IReadOnlyCollection<string> platformIds)
        {
            OptionChanged?.Invoke(new OptionKey(namespaceName, optionName), platformIds);
        }

        internal static PlatformState LoadPlatform(string platformId, IAkOptionDataSource source)
        {
            return new PlatformState
            {
                IntegrationPlatformId = platformId,
                WwiseSDKPlatformName = AkOptionPathGetter.GetPlatform(platformId).WwiseSDKName,
                DisplayName = AkOptionPathGetter.GetPlatform(platformId).DisplayName,
                EffectiveDefaults = source.LoadEffectiveDefaults(platformId),
                Overrides = source.LoadProjectOverrides(platformId),
            };
        }

        private static string ResolvePlatformDisplayName(string namespaceName, List<PlatformState> platforms)
        {
            string resolved = null;
            foreach (var platform in platforms)
            {
                if (!platform.MatchesPlatform(namespaceName))
                {
                    continue;
                }
                if (resolved != null)
                {
                    return null;
                }
                resolved = platform.DisplayName;
            }
            return resolved;
        }

        // Union all (namespace, option) pairs across platforms, preserving the first platform's
        // ordering.
        // Options are split into two buckets, cross-platform and platform-specific
        internal static List<NamespaceEntry> BuildOptionUnion(List<PlatformState> platforms)
        {
            var namespaceOrder = new List<string>();
            var optionsByNamespace = new Dictionary<string, List<string>>();
            var namespaceDisplayNames = new Dictionary<string, string>();
            var namespaceIsAdvanced = new Dictionary<string, bool>();
            var infos = new Dictionary<(string, string), OptionInfo>();

            foreach (var p in platforms)
            {
                foreach (var @namespace in p.Merged)
                {
                    if (@namespace.Options.Count == 0)
                    {
                        continue;
                    }
                    if (@namespace.Visibility == Visibility.Hidden)
                    {
                        continue;
                    }
                    if (!optionsByNamespace.TryGetValue(@namespace.Name, out var optList))
                    {
                        namespaceOrder.Add(@namespace.Name);
                        optList = new List<string>();
                        optionsByNamespace[@namespace.Name] = optList;
                        namespaceDisplayNames[@namespace.Name] = @namespace.DisplayName ?? @namespace.Name;
                        namespaceIsAdvanced[@namespace.Name] = @namespace.Visibility == Visibility.Advanced;
                    }
                    foreach (var option in @namespace.Options)
                    {
                        // Pointer options are ignored
                        if (option.Type == "Pointer" || option.Visibility == Visibility.Hidden)
                        {
                            continue;
                        }
                        var key = (@namespace.Name, option.Name);
                        if (!infos.TryGetValue(key, out var info))
                        {
                            info = new OptionInfo
                            {
                                Name = option.Name,
                                DisplayName = option.DisplayName ?? option.Name,
                                IsAdvanced = option.Visibility == Visibility.Advanced,
                                Doc = option.Doc,
                            };
                            infos[key] = info;
                            optList.Add(option.Name);
                        }
                        info.SupportingPlatformIds.Add(p.IntegrationPlatformId);
                        if (IsOverridden(p.Overrides, @namespace.Name, option.Name))
                        {
                            info.OverridingPlatformIds.Add(p.IntegrationPlatformId);
                        }
                    }
                }
            }

            var totalPlatforms = platforms.Count;
            var result = new List<NamespaceEntry>();
            foreach (var namespaceName in namespaceOrder)
            {
                var entry = new NamespaceEntry
                {
                    NamespaceName = namespaceName,
                    DisplayName = namespaceDisplayNames[namespaceName],
                    PlatformDisplayName = ResolvePlatformDisplayName(namespaceName, platforms),
                    IsAdvanced = namespaceIsAdvanced[namespaceName],
                };
                var groupOrder = new List<string>();
                var groupsByKey = new Dictionary<string, PlatformSpecificGroup>();

                foreach (var optionName in optionsByNamespace[namespaceName])
                {
                    var info = infos[(namespaceName, optionName)];
                    if (info.SupportingPlatformIds.Count == totalPlatforms)
                    {
                        entry.CrossPlatform.Add(info);
                    }
                    else
                    {
                        var key = string.Join("|", info.SupportingPlatformIds);
                        if (!groupsByKey.TryGetValue(key, out var grp))
                        {
                            grp = new PlatformSpecificGroup
                            {
                                SupportSet = new List<string>(info.SupportingPlatformIds),
                            };
                            groupsByKey[key] = grp;
                            groupOrder.Add(key);
                        }
                        grp.Options.Add(info);
                    }
                }
                foreach (var key in groupOrder)
                {
                    entry.PlatformSpecificGroups.Add(groupsByKey[key]);
                }
                result.Add(entry);
            }
            return result;
        }
    }
}
