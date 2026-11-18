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
using AK.Wwise.Unity.Settings;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings
{
    /// <summary>
    /// Store and retrieve the Wwise Runtime Settings.
    /// </summary>
    public class WwiseRuntimeSettings
    {
        public const string IntegrationNamespace = "Integration";
        public const string BanksNamespace = "IntegrationBanks";
        public const string FocusLossNamespace = "IntegrationFocusLoss";
        public const string LoggingNamespace = "IntegrationLogging";

        private static Dictionary<string, Dictionary<string, AkOptionEntry>> _optionMap;

        private static Dictionary<string, Dictionary<string, AkOptionEntry>> OptionMap
        {
            get
            {
                if (_optionMap == null)
                {
                    Reload();
                }
                return _optionMap;
            }
        }

        public static string SoundBanksBasePath
        {
            get => Get<string>(BanksNamespace, "SoundBanksBasePath");
            set => Set(BanksNamespace, "SoundBanksBasePath", value);
        }

        public static string StartupLanguage
        {
            get => Get<string>(IntegrationNamespace, "StartupLanguage");
            set => Set(IntegrationNamespace, "StartupLanguage", value);
        }

        public static bool IsEngineLoggingEnabled
        {
            get => Get<bool>(LoggingNamespace, "IsEngineLoggingEnabled");
            set => Set(LoggingNamespace, "IsEngineLoggingEnabled", value);
        }

        public static bool IsDecodedBankEnabled
        {
            get => Get<bool>(BanksNamespace, "IsDecodedBankEnabled");
            set => Set(BanksNamespace, "IsDecodedBankEnabled", value);
        }

        public static bool IsAutoBankEnabled
        {
            get => Get<bool>(BanksNamespace, "IsAutoBankEnabled");
            set => Set(BanksNamespace, "IsAutoBankEnabled", value);
        }

        public static float DefaultListenerScalingFactor
        {
            get => Get<float>(IntegrationNamespace, "DefaultListenerScalingFactor");
            set => Set(IntegrationNamespace, "DefaultListenerScalingFactor", value);
        }

        public static bool LoadBanksAsynchronously
        {
            get => Get<bool>(BanksNamespace, "LoadBanksAsynchronously");
            set => Set(BanksNamespace, "LoadBanksAsynchronously", value);
        }

        public static bool SuspendAudioDuringFocusLoss
        {
            get => Get<bool>(FocusLossNamespace, "SuspendAudioDuringFocusLoss");
            set => Set(FocusLossNamespace, "SuspendAudioDuringFocusLoss", value);
        }

        public static bool RenderDuringFocusLoss
        {
            get => Get<bool>(FocusLossNamespace, "RenderDuringFocusLoss");
            set => Set(FocusLossNamespace, "RenderDuringFocusLoss", value);
        }

        public static string SoundBanksPersistentDataPath
        {
            get => Get<string>(BanksNamespace, "SoundBankPersistentDataPath");
            set => Set(BanksNamespace, "SoundBankPersistentDataPath", value);
        }

        public static LogLevel WwiseLoggerLevel
        {
            get => Enum.TryParse<LogLevel>(Get<string>(LoggingNamespace, "WwiseLoggerLevel"), out var level)
                ? level
                : LogLevel.Log;
            set => Set(LoggingNamespace, "WwiseLoggerLevel", value.ToString());
        }


        /// <summary>
        /// Load the runtime settings and apply the configured Wwise logger level at runtime startup.
        /// </summary>
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void ReloadOnLoad()
        {
            Reload();
        }

        /// <summary>
        /// Reload the runtime settings for the platform the integration is running on.
        /// </summary>
        public static void Reload()
        {
#if UNITY_EDITOR
            // In the editor the plugin runs on the host, so load the host platform's settings.
            Reload(AkOptionPathGetter.EditorHostPlatformId);
#else
            // The player ships a single asset baked for its build target; platformId is unused at runtime.
            Reload(null);
#endif
        }

        public static void Reload(string platformId)
        {
            var Namespaces = WwiseRuntimeSettingsStore.LoadRuntimeSettings(platformId);
            _optionMap = Namespaces.ToOptionMap();

            // The Settings assembly depends on Logging, so we need to push the setting here to avoid a circular dependency.
            WwiseLogger.SetLogLevel(WwiseLoggerLevel);
        }

        private static T Get<T>(string namespaceName, string optionName)
        {
            if (OptionMap == null || !OptionMap.TryGetValue(namespaceName, out var akOptions) || !akOptions.TryGetValue(optionName, out var option) || option == null)
            {
                return default;
            }

            try
            {
                return (T)Convert.ChangeType(option.DefaultValue, typeof(T));
            }

            catch (Exception e) when (e is InvalidCastException or FormatException or OverflowException)
            {
                WwiseLogger.LogFormat(LogLevel.Warning, "Wwise runtime setting <{0}>.<{1}> could not be read as <{2}>. Using the default value.", namespaceName, optionName, typeof(T).Name);
                return default;
            }
        }

        private static void Set<T>(string namespaceName, string optionName, T defaultValue)
        {
            if (!OptionMap.TryGetValue(namespaceName, out var akOptions) || !akOptions.TryGetValue(optionName, out var option) || option == null)
            {
                WwiseLogger.LogFormat(LogLevel.Warning, "Could not set runtime setting <{1}>, <{0}>.<{1}> does not exist.", namespaceName, optionName);
                return;
            }
            option.DefaultValue = defaultValue;
        }
    }
}
