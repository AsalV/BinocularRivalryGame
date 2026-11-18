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
	internal class AkMacSettingsMigrator : IPlatformMigrator
	{
		public string SettingsName => "Mac";
		public string PlatformId => "Mac";

		public List<AkOptionNamespace> Migrate(AkCommonAdvancedSettings advanced, List<AkOptionNamespace> advancedOverrides,
			List<AkOptionNamespace> sdkDefaults) 
		{
			var macSettings = (AkMacSettings.PlatformAdvancedSettings)advanced;
			var resultNamespaces = new List<AkOptionNamespace>();
			AkOptionExtensions.AppendOptionOverride(sdkDefaults, resultNamespaces, "Output", "MaxSystemAudioObjects", macSettings.NumSpatialAudioPointSources);
			AkOptionExtensions.AppendOptionOverride(sdkDefaults, resultNamespaces, "Output", "EnableVerboseSystemLog", macSettings.VerboseSystemOutput);
			return resultNamespaces;
		}
	}
#pragma warning restore CS0618 // Type or member is obsolete
}
