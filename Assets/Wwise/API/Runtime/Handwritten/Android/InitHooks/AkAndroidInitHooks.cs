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

#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using AK.Wwise.Unity.Logging;
using UnityEngine;
using Wwise.SDK;

namespace Wwise.API.Runtime.Handwritten.Android
{
    public class AkAndroidInitHooks
    {
        private static IntPtr androidActivityGlobalRef;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void RegisterAndroidInitMethod()
        {
            // We don't unsubscribe, these delegates exist for the entire runtime
            AkUnitySoundEngineInitialization.Instance.PreInitializationDelegate += PreInit;
            AkUnitySoundEngineInitialization.Instance.PostTerminationDelegate += AfterTermination;
        }

        private static void ReleaseActivityGlobalRef()
        {
            if (androidActivityGlobalRef != IntPtr.Zero)
            {
                AndroidJNI.DeleteGlobalRef(androidActivityGlobalRef);
                androidActivityGlobalRef = IntPtr.Zero;
            }
        }

        private static void AfterTermination()
        {
            ReleaseActivityGlobalRef();
        }

        private static void PreInit(List<AkOptionNamespace> options)
        {
            // Cleanup existing reference. It should already be IntPtr.Zero, as we should have terminated and cleaned up the reference, but just in case.
            ReleaseActivityGlobalRef();

            //Obtains the Android Java Object "currentActivity" in order to set it for the android io hook initialization
            try
            {
                // Get the current Activity using AndroidJavaClass
                using (var unityPlayer = new UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    // Get the current JVM using AndroidJavaClass
                    var jvm = AndroidJNI.GetJavaVM();
                    AkOptionController.AK_Option_SetP((uint)AkOption_Android.AkOption_Android_JavaVM, jvm);

                    // Get the current Activity using AndroidJavaClass
                    var localActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity").GetRawObject();
                    androidActivityGlobalRef = AndroidJNI.NewGlobalRef(localActivity);
                    AkOptionController.AK_Option_SetP((uint)AkOption_Android.AkOption_Android_ActivityReference,
                        androidActivityGlobalRef);
                }
            }
            catch (Exception ex)
            {
                WwiseLogger.LogFormat(LogLevel.Error, "Failed to pass Android activity to native code: {0}", ex.Message);
            }
        }
    }
}
#endif