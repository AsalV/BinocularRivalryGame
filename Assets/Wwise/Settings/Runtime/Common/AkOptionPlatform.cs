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

namespace AK.Wwise.Unity.Settings
{
	public class AkOptionPlatform
	{
		// The name of the platform according to the Wwise SDK
		public readonly string WwiseSDKName;
		// The name of the platform in the Wwise Unity Integration
		public readonly string IntegrationName;
		// The name to display in the settings
		public readonly string DisplayName;
		// The name of the SDK json file of the Wwise settings (usually following the format "Options.WwiseSDKName.json")
		public readonly string SDKOptionFileName;

		// The identifier of the platform to be used in registries
		public string Id => IntegrationName;

		/// <summary>
		/// Construct a Platform for use with <see cref="AkOptionPathGetter"/>
		/// Use this if the platformName differs between the Wwise SDK and Unity Integration
		/// </summary>
		/// <param name="wwiseSDKName">The name of the platform according to the Wwise SDK.</param>
		/// <param name="integrationName">The name of the platform in the Wwise Unity Integration.</param>
		/// <param name="sdkOptionFileName">The name of the Options file as defined in the SDK.</param>
		/// <param name="displayName">The pretty name of the platform</param>
		public AkOptionPlatform(string wwiseSDKName, string integrationName, string sdkOptionFileName, string displayName)
		{
			WwiseSDKName = wwiseSDKName;
			IntegrationName = integrationName;
			DisplayName = displayName;
			SDKOptionFileName = sdkOptionFileName;
		}

		/// <summary>
		/// Use this constructor if the Platform name is identical everywhere
		/// </summary>
		/// <param name="platformName">The name of the platform, used for the SDK name, the integration name and the display name alike.</param>
		public AkOptionPlatform(string platformName)
		{
			WwiseSDKName = platformName;
			IntegrationName = platformName;
			DisplayName = platformName;
			SDKOptionFileName = $"Options.{platformName}.json";
		}
	}
}