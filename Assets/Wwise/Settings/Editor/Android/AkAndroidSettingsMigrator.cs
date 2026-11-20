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
#pragma warning disable CS0618 // Type or member is obsolete, we need to access for migration
	internal class AkAndroidSettingsMigrator : IPlatformMigrator
	{
		public string SettingsName => "Android";
		public string PlatformId => "Android";
		public List<AkOptionNamespace> Migrate(AkCommonAdvancedSettings advancedSettings, List<AkOptionNamespace> commonOverrides, List<AkOptionNamespace> sdkDefaults)
		{
			var androidAdvanced = (AkAndroidSettings.PlatformAdvancedSettings)advancedSettings;
			var resultNamespaces = new List<AkOptionNamespace>();

			bool allowAAudio = (androidAdvanced.m_AudioAPI & AkAndroidSettings.PlatformAdvancedSettings.AudioAPI.AndroidAudio) != 0;
			AkOptionExtensions.AppendOptionOverride(sdkDefaults, resultNamespaces, "Android", "AllowAAudio", allowAAudio);

			bool allowDolbyAtmos = (androidAdvanced.m_SpatializerAPI & AkAndroidSettings.PlatformAdvancedSettings.SpatializerAPI.DolbyAtmos) != 0;
			bool allowSpatializer = (androidAdvanced.m_SpatializerAPI & AkAndroidSettings.PlatformAdvancedSettings.SpatializerAPI.AndroidSpatializer) != 0;

			AkOptionExtensions.AppendOptionOverride(sdkDefaults, resultNamespaces, "Android", "AllowDolbyAtmos", allowDolbyAtmos);
			AkOptionExtensions.AppendOptionOverride(sdkDefaults, resultNamespaces, "Android", "AllowAndroidSpatializer", allowSpatializer);

			AkOptionExtensions.AppendOptionOverride(sdkDefaults, resultNamespaces, "Android", "AudioPath", (long)androidAdvanced.m_AudioPath);

			AkOptionExtensions.AppendOptionOverride(sdkDefaults, resultNamespaces, "Output", "EnableVerboseSystemLog", androidAdvanced.m_VerboseSink);

			return resultNamespaces;
		}
	}
#pragma warning restore CS0618 // Type or member is obsolete
}
