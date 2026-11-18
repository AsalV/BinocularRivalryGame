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

using AK.Wwise.Unity.Settings;

public class AkBasePlatformSettings : UnityEngine.ScriptableObject
{
	public virtual AkCallbackManager.InitializationSettings CallbackManagerInitializationSettings
	{
		get { return new AkCallbackManager.InitializationSettings(); }
	}

	public virtual string SoundBankPersistentDataPath
	{
		get { return null; }
	}

	public virtual string InitialLanguage => WwiseRuntimeSettings.StartupLanguage;

	public virtual bool LoadBanksAsynchronously
    {
		get 
		{
#if AK_WWISE_ADDRESSABLES && UNITY_ADDRESSABLES
			return true;
#else
			return false;
#endif
		}
		set { }
	}

	public virtual bool SuspendAudioDuringFocusLoss
	{
		get => WwiseRuntimeSettings.SuspendAudioDuringFocusLoss;
		set => WwiseRuntimeSettings.SuspendAudioDuringFocusLoss = value;
	}

	public virtual bool RenderDuringFocusLoss => WwiseRuntimeSettings.RenderDuringFocusLoss;

	public virtual string SoundbankPath => WwiseRuntimeSettings.SoundBanksBasePath;

	public virtual bool UseSubFoldersForGeneratedFiles
	{
		get { return false; }
	}

	public virtual uint MemoryDebugLevel
	{
		get { return 0; }
	}
	
	public virtual bool IsDecodedBankEnabled => WwiseRuntimeSettings.IsDecodedBankEnabled;

	public virtual bool IsAutoBankEnabled
	{
		get => WwiseRuntimeSettings.IsAutoBankEnabled;
		set => WwiseRuntimeSettings.IsAutoBankEnabled = value;
	}

	public virtual float defaultListenerScalingFactor => WwiseRuntimeSettings.DefaultListenerScalingFactor;
}

[System.Serializable]
public class AkCommonOutputSettings
{
	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public string m_AudioDeviceShareset = string.Empty;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_DeviceID = AkUnitySoundEngine.AK_INVALID_UNIQUE_ID;

	public enum PanningRule
	{
		Speakers = 0,
		Headphones = 1
	}

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public PanningRule m_PanningRule = PanningRule.Speakers;

	[System.Serializable]
	public class ChannelConfiguration
	{
		public enum ChannelConfigType
		{
			Anonymous = 0x0,
			Standard = 0x1,
			Ambisonic = 0x2
		}

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public ChannelConfigType m_ChannelConfigType = ChannelConfigType.Anonymous;

		public enum ChannelMask
		{
			NONE = 0x0,

			/// Standard speakers (channel mask):
			FRONT_LEFT = 0x1,        ///< Front left speaker bit mask
			FRONT_RIGHT = 0x2,       ///< Front right speaker bit mask
			FRONT_CENTER = 0x4,      ///< Front center speaker bit mask
			LOW_FREQUENCY = 0x8,     ///< Low-frequency speaker bit mask
			BACK_LEFT = 0x10,        ///< Rear left speaker bit mask
			BACK_RIGHT = 0x20,       ///< Rear right speaker bit mask
			BACK_CENTER = 0x100, ///< Rear center speaker ("surround speaker") bit mask
			SIDE_LEFT = 0x200,   ///< Side left speaker bit mask
			SIDE_RIGHT = 0x400,  ///< Side right speaker bit mask

			/// "Height" speakers.
			TOP = 0x800,     ///< Top speaker bit mask
			HEIGHT_FRONT_LEFT = 0x1000,  ///< Front left speaker bit mask
			HEIGHT_FRONT_CENTER = 0x2000,    ///< Front center speaker bit mask
			HEIGHT_FRONT_RIGHT = 0x4000, ///< Front right speaker bit mask
			HEIGHT_BACK_LEFT = 0x8000,   ///< Rear left speaker bit mask
			HEIGHT_BACK_CENTER = 0x10000,    ///< Rear center speaker bit mask
			HEIGHT_BACK_RIGHT = 0x20000, ///< Rear right speaker bit mask

