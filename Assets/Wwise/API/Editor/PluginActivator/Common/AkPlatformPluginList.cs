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
using System.Linq;
using UnityEditor;
using AK.Wwise.Unity.Logging;

internal class AkPlatformPluginList
{
	internal static void GetPluginsUsedForPlatform(string deploymentTargetName, out HashSet<AkPluginInfo> UsedPlugins)
	{
		UsedPlugins = ParsePlugins(deploymentTargetName);
	}

	public static bool IsPluginUsed(AkPlatformPluginActivator in_config, string in_UnityPlatform, string in_PluginName)
	{
		var pluginDSPPlatform = in_config.WwisePlatformName;

		if (in_PluginName.Contains("AkUnitySoundEngine"))
		{
			return true;
		}

		var pluginName = in_PluginName;
		if (in_PluginName.StartsWith("lib"))
		{
			pluginName = in_PluginName.Substring(3);
		}

		const string factory = "Factory";
		int indexOfFactory = in_PluginName.IndexOf(factory);
		// Ensure the plugin name ends with "Factory.h"
		if (indexOfFactory != -1 && indexOfFactory + factory.Length == in_PluginName.Length)
		{
			pluginName = in_PluginName.Substring(0, indexOfFactory);
		}

		var plugins = ParsePlugins(pluginDSPPlatform);
		if (!in_config.RequiresStaticPluginRegistration)
		{
			foreach (var pluginInfo in plugins)
			{
				if (pluginInfo.DllName == pluginName ||
				    (pluginInfo.StaticLibName == "AkMeterFX" && pluginName.Contains("AkSoundEngine")))
				{
					return true;
				}
			}
		}

		// Exceptions
		if (!string.IsNullOrEmpty(in_config.StaticPluginRegistrationName) &&
		    pluginName.Contains(in_config.StaticPluginRegistrationName))
		{
			return true;
		}

		// Some platforms deal with the static libs directly, unlike all other platforms.
		// Luckily the DLL name is always a subset of the lib name.
		foreach (var pluginInfo in plugins)
		{
			if (pluginInfo.StaticLibName == pluginName)
			{
				return true;
			}
		}

		return false;
	}

	public static void ExecuteParse()
	{
		Update();
	}
	public static void Update(bool forceUpdate = false)
	{
		AkPluginActivator.ActivatePluginsForEditor();
	}

	private static bool PlatformUsesStaticLibs(string platform)
	{
		var pair = AkPluginActivator.BuildTargetToPlatformPluginActivator.FirstOrDefault(x => x.Value.WwisePlatformName == platform);
		
		if (pair.Equals(default(KeyValuePair<BuildTarget, AkPlatformPluginActivator>)))
		{
			// Platform not found
			return false;
		}
		return pair.Value.RequiresStaticPluginRegistration;
	}

	private static HashSet<AkPluginInfo> ParsePlugins(string platform)
	{
		var newPlugins = new System.Collections.Generic.HashSet<AkPluginInfo>();
		var usesStaticLibs = PlatformUsesStaticLibs(platform);

		try
		{
			WwisePluginRefArray pluginRefArray = new WwisePluginRefArray();
			for (var i = 0; i < WwiseProjectDatabase.GetPluginCount(); i++)
			{
				var pluginRef = pluginRefArray[i];
				var rawPluginID = pluginRef.Id;
				if (rawPluginID == 0)
				{
					continue;
				}
				AkPluginActivatorConstants.PluginID pluginID = (AkPluginActivatorConstants.PluginID)rawPluginID;

				if (AkPluginActivatorConstants.alwaysSkipPluginsIDs.Contains(pluginID))
				{
					continue;
				}

				var dll = string.Empty;

				if (!usesStaticLibs && AkPluginActivatorConstants.builtInPluginIDs.Contains(pluginID))
				{
					continue;
				}

				if (string.IsNullOrEmpty(dll))
				{
					dll = pluginRef.DLL;
				}

				var staticLibName = pluginRef.StaticLib;

				AkPluginInfo newPluginInfo = new AkPluginInfo();
				newPluginInfo.PluginID = rawPluginID;
				newPluginInfo.DllName = dll;
				newPluginInfo.StaticLibName = staticLibName;

				if (string.IsNullOrEmpty(newPluginInfo.StaticLibName) && !AkPluginActivatorConstants.PluginIDToStaticLibName.TryGetValue(pluginID, out newPluginInfo.StaticLibName))
				{
					newPluginInfo.StaticLibName = dll;
				}

				newPlugins.Add(newPluginInfo);
			}
		}
		catch (System.Exception ex)
		{
			WwiseLogger.LogFormat(LogLevel.Error, "Plugins could not be parsed. {0}", ex.Message);
		}

		return newPlugins;
	}
}