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
using AK.Wwise.Unity.Settings;
using AK.Wwise.Unity.Logging;
using UnityEngine;
using Wwise.SDK;

public class AkUnitySoundEngineInitialization
{
	protected static AkUnitySoundEngineInitialization m_Instance;

	/// <summary>
	/// Invoked before <see cref="AkUnitySoundEngine.Init"/> has been called.
	/// This can be used to run code before initialization, for example to set AkOptions at runtime via AkOptionExtensions.SetOption.
	/// </summary>
	public System.Action<List<AkOptionNamespace>> PreInitializationDelegate;

	/// <summary>
	/// Invoked after <see cref="AkUnitySoundEngine.Init"/> has been called
	/// </summary>
	public System.Action PostInitializationDelegate;

	/// <summary>
	/// Invoked at the end of <see cref="ResetSoundEngine(bool)"/>
	/// </summary>
	public System.Action PostResetDelegate;

	/// <summary>
	/// Invoked before <see cref="AkUnitySoundEngine.Term"/> has been called
	/// </summary>
	public System.Action PreTerminationDelegate;

	/// <summary>
	/// Invoked after <see cref="AkUnitySoundEngine.Term"/> has been called
	/// </summary>
	public System.Action PostTerminationDelegate;

	/// <summary>
	/// Alias for <see cref="PreInitializationDelegate"/>.
	/// </summary>
	[System.Obsolete("preInitializationDelegate has been renamed to PreInitializationDelegate and will be removed in a future release.")]
	public System.Action<List<AkOptionNamespace>> preInitializationDelegate
	{
		get => PreInitializationDelegate;
		set => PreInitializationDelegate = value;
	}

	/// <summary>
	/// Alias for <see cref="PostInitializationDelegate"/>.
	/// </summary>
	[System.Obsolete("initializationDelegate has been renamed to PostInitializationDelegate and will be removed in a future release.")]
	public System.Action initializationDelegate
	{
		get => PostInitializationDelegate;
		set => PostInitializationDelegate = value;
	}

	/// <summary>
	/// Alias for <see cref="PostResetDelegate"/>.
	/// </summary>
	[System.Obsolete("reInitializationDelegate has been renamed to PostResetDelegate and will be removed in a future release.")]
	public System.Action reInitializationDelegate
	{
		get => PostResetDelegate;
		set => PostResetDelegate = value;
	}

	/// <summary>
	/// Alias for <see cref="PreTerminationDelegate"/>.
	/// </summary>
	[System.Obsolete("terminationDelegate has been renamed to PreTerminationDelegate and will be removed in a future release.")]
	public System.Action terminationDelegate
	{
		get => PreTerminationDelegate;
		set => PreTerminationDelegate = value;
	}

	/// <summary>
	/// Alias for <see cref="PostTerminationDelegate"/>.
	/// </summary>
	[System.Obsolete("postTerminationDelegate has been renamed to PostTerminationDelegate and will be removed in a future release.")]
	public System.Action postTerminationDelegate
	{
		get => PostTerminationDelegate;
		set => PostTerminationDelegate = value;
	}

