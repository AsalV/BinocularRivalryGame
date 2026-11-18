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
using AK.Wwise.Unity.Logging;
using UnityEngine;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings
{
    /// <summary>
    /// This class contains a collection of static methods to load and write AkOption files, in Editor, and at runtime.
    /// </summary>
    public static class AkOptionStore
    {
        private static IAkOptionParser _akOptionParser = null;

        public static IAkOptionParser AkOptionParser
        {
            get
            {
                if (_akOptionParser == null)
                {
#if AK_NEWTONSOFT_JSON
                    _akOptionParser = new NewtonsoftAkOptionsParser();
#else
                    WwiseLogger.Warning(
                        "The Newtonsoft Json package (com.unity.nuget.newtonsoft-json) is not installed, so Wwise options and runtime settings cannot be read. The integration will use built-in default settings. ");
                    _akOptionParser = new NullOptionParser();
#endif
                }
                return _akOptionParser;
            }
        }

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
                WwiseLogger.LogFormat(LogLevel.Error, "Could not parse Options.json in <{0}>. {1}", source, e.Message);
                return new List<AkOptionNamespace>();
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Load only the (read-only) Default options from the SDK (common + platform overrides)
        /// 
        /// </summary>
        /// <param name="platformId">The string name of the platform, see <see cref="AkOptionPathGetter.RegisteredPlatforms"/></param>
        public static List<AkOptionNamespace> LoadSDKDefaults(string platformId)
        {
            var pathGetter = new AkOptionPathGetter(platformId);

            var commonPath = pathGetter.SdkCommonPath;
            if (!File.Exists(commonPath))
            {
                WwiseLogger.LogFormat(LogLevel.Error, "Missing Wwise initialization options in <{0}>. No settings can be loaded.", commonPath);
                return new List<AkOptionNamespace>();
            }

            var result = ParseOrEmpty(AkOptionParser, File.ReadAllText(commonPath), commonPath);

            var sdkPlatformPath = pathGetter.SdkPlatformPath;
            if (sdkPlatformPath != null && File.Exists(sdkPlatformPath))
            {
                var platformDefaults = ParseOrEmpty(AkOptionParser, File.ReadAllText(sdkPlatformPath), sdkPlatformPath);
                result = result.Merge(platformDefaults);
            }
            return result;
        }

        /// <summary>
        /// Load the Unity-integration default overrides for a platform.
        /// These allow platform-specific settings on top of the SDK
        /// defaults but underneath the user's project overrides.
        /// </summary>
        /// <param name="platformId">The string name of the platform, see <see cref="AkOptionPathGetter.RegisteredPlatforms"/></param>
        public static List<AkOptionNamespace> LoadUnityOverrides(string platformId)
        {
            var pathGetter = new AkOptionPathGetter(platformId);

            var commonPath = pathGetter.UnityCommonOverridePath;
            var platformPath = pathGetter.UnityPlatformOverridePath;

            List<AkOptionNamespace> unityCommonMap;
            List<AkOptionNamespace> unityPlatformMap;

            if (commonPath == null || !File.Exists(commonPath))
            {
                unityCommonMap = new List<AkOptionNamespace>();
            }
            else
            {
                unityCommonMap = ParseOrEmpty(AkOptionParser, File.ReadAllText(commonPath), commonPath);
            }

            if (platformPath == null || !File.Exists(platformPath))
            {
                unityPlatformMap = new List<AkOptionNamespace>();
            }
            else
            {
                unityPlatformMap = ParseOrEmpty(AkOptionParser, File.ReadAllText(platformPath), platformPath);
            }

            return unityCommonMap.Merge(unityPlatformMap);
        }

        /// <summary>
        /// Load the defaults from the SDK combined with Unity-specific defaults. This is equivalent to the default values that will be in effect at runtime (i.e. if no user overrides exist), but with metadata from the SDK.
        /// </summary>
        /// <param name="platformId">The string name of the platform, see <see cref="AkOptionPathGetter.RegisteredPlatforms"/></param>
        /// <returns></returns>
        public static List<AkOptionNamespace> LoadEffectiveDefaults(string platformId)
        {
            var sdkDefaults = AkOptionStore.LoadSDKDefaults(platformId);
            var unityOverrides = AkOptionStore.LoadUnityOverrides(platformId);
            return sdkDefaults.Merge(unityOverrides);
        }

        /// <summary>
        /// Load only the user-edited project overrides.
        /// </summary>
        /// <param name="platformId">The string name of the platform, see <see cref="AkOptionPathGetter.RegisteredPlatforms"/></param>
        public static List<AkOptionNamespace> LoadProjectOverrides(string platformId)
        {
            var pathGetter = new AkOptionPathGetter(platformId);
            var projectPlatformPath = pathGetter.ProjectPlatformOverridePath;
            if (!File.Exists(projectPlatformPath))
            {
                WwiseLogger.LogFormat(LogLevel.Verbose, "No AkOptions Override for {0} at {1}. Using default init settings.", AkOptionPathGetter.GetPlatform(platformId)?.DisplayName ?? platformId, projectPlatformPath);
                return new List<AkOptionNamespace>();
            }

            return ParseOrEmpty(AkOptionParser, File.ReadAllText(projectPlatformPath), projectPlatformPath);
        }

        /// <summary>
        /// Write the platform overrides to disk. If <paramref name="overrides"/> is empty,
        /// any existing override file is deleted rather than replaced with an empty placeholder.
        /// </summary>
        public static void WriteProjectOverrides(string platformId, List<AkOptionNamespace> overrides)
        {
            var pathGetter = new AkOptionPathGetter(platformId);
            var outputPath = pathGetter.ProjectPlatformOverridePath;
            var dir = Path.GetDirectoryName(outputPath);
            if (dir == null)
            {
                WwiseLogger.Error("Could not find or create Wwise options settings directory.");
                return;
            }

            if (overrides.Count == 0)
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
                return;
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var json = AkOptionParser.ToJson(overrides, new AkOptionParserConfig { SkipDoc = true, SkipKeys = true, SkipType = false});
            if (File.Exists(outputPath) && AkOptionParser.AreEquivalent(json, File.ReadAllText(outputPath)))
            {
                return;
            }
            File.WriteAllText(outputPath, json);
        }

        /// <summary>
        /// Write the options to be used at runtime. Only overriden options (Unity + user overrides) will be written, and only with the minimal metadata required to set the options at runtime. Used both in editor, and at runtime.
        /// </summary>
        /// <param name="platformId">The string name of the platform, see <see cref="AkOptionPathGetter.RegisteredPlatforms"/></param>
        public static void WriteRuntimeOptions(string platformId)
        {
            var unityOverrides = LoadUnityOverrides(platformId);
            var projectOverrides = LoadProjectOverrides(platformId);
            var sdkOverrides = LoadSDKDefaults(platformId);
            // We only want to write the options that differ from the SDK defaults, so we start with the Unity defaults.
            var merged = unityOverrides.Merge(projectOverrides);

            // We grab the keys from the SDK, since those will have the correct, up-to-date keys
            var finalOptions = AkOptionExtensions.MergeKeysAndTypes(merged, sdkOverrides);

            var json = AkOptionParser.ToJson(finalOptions);

            var resourcePath = AkOptionPathGetter.ResourcesAbsolutePath;
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
            UnityEditor.AssetDatabase.ImportAsset(AkOptionPathGetter.ResourcesAssetPath);
        }

#endif // UNITY_EDITOR

        /// <summary>
        /// Load every override that should be applied at Sound Engine init: Unity-integration
        /// overrides merged with the user's project overrides. Used at init time so Play Mode and
        /// built runtime push the same set of values into Wwise.
        /// </summary>
        /// <param name="platformId">The string name of the platform, see <see cref="AkOptionPathGetter.RegisteredPlatforms"/></param>
        public static List<AkOptionNamespace> LoadRuntimeOptions(string platformId)
        {
#if UNITY_EDITOR
            // We write the runtime options every time in editor so that the flow for editor/runtime (load from Resources) is as similar as possible.
            WriteRuntimeOptions(platformId);
#endif
            var asset = Resources.Load<TextAsset>(AkOptionPathGetter.ResourcesPath);
            if (asset == null)
            {
                WwiseLogger.LogFormat(LogLevel.Warning, "Wwise options resource <{0}> not found. Sound Engine will initialize with default settings.", AkOptionPathGetter.ResourcesPath);
                return new List<AkOptionNamespace>();
            }
            return ParseOrEmpty(AkOptionParser, asset.text, AkOptionPathGetter.ResourcesPath);
        }
    }
}