			//
			// Supported speaker setups. Those are the ones that can be used in the Wwise Sound Engine audio pipeline.
			//
			SETUP_MONO = FRONT_CENTER,        ///< 1.0 setup channel mask
			SETUP_0POINT1 = LOW_FREQUENCY,    ///< 0.1 setup channel mask
			SETUP_1POINT1 = (FRONT_CENTER | LOW_FREQUENCY),    ///< 1.1 setup channel mask
			SETUP_STEREO = (FRONT_LEFT | FRONT_RIGHT), ///< 2.0 setup channel mask
			SETUP_2POINT1 = (SETUP_STEREO | LOW_FREQUENCY),    ///< 2.1 setup channel mask
			SETUP_3STEREO = (SETUP_STEREO | FRONT_CENTER), ///< 3.0 setup channel mask
			SETUP_3POINT1 = (SETUP_3STEREO | LOW_FREQUENCY),   ///< 3.1 setup channel mask
			SETUP_4 = (SETUP_STEREO | SIDE_LEFT | SIDE_RIGHT),  ///< 4.0 setup channel mask
			SETUP_4POINT1 = (SETUP_4 | LOW_FREQUENCY), ///< 4.1 setup channel mask
			SETUP_5 = (SETUP_4 | FRONT_CENTER),    ///< 5.0 setup channel mask
			SETUP_5POINT1 = (SETUP_5 | LOW_FREQUENCY), ///< 5.1 setup channel mask
			SETUP_6 = (SETUP_4 | BACK_LEFT | BACK_RIGHT),   ///< 6.0 setup channel mask
			SETUP_6POINT1 = (SETUP_6 | LOW_FREQUENCY), ///< 6.1 setup channel mask
			SETUP_7 = (SETUP_6 | FRONT_CENTER),    ///< 7.0 setup channel mask
			SETUP_7POINT1 = (SETUP_7 | LOW_FREQUENCY), ///< 7.1 setup channel mask
			SETUP_SURROUND = (SETUP_STEREO | BACK_CENTER), ///< Legacy surround setup channel mask

			// Note. DPL2 does not really have 4 channels, but it is used by plugins to differentiate from stereo setup.
			SETUP_DPL2 = (SETUP_4),       ///< Legacy DPL2 setup channel mask

			SETUP_HEIGHT_4 = (HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT),    ///< 4 speaker height layer.
			SETUP_HEIGHT_5 = (SETUP_HEIGHT_4 | HEIGHT_FRONT_CENTER),                                                                   ///< 5 speaker height layer.
			SETUP_HEIGHT_ALL = (SETUP_HEIGHT_5 | HEIGHT_BACK_CENTER),                                                                      ///< All height speaker layer.

			// Auro speaker setups
			SETUP_AURO_222 = (SETUP_4 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT),    ///< Auro-222 setup channel mask
			SETUP_AURO_8 = (SETUP_AURO_222 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT),     ///< Auro-8 setup channel mask
			SETUP_AURO_9 = (SETUP_AURO_8 | FRONT_CENTER),                                          ///< Auro-9.0 setup channel mask
			SETUP_AURO_9POINT1 = (SETUP_AURO_9 | LOW_FREQUENCY),                                           ///< Auro-9.1 setup channel mask
			SETUP_AURO_10 = (SETUP_AURO_9 | TOP),                                                  ///< Auro-10.0 setup channel mask		
			SETUP_AURO_10POINT1 = (SETUP_AURO_10 | LOW_FREQUENCY),                                         ///< Auro-10.1 setup channel mask	
			SETUP_AURO_11 = (SETUP_AURO_10 | HEIGHT_FRONT_CENTER),                                 ///< Auro-11.0 setup channel mask
			SETUP_AURO_11POINT1 = (SETUP_AURO_11 | LOW_FREQUENCY),                                         ///< Auro-11.1 setup channel mask	
			SETUP_AURO_11_740 = (SETUP_7 | SETUP_HEIGHT_4),                                        ///< Auro-11.0 (7+4) setup channel mask
			SETUP_AURO_11POINT1_740 = (SETUP_AURO_11_740 | LOW_FREQUENCY),                                     ///< Auro-11.1 (7+4) setup channel mask
			SETUP_AURO_13_751 = (SETUP_7 | SETUP_HEIGHT_5 | TOP),                       ///< Auro-13.0 setup channel mask
			SETUP_AURO_13POINT1_751 = (SETUP_AURO_13_751 | LOW_FREQUENCY),                                     ///< Auro-13.1 setup channel mask

