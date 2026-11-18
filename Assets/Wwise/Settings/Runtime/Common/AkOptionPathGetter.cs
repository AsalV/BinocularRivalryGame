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

namespace AK.Wwise.Unity.Settings
{
    public class AkOptionPathGetter
    {
        private static readonly Dictionary<string, AkOptionPlatform> PlatformOptions = new();

        private static readonly string EditorOptionsDir = Path.Combine("Assets", "Wwise", "Settings", "Editor");
        private static readonly string ProjectOptionsDir = Path.Combine("ProjectSettings", "Wwise");

        private static readonly string EditorRuntimeSettingsDir = Path.Combine("Assets", "Wwise", "Settings", "Editor");
        private static readonly string ProjectRuntimeSettingsDir = Path.Combine("ProjectSettings", "Wwise");

        private readonly string _platformId;

        public AkOptionPathGetter(string platformId) => _platformId = platformId;

#if UNITY_EDITOR
        private static readonly Dictionary<UnityEditor.BuildTarget, string> BuildTargetToPlatformId = new();

        public AkOptionPathGetter(UnityEditor.BuildTarget target)
            : this(BuildTargetToPlatformId[target]) { }

        /// <summary>
        /// Register a platform's Options.{platform}.json and its BuildTarget-to-platformId mapping.
        /// </summary>
        /// <param name="target">The Unity <see cref="UnityEditor.BuildTarget"/> for this platform.</param>
        /// <param name="platform">The platform and paths to identify the platform.</param>
        public static void RegisterPlatform(UnityEditor.BuildTarget target, AkOptionPlatform platform)
        {
            BuildTargetToPlatformId[target] = platform.Id;
            RegisterPlatform(platform);
        }

        /// <summary>
        /// Retrieves the registered platform if it exists
        /// </summary>
        /// <param name="platformId">This corresponds to the </param>
        /// <returns></returns>
        public static AkOptionPlatform GetPlatform(string platformId)
        {
            return PlatformOptions.GetValueOrDefault(platformId);
        }

        /// <summary>
        /// Register only the SDK options JSON path for a platform, without a BuildTarget mapping.
        /// Use this when the BuildTarget enum value may not be available in the current Unity install
        /// <remarks>A <see cref="UnityEditor.BuildTarget"/> must also be registered with <see cref="RegisterPlatform(UnityEditor.BuildTarget, AkOptionPlatform)"/> in order to bake the settings into a build.</remarks>
        /// </summary>
        /// <param name="platform">The platform and paths to identify the platform.</param>
        public static void RegisterPlatform(AkOptionPlatform platform)
        {
            PlatformOptions[platform.Id] = platform;
        }

        /// <summary>
        /// All registered BuildTargets and their platform identity strings.
        /// </summary>
        public static IReadOnlyDictionary<UnityEditor.BuildTarget, string> RegisteredPlatforms => BuildTargetToPlatformId;

        /// <summary>
        /// The platform id for the host the editor is running on, or null when it isn't a known host.
        /// </summary>
        public static string EditorHostPlatformId => UnityEngine.Application.platform switch
        {
            UnityEngine.RuntimePlatform.WindowsEditor => "Windows",
            UnityEngine.RuntimePlatform.OSXEditor => "Mac",
            UnityEngine.RuntimePlatform.LinuxEditor => "Linux",
            _ => null,
        };

        /// <summary>
        /// All platform identity strings that have a registered options JSON, including
        /// platforms whose Unity BuildTarget enum value is not available in this install
        /// (registered via <see cref="RegisterPlatform(AK.Wwise.Unity.Settings.AkOptionPlatform)"/>).
        /// </summary>
        public static IReadOnlyCollection<string> RegisteredPlatformIds => PlatformOptions.Keys;
#endif

        /// SDK cross-platform defaults (read-only)
        public string SdkCommonPath => Path.Combine(EditorOptionsDir, "Common", "Options.Common.json");

        /// SDK platform-specific defaults (read-only, registered per-platform)
        public string SdkPlatformPath
        {
            get
            {
                var platform = PlatformOptions.GetValueOrDefault(_platformId);
                if (platform == null)
                {
                    return null;
                }
                return Path.Combine(EditorOptionsDir, platform.IntegrationName, platform.SDKOptionFileName);
            }
        }

        /// Unity-integration cross-platform overrides (read-only, shipped with Wwise Unity)
        public string UnityCommonOverridePath => Path.Combine(EditorOptionsDir, "Common", "Options.Common.Unity.json");

        /// Unity-integration platform-specific overrides (read-only, shipped with Wwise Unity)
        public string UnityPlatformOverridePath
        {
            get
            {
                var sdkPath = SdkPlatformPath;
                if (sdkPath == null)
                {
                    return null;
                }
                var dir = Path.GetDirectoryName(sdkPath);
                var baseName = Path.GetFileNameWithoutExtension(sdkPath);
                return Path.Combine(dir ?? string.Empty, $"{baseName}.Unity.json");
            }
        }

        /// Project platform-specific overrides (user-editable)
        public string ProjectPlatformOverridePath => Path.Combine(ProjectOptionsDir, $"Options.{_platformId}.json");

        /// Runtime: baked overrides asset, loaded via Resources.Load&lt;TextAsset&gt;(ResourcesPath)
        public const string ResourcesPath = "WwiseOptions";

#if UNITY_EDITOR
        private static readonly string ResourcesSubPath = Path.Combine("Wwise", "Resources", "WwiseOptions.json");

        /// Asset-database path used by the build preprocessor to write/delete the baked file
        public static string ResourcesAssetPath => Path.Combine("Assets", ResourcesSubPath);

        /// Filesystem path for direct File.IO on the baked file
        public static string ResourcesAbsolutePath => Path.Combine(UnityEngine.Application.dataPath, ResourcesSubPath);
#endif

        /// Runtime cross-platform defaults (read-only, shipped with Wwise Unity)
        public string RuntimeSettingsCommonPath => Path.Combine(EditorRuntimeSettingsDir, "Common", "Settings.Runtime.Common.json");

        /// Runtime platform-specific defaults (read-only, shipped with Wwise Unity)
        public string RuntimeSettingsPlatformPath =>
            _platformId == null ? null : Path.Combine(EditorRuntimeSettingsDir, _platformId, $"Settings.Runtime.{_platformId}.json");

        /// Runtime platform-specific overrides (user-editable)
        public string ProjectRuntimeSettingsPlatformOverridePath => Path.Combine(ProjectRuntimeSettingsDir, $"Settings.Runtime.{_platformId}.json");

        /// Runtime: baked settings asset, loaded via Resources.Load&lt;TextAsset&gt;(RuntimeSettingsResourcesPath)
        public const string RuntimeSettingsResourcesPath = "WwiseRuntimeSettings";

#if UNITY_EDITOR
        private static readonly string RuntimeSettingsResourcesSubPath = Path.Combine("Wwise", "Resources", "WwiseRuntimeSettings.json");

        /// Asset-database path used by the build preprocessor to write/delete the baked file
        public static string RuntimeSettingsResourcesAssetPath => Path.Combine("Assets", RuntimeSettingsResourcesSubPath);

        /// Filesystem path for direct File.IO on the baked file
        public static string RuntimeSettingsResourcesAbsolutePath => Path.Combine(UnityEngine.Application.dataPath, RuntimeSettingsResourcesSubPath);
#endif
    }
}
