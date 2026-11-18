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

using System.Collections.Generic;
using System.IO;
using AK.Wwise.Unity.Settings;
using AK.Wwise.Unity.Logging;
using UnityEngine;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings
{
    public static class WwiseRuntimeSettingsStore
    {
        private static IAkOptionParser AkOptionParser => AkOptionStore.AkOptionParser;

        /// <summary>
        /// Parse AkOption JSON and return the parsed namespaces. Returns an
        /// empty list (logging the failure) if parsing throws.
        /// </summary>
        private static List<AkOptionNamespace> ParseOrEmpty(IAkOptionParser parser, string json, string source)
        {
            try
            {
                
                return parser.Parse(json);
            }
            catch (AkOptionParseException e)
            {
                WwiseLogger.LogFormat(LogLevel.Error, "Could not parse Settings.Runtime.json in <{0}>. {1}", source, e.Message);
                return new List<AkOptionNamespace>();
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Load only the (read-only) default runtime settings shipped with the Wwise Unity integration (common + platform overrides).
        /// </summary>
        /// <param name="platformId"></param>
        public static List<AkOptionNamespace> LoadDefaults(string platformId)
        {
            var pathGetter = new AkOptionPathGetter(platformId);

            var commonPath = pathGetter.RuntimeSettingsCommonPath;
            if (!File.Exists(commonPath))
            {
                WwiseLogger.LogFormat(LogLevel.Error, "Missing Wwise runtime settings in <{0}>. No settings can be loaded.", commonPath);
                return new List<AkOptionNamespace>();
            }

            var result = ParseOrEmpty(AkOptionParser, File.ReadAllText(commonPath), commonPath);

            var platformPath = pathGetter.RuntimeSettingsPlatformPath;
            if (platformPath != null && File.Exists(platformPath))
            {
                var platformDefaults = ParseOrEmpty(AkOptionParser, File.ReadAllText(platformPath), platformPath);
                result = result.Merge(platformDefaults);
            }
            return result;
        }

        /// <summary>
        /// Load only the user-edited project overrides.
        /// </summary>
        /// <param name="platformId"></param>
        public static List<AkOptionNamespace> LoadProjectOverrides(string platformId)
        {
            var projectPath = new AkOptionPathGetter(platformId).ProjectRuntimeSettingsPlatformOverridePath;
            if (!File.Exists(projectPath))
            {
                WwiseLogger.LogFormat(LogLevel.Verbose, "No Wwise runtime settings override at {0}. Using default settings.", projectPath);
                return new List<AkOptionNamespace>();
            }

            return ParseOrEmpty(AkOptionParser, File.ReadAllText(projectPath), projectPath);
        }

        /// <summary>
        /// Load the shipped defaults combined with the user's project overrides. This is equivalent to the values that will be in effect at runtime, with metadata from the defaults.
        /// </summary>
        /// <param name="platformId">The string name of the platform, see <see cref="AkOptionPathGetter.RegisteredPlatforms"/></param>
        public static List<AkOptionNamespace> LoadEffectiveSettings(string platformId)
        {
            var defaults = LoadDefaults(platformId);
            var projectOverrides = LoadProjectOverrides(platformId);
            return defaults.Merge(projectOverrides);
        }

        /// <summary>
        /// Write the project overrides to disk. If <paramref name="overrides"/> is empty,
        /// any existing override file is deleted rather than replaced with an empty placeholder.
        /// </summary>
        public static void WriteProjectOverrides(string platformId, List<AkOptionNamespace> overrides)
        {
            var outputPath = new AkOptionPathGetter(platformId).ProjectRuntimeSettingsPlatformOverridePath;
            var dir = Path.GetDirectoryName(outputPath);
            if (dir == null)
            {
                WwiseLogger.Error("Could not find or create Wwise runtime settings directory.");
                return;
            }

            if (overrides.Count == 0)
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
            }
            else
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var json = AkOptionParser.ToJson(overrides, new AkOptionParserConfig { SkipDoc = true, SkipKeys = true, SkipType = false });
                if (!File.Exists(outputPath) || !AkOptionParser.AreEquivalent(json, File.ReadAllText(outputPath)))
                {
                    File.WriteAllText(outputPath, json);
                }
            }

            ReloadIfHostPlatform(platformId);
        }

        private static void ReloadIfHostPlatform(string platformId)
        {
            if (platformId == AkOptionPathGetter.EditorHostPlatformId)
            {
                WwiseRuntimeSettings.Reload(platformId);
            }
        }

        /// <summary>
        /// Author a single project override and persist it: applies the change on top of the
        /// current overrides, relative to the shipped defaults, and writes the result to disk.
        /// </summary>
        public static void WriteProjectOverride(string platformId, string namespaceName, string optionName, object value)
        {
            var defaults = LoadDefaults(platformId);
            var overrides = LoadProjectOverrides(platformId);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, namespaceName, optionName, value);
            WriteProjectOverrides(platformId, overrides);
        }

        /// <summary>
        /// Write the settings to be used at runtime. Only overriden settings (Unity + user overrides) will be written. Used both in editor, and at runtime.
        /// </summary>
        /// <param name="platformId">The string name of the platform, see <see cref="AkOptionPathGetter.RegisteredPlatforms"/></param>
        public static void WriteRuntimeSettings(string platformId)
        {
            var finalSettings = LoadEffectiveSettings(platformId);

            var json = AkOptionParser.ToJson(finalSettings, new AkOptionParserConfig { SkipKeys = true });

            var resourcePath = AkOptionPathGetter.RuntimeSettingsResourcesAbsolutePath;
            var dir = Path.GetDirectoryName(resourcePath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(resourcePath) && AkOptionParser.AreEquivalent(json, File.ReadAllText(resourcePath)))
            {
                return;
            }
            File.WriteAllText(resourcePath, json);
            UnityEditor.AssetDatabase.ImportAsset(AkOptionPathGetter.RuntimeSettingsResourcesAssetPath);
        }

#endif // UNITY_EDITOR

        /// <summary>
        /// Load the effective runtime settings: the shipped defaults merged with the user's
        /// project overrides. Used so Play Mode and built runtime read the same set of values.
        /// </summary>
        public static List<AkOptionNamespace> LoadRuntimeSettings(string platformId)
        {
#if UNITY_EDITOR
            // We write the runtime settings every time in editor so that the flow for editor/runtime (load from Resources) is as similar as possible.
            WriteRuntimeSettings(platformId);
#endif
            var asset = Resources.Load<TextAsset>(AkOptionPathGetter.RuntimeSettingsResourcesPath);
            if (asset == null)
            {
                WwiseLogger.LogFormat(LogLevel.Warning, "Wwise runtime settings resource <{0}> not found. The integration will use default settings.", AkOptionPathGetter.RuntimeSettingsResourcesPath);
                return new List<AkOptionNamespace>();
            }
            return ParseOrEmpty(AkOptionParser, asset.text, AkOptionPathGetter.RuntimeSettingsResourcesPath);
        }
    }
}