			// Dolby speaker setups: in Dolby nomenclature, [#plane].[lfe].[#height]
			SETUP_DOLBY_5_0_2 = (SETUP_5 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 5.0.2 setup channel mask
			SETUP_DOLBY_5_1_2 = (SETUP_DOLBY_5_0_2 | LOW_FREQUENCY),                                   ///< Dolby 5.1.2 setup channel mask
			SETUP_DOLBY_6_0_2 = (SETUP_6 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 6.0.2 setup channel mask
			SETUP_DOLBY_6_1_2 = (SETUP_DOLBY_6_0_2 | LOW_FREQUENCY),                                   ///< Dolby 6.1.2 setup channel mask
			SETUP_DOLBY_6_0_4 = (SETUP_DOLBY_6_0_2 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT), ///< Dolby 6.0.4 setup channel mask
			SETUP_DOLBY_6_1_4 = (SETUP_DOLBY_6_0_4 | LOW_FREQUENCY),                                   ///< Dolby 6.1.4 setup channel mask
			SETUP_DOLBY_7_0_2 = (SETUP_7 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 7.0.2 setup channel mask
			SETUP_DOLBY_7_1_2 = (SETUP_DOLBY_7_0_2 | LOW_FREQUENCY),                                   ///< Dolby 7.1.2 setup channel mask
			SETUP_DOLBY_7_0_4 = (SETUP_DOLBY_7_0_2 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT), ///< Dolby 7.0.4 setup channel mask
			SETUP_DOLBY_7_1_4 = (SETUP_DOLBY_7_0_4 | LOW_FREQUENCY),                                   ///< Dolby 7.1.4 setup channel mask

			SETUP_ALL_SPEAKERS = (SETUP_7POINT1 | BACK_CENTER | SETUP_HEIGHT_ALL | TOP), ///< All speakers.
		};

		[AkEnumFlag(typeof(ChannelMask))]
		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public ChannelMask m_ChannelMask = ChannelMask.NONE;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_NumberOfChannels = 0;
	}

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public ChannelConfiguration m_ChannelConfig = new ChannelConfiguration();
}

[System.Serializable]
public partial class AkCommonUserSettings
{
	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public string m_BasePath = AkBasePathGetter.DefaultBasePath;

	[UnityEngine.HideInInspector]
	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public string m_StartupLanguage = "English(US)";

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public bool m_EngineLogging = AkCallbackManager.InitializationSettings.DefaultIsLoggingEnabled;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public bool m_IsDecodedBankEnabled = false;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public bool m_IsAutoBankEnabled = true;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public float m_DefaultListenerScalingFactor = 1.0f;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_MaximumNumberOfPositioningPaths = 255;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_CommandQueueSize = 256 * 1024;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_SamplesPerFrame = 512;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public AkCommonOutputSettings m_MainOutputSettings;

	[UnityEngine.Range(0, 1)]
	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public float m_StreamingLookAheadRatio = 1.0f;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_SampleRate = 48000;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public ushort m_NumberOfRefillsInVoice = 4;

	[System.Serializable]
	public class SpatialAudioSettings
	{
		public enum TransmissionOperation
		{
			Add,
			Multiply,
			Max
		};

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_MaxSoundPropagationDepth = AkUnitySoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public float m_MovementThreshold = 0.25f;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_NumberOfPrimaryRays = 35;

		[UnityEngine.Serialization.FormerlySerializedAs("m_ReflectionsOrder")]
		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_MaxReflectionOrder = 2;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_MaxDiffractionOrder = 4;

		[UnityEngine.Range(0, 4)]
		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_DiffractionOnReflectionsOrder = 2;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_MaxEmitterRoomAuxSends = 3;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public float m_MaxPathLength = 1000.0f;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public float m_CPULimitPercentage = 0.0f;

