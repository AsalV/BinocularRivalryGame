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
using AK.Wwise.Unity.Logging;
using UnityEditor;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings.Editor
{
    internal static class AkOptionMigration
    {
        public static void TryMigrateIfNeeded()
        {
            // Discover the platform-specific settings assets directly on disk, scoped to the canonical
            // ScriptableObjects folder. So we can ignore the global/common ScriptableObject instance
            var scriptableObjectFolder = AkWwiseEditorSettings.WwiseScriptableObjectRelativePath.Replace('\\', '/');
            if (!AssetDatabase.IsValidFolder(scriptableObjectFolder))
            {
                return;
            }

            var guids = AssetDatabase.FindAssets("t:AkCommonPlatformSettings", new[] { scriptableObjectFolder });
            if (guids.Length == 0)
            {
                return;
            }

            WwiseLogger.LogFormat(LogLevel.Log,
                "AkOptionMigration: found {0} legacy AkWwiseInitializationSettings asset(s) in <{1}>, starting migration.",
                guids.Length, scriptableObjectFolder);

            var migrators = new Dictionary<string, IPlatformMigrator>();
            foreach (var type in TypeCache.GetTypesDerivedFrom<IPlatformMigrator>())
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }
                var m = (IPlatformMigrator)Activator.CreateInstance(type);
                if (migrators.ContainsKey(m.SettingsName))
                {
                    // FIXME Need to handle custom platforms
                    WwiseLogger.LogFormat(LogLevel.Warning, "AkOptionMigration: duplicate migrator for settings <{0}>; keeping the first.", m.SettingsName);
                    continue;
                }
                migrators[m.SettingsName] = m;
            }

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                // Resolve the migrator by file name before loading the asset.
                var settingsName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (!migrators.TryGetValue(settingsName, out var migrator))
                {
                    // No migrator: the deprecated container (delete) or a custom/not-installed platform (leave).
#pragma warning disable CS0618 // Type or member is obsolete, we need this for migration
                    if (settingsName == nameof(AkWwiseInitializationSettings)
                        && AssetDatabase.LoadAssetAtPath<AkWwiseInitializationSettings>(path) != null)
#pragma warning restore CS0618 // Type or member is obsolete
                    {
                        WwiseLogger.LogFormat(LogLevel.Verbose, "AkOptionMigration: removing deprecated container <{0}>.", path);
                        AssetDatabase.DeleteAsset(path);
                    }
                    else
                    {
                        WwiseLogger.LogFormat(LogLevel.Verbose, "AkOptionMigration: no migrator for <{0}>; leaving <{1}> in place.", settingsName, path);
                    }
                    continue;
                }

#pragma warning disable CS0618 // Type or member is obsolete, we need this for migration
                var platformSettings = AssetDatabase.LoadAssetAtPath<AkWwiseInitializationSettings.PlatformSettings>(path);
#pragma warning restore CS0618 // Type or member is obsolete
                if (platformSettings == null)
                {
                    WwiseLogger.LogFormat(LogLevel.Verbose, "AkOptionMigration: could not load <{0}> as platform settings; leaving in place.", path);
                    continue;
                }

                WwiseLogger.LogFormat(LogLevel.Log, "AkOptionMigration: migrating platform <{0}>...", settingsName);

                try
                {
                    // Effective pre-migration defaults: SDK defaults overlaid with Unity-integration overrides.
                    // Migrators only emit entries that differ from this baseline.
                    var defaults = AkOptionStore.LoadEffectiveDefaults(migrator.PlatformId);

                    // Defaults could not be read, retry on a later run.
                    if (defaults.Count == 0)
                    {
                        WwiseLogger.LogFormat(LogLevel.Warning,
                            "AkOptionMigration: could not load option defaults for platform <{0}>; leaving <{1}> in place and skipping migration. Make sure the Newtonsoft package is installed; migration will be retried on next editor launch.",
                            settingsName, path);
                        continue;
                    }

                    var akOptionOverrides = MigratePlatformSettings(platformSettings, migrator, defaults);
                    if (akOptionOverrides.Count > 0)
                    {
                        AkOptionStore.WriteProjectOverrides(migrator.PlatformId, akOptionOverrides);
                    }

                    // Migrate runtime settings
                    var runtimeDefaults = WwiseRuntimeSettingsStore.LoadDefaults(migrator.PlatformId);
                    var wwiseRuntimeOverrides = WwiseRuntimeSettingsMigration.Migrate(platformSettings, runtimeDefaults);
                    if (wwiseRuntimeOverrides.Count > 0)
                    {
                        WwiseRuntimeSettingsStore.WriteProjectOverrides(migrator.PlatformId, wwiseRuntimeOverrides);
                    }

                    LogMigrationSummary(migrator.PlatformId, akOptionOverrides, wwiseRuntimeOverrides);

                    // Migration complete, we can remove the asset
                    AssetDatabase.DeleteAsset(path);
                }
                catch (Exception e)
                {
                    WwiseLogger.LogFormat(LogLevel.Error,
                        "AkOptionMigration: failed to migrate platform <{0}>; leaving <{1}> in place.\n{2}",
                        settingsName, path, e);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static readonly string[] UnmigratedSettings =
        {
            "AkCommonCommSettings.m_NotificationPort",
            "AkCommonCommSettings.m_PoolSize",
            "AkCommonAdvancedSettings.m_ContinuousPlaybackLookAhead",
            "SpatialAudioSettings.m_MaxGlobalReflectionPaths",
            "PlatformAdvancedSettings.UseHardwareCodecLowLatencyMode"
        };

        private static void AppendSummaryEntries(List<string> entries, List<AkOptionNamespace> overrides)
        {
            foreach (var (namespaceName, optionName, option) in overrides.AllOptionsWithNames())
            {
                entries.Add($"{namespaceName}.{optionName} = <{option.DefaultValue}>");
            }
        }

        private static void LogMigrationSummary(
            string platformId,
            List<AkOptionNamespace> akOptionOverrides,
            List<AkOptionNamespace> wwiseRuntimeOverrides)
        {
            var loggedEntries = new List<string>();
            AppendSummaryEntries(loggedEntries, akOptionOverrides);
            AppendSummaryEntries(loggedEntries, wwiseRuntimeOverrides);

            if (loggedEntries.Count == 0)
            {
                WwiseLogger.LogFormat(LogLevel.Log,
                    "AkOptionMigration: migrated platform <{0}>, no values to migrate.",
                    platformId);
            }
            else
            {
                WwiseLogger.LogFormat(LogLevel.Log,
                    "AkOptionMigration: migrated platform <{0}>, {1} options changed: {2}.",
                    platformId, loggedEntries.Count, string.Join(", ", loggedEntries));
            }

            WwiseLogger.LogFormat(LogLevel.Log,
                "AkOptionMigration: the following settings are no longer migrated for platform <{0}> and now use the Wwise SDK defaults (or Unity-specific overrides).\n{1}",
                platformId, string.Join(", ", UnmigratedSettings));
        }

        // Migrates a single platform's settings against the given defaults baseline.
        public static List<AkOptionNamespace> MigratePlatformSettings(
            AkCommonPlatformSettings platform,
            IPlatformMigrator platformMigrator,
            List<AkOptionNamespace> defaults)
        {
#pragma warning disable CS0618 // Type or member is obsolete, we need to access it for migration
            var user = platform.GetUserSettings();
            var advanced = platform.GetAdvancedSettings();
            var comm = platform.GetCommsSettings();
#pragma warning restore CS0618 // Type or member is obsolete, we need to access it for migration

            var userOverrides = AkCommonUserSettingsMigrator.Migrate(user, defaults);
            var platformUserOverrides = platformMigrator?.Migrate(user, userOverrides, defaults);

            var advancedOverrides = AkCommonAdvancedSettingsMigrator.Migrate(advanced, defaults);
            var platformAdvancedOverrides = platformMigrator?.Migrate(advanced, advancedOverrides, defaults);

            var commOverrides = AkCommonCommSettingsMigrator.Migrate(comm, defaults);
            var platformCommOverrides = platformMigrator?.Migrate(comm, commOverrides, defaults);

            var settingsToOverride = userOverrides.Merge(advancedOverrides, commOverrides,
                platformUserOverrides, platformAdvancedOverrides, platformCommOverrides);

            StripHiddenOverrides(settingsToOverride, defaults);

            return settingsToOverride;
        }

        /// <summary>
        /// Hidden options should not be migrated, since they will be invisible to the user.
        /// AppendOptionOverride does not copy Visibility onto emitted overrides, so the lookup goes
        /// through the defaults map.
        /// </summary>
        private static void StripHiddenOverrides(
            List<AkOptionNamespace> settingsToOverride,
            List<AkOptionNamespace> defaults)
        {
            var defaultsByName = defaults.ByName();
            foreach (var ns in settingsToOverride)
            {
                if (!defaultsByName.TryGetValue(ns.Name, out var defaultNs))
                {
                    continue;
                }

                if (defaultNs.Visibility == AkOptionVisibility.Hidden)
                {
                    ns.Options.Clear();
                    continue;
                }

                var defaultOptsByName = defaultNs.OptionsByName();
                ns.Options.RemoveAll(opt =>
                {
                    if (defaultOptsByName.TryGetValue(opt.Name, out var d)
                        && d.Visibility == AkOptionVisibility.Hidden)
                    {
                        return true;
                    }
                    return false;
                });
            }

            settingsToOverride.RemoveAll(ns => ns.Options.Count == 0);
        }
    }
}
