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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AK.Wwise.Unity.Settings.Editor.Common.UI
{
    /// <summary>
    /// Bridges AkOption changes into Unity's native Undo history. A transient, non-persisted ScriptableObject proxy gives Undo.RegisterCompleteObjectUndo something real to snapshot.
    /// The actual option data is replayed through the caller-supplied applyChanges delegate when undoRedoPerformed fires.
    /// </summary>
    /// <remarks>
    /// <see cref="Push"/> is the only entry point callers use to record history. There is intentionally no matching "Pop".
    /// Traversal (undo AND redo) is never invoked directly. It happens only as a reaction to Unity's own
    /// <see cref="Undo.undoRedoPerformed"/> event, fired when the user presses Ctrl+Z / Ctrl+Y on the proxy.
    /// </remarks>
    internal class WwiseOptionUndoRedo : IDisposable
    {
        private class HistoryEntry
        {
            public List<WwiseOptionChange> Changes;
        }

        private readonly Action<List<WwiseOptionChange>, bool> _applyChanges;
        private readonly UndoProxy _proxy;
        private readonly List<HistoryEntry> _history = new List<HistoryEntry>();
        private int _currentIndex = -1;

        public WwiseOptionUndoRedo(Action<List<WwiseOptionChange>, bool> applyChanges)
        {
            _applyChanges = applyChanges;
            _proxy = ScriptableObject.CreateInstance<UndoProxy>();
            _proxy.hideFlags = HideFlags.DontSave;
            _proxy.Version = -1;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            _proxy.Disabled += () => Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        /// <summary>
        /// Records <paramref name="changes"/> as one new undoable step. Called by whoever owns this instance
        /// each time it wants a set of option edits to become a single Ctrl+Z/Ctrl+Y step.
        /// Like any undo stack, pushing after an undo discards the redo branch. Any steps ahead of the
        /// current position are dropped, so they can no longer be reached by redoing.
        /// </summary>
        public void Push(List<WwiseOptionChange> changes, string description)
        {
            if (changes == null || changes.Count == 0)
            {
                return;
            }

            if (_currentIndex + 1 < _history.Count)
            {
                _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
            }

            _history.Add(new HistoryEntry { Changes = changes });
            Undo.RegisterCompleteObjectUndo(_proxy, description);
            _proxy.Version = _history.Count - 1;
            _currentIndex = _proxy.Version;
        }

        private void OnUndoRedoPerformed()
        {
            var targetIndex = _proxy.Version;
            while (_currentIndex > targetIndex)
            {
                _applyChanges(_history[_currentIndex].Changes, false);
                _currentIndex--;
            }
            while (_currentIndex < targetIndex)
            {
                _currentIndex++;
                _applyChanges(_history[_currentIndex].Changes, true);
            }
        }

        public void Dispose()
        {
            if (_proxy == null)
            {
                return;
            }

            Undo.ClearUndo(_proxy);
            UnityEngine.Object.DestroyImmediate(_proxy);
        }
    }
}
