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
using AK.Wwise.Unity.Logging;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace AK.Wwise.Unity.Settings.Editor
{
	/// <summary>
	/// Bakes the platform-specific options into the override
	/// </summary>
	public class AkOptionsBuildPreprocessor : IPreprocessBuildWithReport
	{
		public int callbackOrder => 0;

		public void OnPreprocessBuild(BuildReport report)
		{
			var target = report.summary.platform;
			if (!AkOptionPathGetter.RegisteredPlatforms.TryGetValue(target, out var platformId))
			{
				WwiseLogger.LogFormat(LogLevel.Warning, "No Wwise options registered for BuildTarget <{0}>. Removing any stale baked options and runtime settings; Sound Engine will fall back to defaults at runtime.", target);
				UnityEditor.AssetDatabase.DeleteAsset(AkOptionPathGetter.ResourcesAssetPath);
				UnityEditor.AssetDatabase.DeleteAsset(AkOptionPathGetter.RuntimeSettingsResourcesAssetPath);
				return;
			}
			AkOptionStore.WriteRuntimeOptions(platformId);
			WwiseRuntimeSettingsStore.WriteRuntimeSettings(platformId);
		}
	}
}

