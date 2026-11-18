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

using AK.Wwise.Unity.Logging;

[System.Obsolete(AkUnitySoundEngine.Migrated_Setting_AkOption_2026_1_2)]
public class AkWwiseInitializationSettings : AkCommonPlatformSettings
{

	[UnityEngine.HideInInspector]
	public AkCommonUserSettings UserSettings = new AkCommonUserSettings();
	[UnityEngine.HideInInspector]
	public AkCommonAdvancedSettings AdvancedSettings = new AkCommonAdvancedSettings();
	[UnityEngine.HideInInspector]
	public AkCommonCommSettings CommsSettings = new AkCommonCommSettings();

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

	public abstract class PlatformSettings : AkCommonPlatformSettings
	{
	}

	public class CommonPlatformSettings : PlatformSettings
	{
#pragma warning disable CS0672 // Member overrides obsolete member, required for migration
		public override AkCommonUserSettings GetUserSettings()
		{
			return UserSettings;
		}

		public override AkCommonAdvancedSettings GetAdvancedSettings()
		{
			return AdvancedSettings;
		}

		public override AkCommonCommSettings GetCommsSettings()
		{
			return CommsSettings;
		}
#pragma warning restore CS0672 // Member overrides obsolete member

		[UnityEngine.HideInInspector]
		public AkCommonUserSettings UserSettings;
		[UnityEngine.HideInInspector]
		public AkCommonAdvancedSettings AdvancedSettings;
		[UnityEngine.HideInInspector]
		public AkCommonCommSettings CommsSettings;
	}

	private static AkWwiseInitializationSettings m_Instance = null;

	public static AkWwiseInitializationSettings Instance
	{
		get
		{
			if (m_Instance == null)
			{
#if UNITY_EDITOR
				var name = typeof(AkWwiseInitializationSettings).Name;
				m_Instance = GetAsset<AkWwiseInitializationSettings>(name, name);
#else
				// We try to force load the settings if they haven't already been loaded
				var loaded = UnityEngine.Resources.FindObjectsOfTypeAll<AkWwiseInitializationSettings>();
				if (loaded.Length > 0)
				{
					m_Instance = loaded[0];
				}
				else
				{
					m_Instance = CreateInstance<AkWwiseInitializationSettings>();
					WwiseLogger.Warning("AkWwiseInitializationSettings is deprecated (replaced with AkOption); returning default values.");
				}
#endif
			}

			return m_Instance;
		}
	}

#if UNITY_EDITOR
	// Loads the existing settings asset if one is present, otherwise a transient with default values
	private static T GetAsset<T>(string className, string fileName) where T : AkCommonPlatformSettings
	{
		var path = System.IO.Path.Combine(AkWwiseEditorSettings.WwiseScriptableObjectRelativePath, fileName + ".asset");
		var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
		if (asset)
		{
			return asset;
		}

		var guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
		foreach (var assetGuid in guids)
		{
			var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(assetGuid);
			asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
			if (asset)
			{
				return asset;
			}
		}

		WwiseLogger.LogFormat(LogLevel.Warning, "{0} is deprecated (replaced with AkOption); returning a transient instance with default values.", typeof(T).Name);
		return CreateInstance(className) as T;
	}
	#endif
}
