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
using UnityEngine;
[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
public class AkAndroidSettings : AkWwiseInitializationSettings.PlatformSettings
{
	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public override AkCommonUserSettings GetUserSettings()
	{
		return UserSettings;
	}

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public override AkCommonAdvancedSettings GetAdvancedSettings()
	{
		return AdvancedSettings;
	}

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public override AkCommonCommSettings GetCommsSettings()
	{
		return CommsSettings;
	}

	[System.Serializable]
	public class PlatformAdvancedSettings : AkCommonAdvancedSettings
	{
		[Flags]
		public enum AudioAPI
		{
			AndroidAudio = 1 << 0,
			OpenSL_ES = 1 << 1
		}
		
		[Flags]
		public enum SpatializerAPI
		{
			DolbyAtmos = 1 << 8,
			AndroidSpatializer = 1 << 9
		}

		public enum AudioPath
		{
			Legacy,
			LowLatency,
			Exclusive
		}

		[AkEnumFlag(typeof(AudioAPI))]
		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public AudioAPI m_AudioAPI = AudioAPI.AndroidAudio | AudioAPI.OpenSL_ES;
		
		[AkEnumFlag(typeof(SpatializerAPI))]
		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public SpatializerAPI m_SpatializerAPI = SpatializerAPI.DolbyAtmos;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public AudioPath m_AudioPath = AudioPath.LowLatency;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public bool m_VerboseSink = false;

	}

	[SerializeField]
	private AkCommonUserSettings UserSettings;

	[SerializeField]
	private PlatformAdvancedSettings AdvancedSettings;

	[SerializeField]
	private AkCommonCommSettings CommsSettings;
}
