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

namespace Wwise.SDK
{
    internal class NullOptionParser: IAkOptionParser
    {
        public List<AkOptionNamespace> Parse(string json)
        {
            return new();
        }

        public string ToJson(List<AkOptionNamespace> namespaces, AkOptionParserConfig config)
        {
            return "{\"Version\": 1, \"Namespaces\": []}";
        }

        public string ToJson(List<AkOptionNamespace> namespaces)
        {
            return "{\"Version\": 1, \"Namespaces\": []}";
        }

        public bool AreEquivalent(string jsonA, string jsonB)
        {
            return true;
        }
    }
}