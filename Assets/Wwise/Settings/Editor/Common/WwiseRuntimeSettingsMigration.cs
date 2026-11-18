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
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings.Editor
{
    /// <summary>
    /// Migrates the Wwise runtime settings off the old AkWwiseInitializationSettings ScriptableObject
    /// into the platform's runtime override. Each field is read directly off the platform's
    /// UserSettings/AdvancedSettings; AkOptionMigration calls this once per platform and persists the
    /// result via WwiseRuntimeSettingsStore.WriteProjectOverrides(platformId, ...).
    /// </summary>
    internal static class WwiseRuntimeSettingsMigration
    {
        /// <summary>
        /// Emit the runtime settings overrides that differ from <paramref name="defaults"/>. Pure mapping;
        /// the caller persists the result via WwiseRuntimeSettingsStore.WriteProjectOverrides.
        /// </summary>
        public static List<AkOptionNamespace> Migrate(AkCommonPlatformSettings platform, List<AkOptionNamespace> defaults)
        {
#pragma warning disable CS0618 // Type or member is obsolete, we need to access it for migration
            var user = platform.GetUserSettings();
            var advanced = platform.GetAdvancedSettings();
#pragma warning restore CS0618 // Type or member is obsolete

            var overrides = new List<AkOptionNamespace>();
#pragma warning disable CS0618 // Type or member is obsolete, we need to access it for migration
            // User settings.
            // Normalize separators to a forward slash.
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.BanksNamespace, "SoundBanksBasePath", user.m_BasePath?.Replace('\\', '/') ?? string.Empty);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.IntegrationNamespace, "StartupLanguage", user.m_StartupLanguage);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.LoggingNamespace, "IsEngineLoggingEnabled", user.m_EngineLogging);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.BanksNamespace, "IsDecodedBankEnabled", user.m_IsDecodedBankEnabled);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.BanksNamespace, "IsAutoBankEnabled", user.m_IsAutoBankEnabled);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.IntegrationNamespace, "DefaultListenerScalingFactor", user.m_DefaultListenerScalingFactor);

            // Advanced settings.
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.BanksNamespace, "LoadBanksAsynchronously", advanced.m_LoadBankAsynchronously);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.FocusLossNamespace, "SuspendAudioDuringFocusLoss", advanced.m_SuspendAudioDuringFocusLoss);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.FocusLossNamespace, "RenderDuringFocusLoss", advanced.m_RenderDuringFocusLoss);
            AkOptionExtensions.AppendOptionOverride(defaults, overrides, WwiseRuntimeSettings.BanksNamespace, "SoundBankPersistentDataPath", advanced.m_SoundBankPersistentDataPath ?? string.Empty);
#pragma warning restore CS0618 // Type or member is obsolete

            return overrides;
        }
    }
}
