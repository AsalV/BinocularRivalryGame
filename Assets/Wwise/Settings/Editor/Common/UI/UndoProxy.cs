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

namespace AK.Wwise.Unity.Settings.Editor.Common.UI
{
    /// <summary>
    /// Transient ScriptableObject that <see cref="WwiseOptionUndoRedo"/> registers with the Unity Undo system.
    /// </summary>
    internal class UndoProxy : ScriptableObject
    {
        public int Version;
        public event Action Disabled;

        private void OnDisable()
        {
            Disabled?.Invoke();
        }
    }
}
