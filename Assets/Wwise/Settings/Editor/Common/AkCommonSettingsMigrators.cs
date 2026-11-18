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
	internal static class AkCommonUserSettingsMigrator
	{
		public static List<AkOptionNamespace> Migrate(AkCommonUserSettings s, List<AkOptionNamespace> defaultSDKOptions)
		{
			var resultNamespaces = new List<AkOptionNamespace>();

			// SoundEngine
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "SoundEngine", "MaxNumPaths", s.m_MaximumNumberOfPositioningPaths);

			// AudioThread
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "AudioMgr", "CommandQueueSize", s.m_CommandQueueSize);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "AudioMgr", "NumSamplesPerFrame", s.m_SamplesPerFrame);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "AudioMgr", "SampleRate", s.m_SampleRate);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "AudioMgr", "MultipleBuffering", s.m_NumberOfRefillsInVoice);

			// StreamDevice
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "StreamMgr", "LookAheadRatio", s.m_StreamingLookAheadRatio);

			// Output
			var sharesetHash = string.IsNullOrEmpty(s.m_MainOutputSettings.m_AudioDeviceShareset)
				? AkUnitySoundEngine.AK_INVALID_UNIQUE_ID
				: AkUtilities.ShortIDGenerator.Compute(s.m_MainOutputSettings.m_AudioDeviceShareset);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Output", "AudioDeviceShareset", sharesetHash);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Output", "DeviceID", s.m_MainOutputSettings.m_DeviceID);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Output", "PanningRule", (long)s.m_MainOutputSettings.m_PanningRule);

			// ChannelConfig via native serialization
			var channelCfg = s.m_MainOutputSettings.m_ChannelConfig;
			AkChannelConfig akConfig = new AkChannelConfig();
			switch (channelCfg.m_ChannelConfigType)
			{
				case AkCommonOutputSettings.ChannelConfiguration.ChannelConfigType.Standard:
					akConfig.SetStandard((uint)channelCfg.m_ChannelMask);
					break;
				case AkCommonOutputSettings.ChannelConfiguration.ChannelConfigType.Ambisonic:
					akConfig.SetAmbisonic((uint)channelCfg.m_NumberOfChannels);
					break;
				default:
					akConfig.SetAnonymous((uint)channelCfg.m_NumberOfChannels);
					break;
			}
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Output", "ChannelConfig", akConfig.Serialize());
			akConfig.Dispose();

			// Acoustics
			var sa = s.m_SpatialAudioSettings;
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "MaxSoundPropagationDepth", sa.m_MaxSoundPropagationDepth);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "MovementThreshold", sa.m_MovementThreshold);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "NumberOfPrimaryRays", sa.m_NumberOfPrimaryRays);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "MaxReflectionOrder", sa.m_MaxReflectionOrder);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "MaxDiffractionOrder", sa.m_MaxDiffractionOrder);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "MaxDiffractionPaths", sa.m_MaxDiffractionPaths);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "MaxEmitterRoomAuxSends", sa.m_MaxEmitterRoomAuxSends);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "DiffractionOnReflectionsOrder", sa.m_DiffractionOnReflectionsOrder);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "MaxDiffractionAngleDegrees", sa.m_MaxDiffractionAngleDegrees);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "MaxPathLength", sa.m_MaxPathLength);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "CPULimitPercentage", sa.m_CPULimitPercentage);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "SmoothingConstantMs", sa.m_SmoothingConstantMs);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "LoadBalancingSpread", sa.m_LoadBalancingSpread);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "EnableGeometricDiffractionAndTransmission", sa.m_EnableGeometricDiffractionAndTransmission);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "CalcEmitterVirtualPosition", sa.m_CalcEmitterVirtualPosition);

			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "TransmissionOperation", (long)sa.m_TransmissionOperation);

			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "ClusteringMinPoints", sa.m_ClusteringMinPoints);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "ClusteringMaxDistance", sa.m_ClusteringMaxDistance);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Acoustics", "ClusteringDeadZoneDistance", sa.m_ClusteringDeadZoneDistance);

			return resultNamespaces;
		}
	}

	internal static class AkCommonAdvancedSettingsMigrator
	{
		public static List<AkOptionNamespace> Migrate(AkCommonAdvancedSettings s, List<AkOptionNamespace> defaultSDKOptions)
		{
			var resultNamespaces = new List<AkOptionNamespace>();

			// StreamDevice
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "StreamMgr", "IOMemorySize", s.m_IOMemorySize);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "StreamMgr", "AutoStreamTargetBufferLength", s.m_TargetAutoStreamBufferLengthMs);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "StreamMgr", "UseStreamCache", s.m_UseStreamCache);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "StreamMgr", "UseSubfoldering", s.m_UseSubFoldersForGeneratedFiles);

			// BankMgr
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "BankMgr", "EnableManualGameSyncPreparation", s.m_EnableGameSyncPreparation);

			// Profiler
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Profiler", "MonitorQueuePoolSize", s.m_MonitorQueuePoolSize);

			// AudioThread
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "AudioMgr", "MaxHardwareTimeoutMs", s.m_MaximumHardwareTimeoutMs);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "AudioMgr", "EnableDebugOutOfRangeCheck", s.m_DebugOutOfRangeCheckEnabled);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "AudioMgr", "DebugOutOfRangeLimit", s.m_DebugOutOfRangeLimit);

			// MemoryMgr
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "MemoryMgr", "DebugLevel", s.m_MemoryDebugLevel);

			return resultNamespaces;
		}
	}

	internal static class AkCommonCommSettingsMigrator
	{
		public static List<AkOptionNamespace> Migrate(AkCommonCommSettings s, List<AkOptionNamespace> defaultSDKOptions)
		{
			var resultNamespaces = new List<AkOptionNamespace>();

			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Comm", "DiscoveryPort", s.m_DiscoveryBroadcastPort);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Comm", "CommandPort", s.m_CommandPort);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Comm", "EnableSystemCommsManagement", s.m_InitializeSystemComms);
			AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Comm", "API", (long)s.m_commSystem);
			if (!string.IsNullOrEmpty(s.m_NetworkName))
			{
				AkOptionExtensions.AppendOptionOverride(defaultSDKOptions, resultNamespaces, "Comm", "AppNetworkName", s.m_NetworkName ?? "");
			}

			return resultNamespaces;
		}
	}
#pragma warning restore CS0618 // Type or member is obsolete
}
