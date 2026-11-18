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
using UnityEngine.UIElements;

namespace AK.Wwise.Unity.Settings.Editor.Common.UI
{
    /// <summary>
    /// Index from model identity (platform, namespace, option) to the elements rendering it.
    /// Lets a change notification be applied to exactly the affected cells without walking the
    /// element tree.
    /// Populated lazily since namespace foldouts only build their rows on first expand.
    /// </summary>
    internal class WwiseSettingsViewIndex
    {
        private readonly Dictionary<(string platformId, OptionKey key), VisualElement> _cells =
            new Dictionary<(string, OptionKey), VisualElement>();
        private readonly Dictionary<OptionKey, (VisualElement labelRow, VisualElement cellRow)> _rows =
            new Dictionary<OptionKey, (VisualElement, VisualElement)>();
        private readonly Dictionary<string, Foldout> _namespaceFoldouts = new Dictionary<string, Foldout>();
        private readonly Dictionary<string, List<VisualElement>> _columns = new Dictionary<string, List<VisualElement>>();

        public IReadOnlyDictionary<OptionKey, (VisualElement labelRow, VisualElement cellRow)> Rows => _rows;
        public IReadOnlyDictionary<string, Foldout> NamespaceFoldouts => _namespaceFoldouts;

        public void AddNamespace(string namespaceName, Foldout foldout)
        {
            _namespaceFoldouts[namespaceName] = foldout;
        }

        /// <param name="cellRow">Null for single-platform rows, which have no separate label column.</param>
        public void AddRow(string namespaceName, string optionName,
                           VisualElement labelRow, VisualElement cellRow)
        {
            _rows[new OptionKey(namespaceName, optionName)] = (labelRow, cellRow);
        }

        /// <summary>Record the editor widget for one cell</summary>
        public void AddCell(string platformId, string namespaceName, string optionName, VisualElement widget)
        {
            _cells[(platformId, new OptionKey(namespaceName, optionName))] = widget;
        }

        public bool TryGetCell(string platformId, OptionKey key, out VisualElement widget)
        {
            return _cells.TryGetValue((platformId, key), out widget);
        }

        public bool TryGetNamespaceFoldout(string namespaceName, out Foldout foldout)
        {
            return _namespaceFoldouts.TryGetValue(namespaceName, out foldout);
        }

        /// <summary>Record a header or cell in a platform's column, shown and hidden with that column.</summary>
        public void AddColumnElement(string platformId, VisualElement element)
        {
            if (!_columns.TryGetValue(platformId, out var elements))
            {
                elements = new List<VisualElement>();
                _columns[platformId] = elements;
            }
            elements.Add(element);
        }

        public IReadOnlyList<VisualElement> GetColumn(string platformId)
        {
            return _columns.TryGetValue(platformId, out var elements) ? elements : System.Array.Empty<VisualElement>();
        }
    }
}
