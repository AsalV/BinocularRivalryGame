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

#if AK_NEWTONSOFT_JSON

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Wwise.SDK
{

    internal class NewtonsoftAkOptionsParser: IAkOptionParser
    {
        private class AkOptionFile
        {
            public int Version;
            public List<AkOptionNamespace> Namespaces = new List<AkOptionNamespace>();
        }

        public List<AkOptionNamespace> Parse(string json)
        {
            if (json == null)
                throw new AkOptionParseException("Input JSON string is null.");

            try
            {
                // First check the Version before parsing the entire tree.
                var version = PeekVersion(json)
                    ?? throw new AkOptionParseException("Missing Version field.");
                var namespaces = version switch
                {
                    1 => JsonConvert.DeserializeObject<AkOptionFile>(json)?.Namespaces
                        ?? throw new AkOptionParseException("Missing Namespaces field."),
                    _ => throw new AkOptionParseException($"Unsupported Options.json version: {version}")
                };

                // Use the Type field to make sure the Options' values are the correct type.
                foreach (var opt in namespaces.SelectMany(n => n.Options))
                {
                    opt.MinValue = ConvertType(opt.MinValue, opt.Type);
                    opt.MaxValue = ConvertType(opt.MaxValue, opt.Type);
                    opt.DefaultValue = ConvertType(opt.DefaultValue, opt.Type);
                }
                return namespaces;
            }
            catch (JsonException ex)
            {
                throw new AkOptionParseException("Failed to parse options JSON.", ex);
            }
        }

        private static int? PeekVersion(string json)
        {
            using var reader = new JsonTextReader(new StringReader(json));
            if (!reader.Read() || reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }
            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                if ((string)reader.Value! == "Version")
                {
                    return reader.ReadAsInt32();
                }
                reader.Read();
                reader.Skip();
            }
            return null;
        }

        private static object? ConvertType(object? value, string? type)
        {
            if (value == null)
            {
                return null;
            }
            return type switch
            {
                "Integer" or "ChannelConfig" => (object)Convert.ToInt64(value),
                "Real" => (object)Convert.ToDouble(value),
                _ => value
            };
        }

        public string ToJson(List<AkOptionNamespace> namespaces, AkOptionParserConfig config)
        {
            var file = new AkOptionFile { Version = 1, Namespaces = namespaces };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new AkOptionsContractResolver(config),
                Converters = { new PointerFilteringConverter(), new StringEnumConverter() }
            };
            return JsonConvert.SerializeObject(file, Formatting.Indented, settings);
        }

        public string ToJson(List<AkOptionNamespace> namespaces)
        {
            return ToJson(namespaces, new AkOptionParserConfig());
        }

        public bool AreEquivalent(string jsonA, string jsonB)
        {
            return JToken.DeepEquals(JToken.Parse(jsonA), JToken.Parse(jsonB));
        }

        private class AkOptionsContractResolver : DefaultContractResolver
        {
            private readonly HashSet<string> _excluded;

            public AkOptionsContractResolver(AkOptionParserConfig config)
            {
                _excluded = new HashSet<string>();
                if (config.SkipDoc)
                {
                    _excluded.Add(nameof(AkOptionNamespace.Doc));
                    _excluded.Add(nameof(AkOptionEntry.AllowedValues));
                    _excluded.Add(nameof(AkOptionAllowedValue.Doc));
                }
                if (config.SkipKeys)
                {
                    _excluded.Add(nameof(AkOptionEntry.Key));
                }
                if (config.SkipMaxLength)
                {
                    _excluded.Add(nameof(AkOptionEntry.MaxLength));
                }
                if (config.SkipType)
                {
                    _excluded.Add(nameof(AkOptionEntry.Type));
                }
                if (config.SkipNamespaceMetadata)
                {
                    _excluded.Add(nameof(AkOptionNamespace.KeyStart));
                    _excluded.Add(nameof(AkOptionNamespace.NumOptions));
                    _excluded.Add(nameof(AkOptionNamespace.ConditionalMacro));
                }
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return base.CreateProperties(type, memberSerialization)
                    .Where(p => p.UnderlyingName != null && !_excluded.Contains(p.UnderlyingName))
                    .ToList();
            }
        }

        private class PointerFilteringConverter : JsonConverter<List<AkOptionEntry>>
        {
            public override List<AkOptionEntry>? ReadJson(JsonReader r, Type t, List<AkOptionEntry>? e, bool h, JsonSerializer s) => throw new NotSupportedException();

            public override void WriteJson(JsonWriter writer, List<AkOptionEntry>? value, JsonSerializer serializer)
            {
                writer.WriteStartArray();
                foreach (var entry in value!)
                {
                    if (entry.Type == "Pointer") continue;
                    serializer.Serialize(writer, entry);
                }
                writer.WriteEndArray();
            }
        }
    }
}
#endif
