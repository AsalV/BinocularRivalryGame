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
using AK.Wwise.Unity.Logging;
using UnityEditor;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings.Editor
{
    /// <summary>
    /// Applies the Wwise logger verbosity in the editor at load time.
    /// We avoid the WwiseRuntimeSetting approach to loading this setting, since we want to bypass the asset database, and avaoid timing conflicts.
    /// </summary>
    internal static class WwiseLoggerEditorInitializer
    {

        [InitializeOnLoadMethod]
        internal static void ApplyLogLevel()
        {
            var platformId = AkOptionPathGetter.EditorHostPlatformId;

            var map = WwiseRuntimeSettingsStore.LoadEffectiveSettings(platformId).ToOptionMap();
            if (map.TryGetValue(WwiseRuntimeSettings.LoggingNamespace, out var options)
                && options.TryGetValue("WwiseLoggerLevel", out var option)
                && option?.DefaultValue != null
                && Enum.TryParse<LogLevel>(option.DefaultValue.ToString(), out var level))
            {
                WwiseLogger.SetLogLevel(level);
            }
        }
    }
}
