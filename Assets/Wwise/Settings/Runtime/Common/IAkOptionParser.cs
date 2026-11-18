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

namespace Wwise.SDK
{
    /// <summary>
    /// Thrown when an <see cref="IAkOptionParser"/> fails to parse a JSON string.
    /// </summary>
    public class AkOptionParseException : Exception
    {
        public AkOptionParseException(string message) : base(message) { }
        public AkOptionParseException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Config for serializing AkOption JSON strings.
    /// </summary>
    public class AkOptionParserConfig
    {
        /// <summary>
        /// When true, Doc strings and AllowedValues are omitted from the serialized output.
        /// </summary>
        public bool SkipDoc = true;

        /// <summary>
        /// When true, keys are omitted from the serialized output.
        /// </summary>
        public bool SkipKeys = false;

        /// <summary>
        /// When true, the MaxLength field is omitted from the serialized output.
        /// </summary>
        public bool SkipMaxLength = true;

        /// <summary>
        /// When true, Type is omitted from the serialized output.
        /// </summary>
        public bool SkipType = false;

        /// <summary>
        /// When true, namespace-level metadata (KeyStart, NumOptions, ConditionalMacro) is omitted from the serialized output.
        /// </summary>
        public bool SkipNamespaceMetadata = true;
    }

    /// <summary>
    /// Interface for parsing and serializing AkOption metadata JSON files.
    /// </summary>
    public interface IAkOptionParser
    {
        /// <summary>
        /// Deserialize an AkOptions JSON string into a AkOptionNamespaces
        /// </summary>
        /// <param name="json">JSON string from an AkOptions metadata file.</param>
        /// <returns>Namespaces containing the AkOptions (e.g. "SoundEngine", "MemoryMgr").</returns>
        /// <exception cref="AkOptionParseException">Thrown if the input is null or not valid JSON.</exception>
        List<AkOptionNamespace> Parse(string json);

        /// <summary>
        /// Serialize a list of option namespaces back to JSON.
        /// </summary>
        /// <param name="namespaces">The namespaces to serialize.</param>
        /// <param name="config">Controls what is included in the output.</param>
        /// <returns>A JSON string representing the option namespaces.</returns>
        string ToJson(List<AkOptionNamespace> namespaces, AkOptionParserConfig config);

        /// <summary>
        /// Serialize a list of option namespaces back to JSON using the default <see cref="AkOptionParserConfig"/>.
        /// </summary>
        /// <param name="namespaces">The namespaces to serialize.</param>
        /// <returns>A JSON string representing the option namespaces.</returns>
        string ToJson(List<AkOptionNamespace> namespaces);

        /// <summary>
        /// Whether two JSON strings represent structurally equivalent content
        /// (key order and whitespace ignored).
        /// </summary>
        bool AreEquivalent(string jsonA, string jsonB);
    }
}