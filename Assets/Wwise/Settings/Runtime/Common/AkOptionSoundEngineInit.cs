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

#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AK.Wwise.Unity.Logging;
using Wwise.SDK;

namespace AK.Wwise.Unity.Settings
{
    /// <summary>
    /// Initialize the Sound Engine using the AkOptions API.
    /// </summary>
    public class AkOptionSoundEngineInit
    {
        /// <summary>
        /// Reset the Options to their default values
        /// </summary>
        public static void ResetAkOptions()
        {
            AkOptionController.AK_Option_ResetToDefaults();
        }

        /// <summary>
        /// Initialize the Wwise Sound Engine using the specified AkOption init settings.
        /// Use <see cref="ResetAkOptions"/> to obtain a clean slate of options prior to this call.
        /// </summary>
        /// <param name="optionNamespaces">The runtime options for the Sound Engine. Only overrides need to be passed, Wwise will otherwise use the default values for each option.</param>
        /// <returns>The <see cref="AKRESULT"/> code from Wwise, or <see cref="AKRESULT.AK_AlreadyInitialized"/> if already initialized.</returns>
        public static AKRESULT Init(List<AkOptionNamespace> optionNamespaces)
        {
            if (AkSoundEngine.AK_SoundEngine_IsInitialized())
            {
                WwiseLogger.Warning("Sound Engine is already initialized.");
                return AKRESULT.AK_AlreadyInitialized;
            }

            foreach (var (namespaceName, optionName, option) in optionNamespaces.AllOptionsWithNames())
            {
                if (option.DefaultValue != null)
                {
                    if (option.Key == null)
                    {
                        WwiseLogger.LogFormat(LogLevel.Warning,
                            "Initialization: Namespace <{0}>. Could not set option <{1}> with value <{2}>. No key was provided. Result: <{3}>",
                            namespaceName, optionName, option.DefaultValue,
                            Enum.GetName(typeof(AKRESULT), AKRESULT.AK_InvalidID));
                        continue;
                    }

                    var setOptionResult = SetOption(namespaceName, optionName, (uint)option.Key, option.Type, option.DefaultValue);
                    if (setOptionResult != AKRESULT.AK_Success)
                    {
                        WwiseLogger.LogFormat(LogLevel.Error,
                            "Initialization: Namespace <{0}>. Could not set option <{1}> with value <{2}>. Result: <{3}>",
                            namespaceName, optionName, option.DefaultValue,
                            Enum.GetName(typeof(AKRESULT), setOptionResult));
                    }
                    else
                    {
                        WwiseLogger.LogFormat(LogLevel.VeryVerbose,
                            "Initialization: Namespace <{0}>: Set option <{1}> with value <{2}>", namespaceName,
                            optionName, option.DefaultValue.ToString());
                    }
                }
            }

            if (AkMemoryMgr.AK_MemoryMgr_IsInitialized())
            {
                WwiseLogger.Warning("Memory Manager is already initialized.");
            }

            var memMgrResult = AkMemoryMgr.AK_MemoryMgr_Init();
            if (memMgrResult != AKRESULT.AK_Success)
            {
                return memMgrResult;
            }
            else
            {
                return AkSoundEngine.AK_SoundEngine_Init();
            }
        }

        private static AKRESULT SetOption(string namespaceName, string optionName, uint key, string? type, object value)
        {
            switch (type)
            {
                case "Integer":
                case "ChannelConfig":
                {
                    var optionValue = Convert.ToInt64(value);
                    return AkOptionController.AK_Option_SetI(key, optionValue);
                }
                case "Boolean":
                {
                    var optionValue = Convert.ToBoolean(value);
                    return AkOptionController.AK_Option_SetB(key, optionValue);
                }
                case "Real":
                {
                    var optionValue = Convert.ToDouble(value);
                    return AkOptionController.AK_Option_SetF(key, optionValue);
                }
                case "String":
                {
                    var strValue = value as string;
                    var strPtr = Marshal.StringToCoTaskMemUTF8(strValue);
                    try
                    {
                        var result = AkOptionController.AK_Option_SetS(key, strPtr);
                        // Strings that exceed the option's storage are truncated by the Sound Engine.
                        if (result == AKRESULT.AK_Success)
                        {
                            var storedValue = AkOptionController.AK_Option_GetS(key);
                            if (storedValue != strValue)
                            {
                                WwiseLogger.LogFormat(LogLevel.Warning,
                                    "Initialization: Namespace <{0}>. Option <{1}> with value <{2}> is too long and was truncated to <{3}>.",
                                    namespaceName, optionName, strValue, storedValue);
                            }
                        }
                        return result;
                    }
                    catch (Exception e)
                    {
                        WwiseLogger.LogFormat(LogLevel.Error,
                            "Initialization: Namespace <{0}>. Could not set option <{1}> with value <{2}>.\n{3}",
                            namespaceName, optionName, value, e.Message);
                        return AKRESULT.AK_Fail;
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(strPtr);
                    }
                }
            }

            return AKRESULT.AK_InvalidParameter;
        }
    }
}