using CUE4Parse.UE4.Objects.Core.Serialization;
using CUE4Parse.UE4.Objects.Core.Misc;
using Newtonsoft.Json;
using System.Text;

namespace UnrealReZen.Core.Helpers
{
    public static class JsonHelpers
    {
        public static IEnumerable<FCustomVersion>? ReadCustomVersionsJson(string? path)
        {
            if (path == null) return null;

            var serializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
            });

            bool passed = false;
            List<JsonFCustomVersion>? list = null;


            Encoding[] encodings = [Encoding.Default, Encoding.Unicode];
            foreach (var encoding in encodings)
            {
                try
                {
                    Console.Error.WriteLine($"Reading {path} with {encoding.EncodingName} encoding.");
                    using var stream = new StreamReader(path, encoding);
                    var reader = new JsonTextReader(stream);
                    list = serializer.Deserialize<List<JsonFCustomVersion>>(reader);

                    return list?.Select(x => (FCustomVersion)x);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error reading {path} with {encoding.EncodingName} encoding.");
                    Console.Error.WriteLine(ex.Message);
                }
            }

            throw new JsonReaderException($"Could not read {path} with any known encodings");
        }

        private struct JsonFCustomVersion
        {
            [JsonRequired] public string Key;
            [JsonRequired] public int Version;
            public string? FriendlyName;

            public static implicit operator FCustomVersion(JsonFCustomVersion x)
            {
                return new FCustomVersion(new FGuid(x.Key.Replace("-", "")), x.Version);
            }
        }
    }
}