	public static AkUnitySoundEngineInitialization Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new AkUnitySoundEngineInitialization();
			}

			return m_Instance;
		}
	}

	public bool InitializeSoundEngine()
	{
		WwiseLogger.LogFormat(LogLevel.Log, "Wwise(R) SDK Version {0}.", AkUnitySoundEngine.WwiseVersion);
		WwiseRuntimeSettings.Reload();
		var options = AkOptionStore.LoadRuntimeOptions(AkBasePathGetter.GetPlatformName());

		// Always reset the stale options before starting initialization
		AkUnitySoundEngine.ResetAkOptions();

		options = InjectDefaultAppNetworkName(options);
		options = InjectPluginPath(options);

		PreInitializationDelegate?.InvokeUnitySafe(options);

		AkUnitySoundEngine.SetupNativeHooks();

		var initResult = AkUnitySoundEngine.Init(options);

		if (initResult != AKRESULT.AK_Success)
		{
			WwiseLogger.LogFormat(LogLevel.Error, "Failed to initialize the sound engine. Reason: {0}", initResult);
			return false;
		}

		var akBasePathGetterInstance = AkBasePathGetter.Get();
		var soundBankBasePath = akBasePathGetterInstance.SoundBankBasePath;
		if (string.IsNullOrEmpty(soundBankBasePath))
		{
			// this is a nearly impossible situation
			WwiseLogger.Error("Couldn't find SoundBanks base path. Terminating sound engine.");
			AkUnitySoundEngine.Term();
			return false;
		}

		var persistentDataPath = akBasePathGetterInstance.PersistentDataPath;
		var isBasePathSameAsPersistentPath = soundBankBasePath == persistentDataPath;

#if UNITY_ANDROID
		var canSetBasePath = !isBasePathSameAsPersistentPath;
		var canSetPersistentDataPath = true;
#else
		var canSetBasePath = true;
		var canSetPersistentDataPath = !isBasePathSameAsPersistentPath;
#endif

		if (canSetBasePath)
		{
			var result =  AkUnitySoundEngine.AddBasePath(soundBankBasePath);
			if (result != AKRESULT.AK_Success)
			{
#if !AK_WWISE_ADDRESSABLES
#if !UNITY_ANDROID || UNITY_EDITOR
				var format =
					"Failed to set SoundBanks base path to <{0}> with result <{1}>. Make sure SoundBank path is correctly set under Edit > Project Settings > Wwise > Runtime Settings > Asset Management.";
				// It might be normal for SetBasePath to return AK_PathNotFound on Android. Silence the error log to avoid confusion.
				WwiseLogger.LogFormat(LogLevel.Error, format, soundBankBasePath, result);
#endif
#endif
			}
		}

		if (canSetPersistentDataPath && !string.IsNullOrEmpty(persistentDataPath))
		{
			AkUnitySoundEngine.AddBasePath(persistentDataPath);
		}

		var decodedBankFullPath = akBasePathGetterInstance.DecodedBankFullPath;
		if (!string.IsNullOrEmpty(decodedBankFullPath) && WwiseRuntimeSettings.IsDecodedBankEnabled)
		{
			// AkUnitySoundEngine.SetDecodedBankPath creates the folders for writing to (if they don't exist)
			AkUnitySoundEngine.SetDecodedBankPath(decodedBankFullPath);

			// Adding decoded bank path last to ensure that it is the first one used when writing decoded banks.
			AkUnitySoundEngine.AddBasePath(decodedBankFullPath);
		}

		var setLanguageResult = AkUnitySoundEngine.SetCurrentLanguage(WwiseRuntimeSettings.StartupLanguage);
		if (setLanguageResult != AKRESULT.AK_Success)
		{
			WwiseLogger.LogFormat(LogLevel.Error, "Failed to set language. Reason: {0}", setLanguageResult);
		}

		var callbackManagerInitSettings = new AkCallbackManager.InitializationSettings();
		callbackManagerInitSettings.IsLoggingEnabled = WwiseRuntimeSettings.IsEngineLoggingEnabled;
		AkCallbackManager.Init(callbackManagerInitSettings);

		WwiseLogger.Log("Sound engine initialized successfully.");
		LoadInitBank();
		PostInitializationDelegate?.InvokeUnitySafe();
		return true;
	}

	private static List<AkOptionNamespace> InjectDefaultAppNetworkName(List<AkOptionNamespace> options)
	{
		var appNameEntry = options.FirstOrDefault(n => n.Name == "Comm")?.Options
			.FirstOrDefault(o => o.Name == "AppNetworkName");
		// If an application name was already specified, keep it; otherwise default to the ProductName.
		if (appNameEntry != null)
		{
			return options;
		}

		var networkName = Application.productName;
#if UNITY_EDITOR
		networkName += " (Editor)";
#endif
		return options.SetOption("Comm", "AppNetworkName", AkOption_Comm.AkOption_Comm_AppNetworkName, networkName);
	}

	private static List<AkOptionNamespace> InjectPluginPath(List<AkOptionNamespace> options)
	{
		// The native plugin DLL directory (formerly AkInitSettings.szPluginDLLPath) is computed at runtime
		// per-platform by AkBasePathGetter.GetPluginPath and exposed via AkBasePathGetter.Get().PluginPath.
		var pluginPath = AkBasePathGetter.Get().PluginPath;
		if (string.IsNullOrEmpty(pluginPath))
		{
			return options;
		}

		return options.SetOption("SoundEngine", "PluginPath", AkOption_SoundEngine.AkOption_SoundEngine_PluginPath, pluginPath);
	}

	protected virtual void LoadInitBank()
	{
		AkBankManager.LoadInitBank();
	}

	protected virtual void ClearBanks()
	{
		AkUnitySoundEngine.ClearBanks();
	}

	protected virtual void ResetBanks()
	{
		AkBankManager.Reset();
	}


	public bool ResetSoundEngine(bool isInPlayMode)
	{
		if (isInPlayMode && AkUnitySoundEngine.IsInitialized())
		{
			ClearBanks();
			LoadInitBank();
		}

		var callbackManagerInitSettings = new AkCallbackManager.InitializationSettings
		{
			IsLoggingEnabled = WwiseRuntimeSettings.IsEngineLoggingEnabled
		};
		AkCallbackManager.Init(callbackManagerInitSettings);

		PostResetDelegate?.InvokeUnitySafe();
		return true;
	}

	public bool ShouldKeepSoundEngineEnabled()
	{
#if UNITY_EDITOR
		bool result = true;
		if(UnityEditor.EditorApplication.isUpdating || UnityEditor.EditorApplication.isCompiling)
		{
			return false;
		}

		if (UnityEngine.Application.isPlaying)
		{
			return true;
		}
		if (!UnityEngine.Application.isPlaying)
		{
			result = AkWwiseEditorSettings.Instance.LoadSoundEngineInEditMode;
		}
#if UNITY_2019_3_OR_NEWER
		if(UnityEditor.EditorSettings.enterPlayModeOptionsEnabled)
		{
			result &= UnityEditor.EditorSettings.enterPlayModeOptions.HasFlag(UnityEditor.EnterPlayModeOptions.DisableDomainReload);
		}
		else
		{
			result = false;
		}
#endif // UNITY_2019_3_OR_NEWER
		return result;
#else
		return false;
#endif // UNITY_EDITOR
	}

	public void ResetSoundEngine()
    {
		TerminateSoundEngine(forceReset : true);
		if(ShouldKeepSoundEngineEnabled())
		{
			ResetSoundEngine(isInPlayMode : true);
			InitializeSoundEngine();
		}
    }

	public void TerminateSoundEngine()
    {
		TerminateSoundEngine(forceReset : false);
    }

	private void TerminateSoundEngine(bool forceReset)
	{
		if (!AkUnitySoundEngine.IsInitialized())
		{
			return;
		}

		if (ShouldKeepSoundEngineEnabled() && !forceReset)
		{
			return;
		}
		
		PreTerminationDelegate?.InvokeUnitySafe();
		AkUnitySoundEngine.Term();

		// Make sure we have no callbacks left after Term. Some might be posted during termination.
		AkCallbackManager.PostCallbacks();

		AkCallbackManager.Term();
		ResetBanks();

		WwiseLogger.Log("Sound engine terminated successfully.");

		PostTerminationDelegate?.InvokeUnitySafe();
	}
}

#if WWISE_ADDRESSABLES_23_1_OR_LATER
[System.Obsolete(AkUnitySoundEngine.Ak_Sound_Engine_Init_Rename_2024_1_0)]
public class AkSoundEngineInitialization  : AkUnitySoundEngineInitialization {}
#endif
