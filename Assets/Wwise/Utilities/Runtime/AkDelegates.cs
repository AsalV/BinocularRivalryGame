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
using System;
using UnityEngine;
using AK.Wwise.Unity.Logging;

public static class AkDelegates
{
    /// <summary>
    /// Invokes a parameterless Action, skipping destroyed UnityEngine.Object targets and
    /// swallowing exceptions from subscribers so one bad subscriber cannot crash the publisher.
    /// </summary>
    /// <param name="action">The Action to invoke.</param>
    public static void InvokeUnitySafe(this System.Action action)
    {
        if (action == null) return;

        foreach (Delegate del in action.GetInvocationList())
        {
            if (del.Target is UnityEngine.Object unityTarget && unityTarget == null)
            {
                continue;
            }

            try
            {
                ((Action)del).Invoke();
            }
            catch (Exception ex)
            {
                WwiseLogger.LogFormat(LogLevel.Error, "Exception during safe Invoke. Method: {0}. Error: {1}", del.Method.Name, ex.Message);
            }
        }
    }

    public static void InvokeUnitySafe<T>(this Action<T> action, T arg)
    {
        if (action == null) return;

        foreach (Delegate del in action.GetInvocationList())
        {
            if (del.Target is UnityEngine.Object unityTarget && unityTarget == null)
            {
                continue;
            }

            try
            {
                ((Action<T>)del).Invoke(arg);
            }
            catch (Exception ex)
            {
                WwiseLogger.LogFormat(LogLevel.Error, "Exception during safe Invoke. Method: {0}. Error: {1}", del.Method.Name, ex.Message);
            }
        }
    }
}