		[UnityEngine.Serialization.FormerlySerializedAs("m_EnableDirectPathDiffraction")]
		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public bool m_EnableGeometricDiffractionAndTransmission = true;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public bool m_CalcEmitterVirtualPosition = true;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_LoadBalancingSpread = 1;

		[UnityEngine.MinAttribute(0)]
		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_MaxDiffractionPaths = 8;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public float m_MaxDiffractionAngleDegrees = 180.0f;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public float m_SmoothingConstantMs = 0.0f;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public TransmissionOperation m_TransmissionOperation = TransmissionOperation.Max;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public uint m_ClusteringMinPoints = 0;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public float m_ClusteringMaxDistance = 5.0f;

		[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
		public float m_ClusteringDeadZoneDistance = 10.0f;
	}

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public SpatialAudioSettings m_SpatialAudioSettings;
}

[System.Serializable]
public class AkCommonAdvancedSettings
{
	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_IOMemorySize = 2 * 1024 * 1024;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public float m_TargetAutoStreamBufferLengthMs = 380.0f;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public bool m_UseStreamCache = false;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public bool m_LoadBankAsynchronously = false;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public bool m_EnableGameSyncPreparation = false;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_MonitorQueuePoolSize = 1024 * 1024;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_MaximumHardwareTimeoutMs = 1000;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public bool m_DebugOutOfRangeCheckEnabled = false;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public float m_DebugOutOfRangeLimit = 16.0f;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public bool m_SuspendAudioDuringFocusLoss = true;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public bool m_RenderDuringFocusLoss;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_WwiseRuntimeSettings_2026_1_2)]
	public string m_SoundBankPersistentDataPath;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public bool m_UseSubFoldersForGeneratedFiles;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public uint m_MemoryDebugLevel = 0;
}

[System.Serializable]
public class AkCommonCommSettings
{
	public static ushort DefaultDiscoveryBroadcastPort = 24024;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public ushort m_DiscoveryBroadcastPort = DefaultDiscoveryBroadcastPort;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public ushort m_CommandPort;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public bool m_InitializeSystemComms = true;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public string m_NetworkName;

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public AkCommSystem m_commSystem = AkCommSystem.AkCommSystem_Socket;

}

public abstract class AkCommonPlatformSettings : AkBasePlatformSettings
{
	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public abstract AkCommonUserSettings GetUserSettings();

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public abstract AkCommonAdvancedSettings GetAdvancedSettings();

	[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
	public abstract AkCommonCommSettings GetCommsSettings();

	public override AkCallbackManager.InitializationSettings CallbackManagerInitializationSettings => new() { IsLoggingEnabled = WwiseRuntimeSettings.IsEngineLoggingEnabled };

	public override string InitialLanguage => WwiseRuntimeSettings.StartupLanguage;

	public override bool IsDecodedBankEnabled => WwiseRuntimeSettings.IsDecodedBankEnabled;

	public override bool IsAutoBankEnabled
	{
		get => WwiseRuntimeSettings.IsAutoBankEnabled;
		set => WwiseRuntimeSettings.IsAutoBankEnabled = value;
	}

	public override bool LoadBanksAsynchronously
	{
		get => WwiseRuntimeSettings.LoadBanksAsynchronously;
		set => WwiseRuntimeSettings.LoadBanksAsynchronously = value;
	}

	public override string SoundBankPersistentDataPath => WwiseRuntimeSettings.SoundBanksPersistentDataPath;

	public override bool SuspendAudioDuringFocusLoss
	{
		get => WwiseRuntimeSettings.SuspendAudioDuringFocusLoss;
		set => WwiseRuntimeSettings.SuspendAudioDuringFocusLoss = value;
	}

	public override bool RenderDuringFocusLoss => WwiseRuntimeSettings.RenderDuringFocusLoss;

	public override string SoundbankPath => WwiseRuntimeSettings.SoundBanksBasePath;

	public override float defaultListenerScalingFactor => WwiseRuntimeSettings.DefaultListenerScalingFactor;
}
