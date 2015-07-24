using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Nanarchy.Core.Helpers
{

    public static class Json
    {
        static readonly Lazy<JsonSerializerSettings> _serializerSettings =
            new Lazy<JsonSerializerSettings>(() =>
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                };
                return settings;
            });

        public static string Serialize(object data, bool withTypeInfo = false)
        {
            return JsonConvert.SerializeObject(data, GetJsonSettings(withTypeInfo));
        }

        public static T Deserialize<T>(string json, bool withTypeInfo = false)
        {
            return JsonConvert.DeserializeObject<T>(json, GetJsonSettings(withTypeInfo));
        }

        public static object Deserialize(string json, Type type, bool withTypeInfo = false)
        {
            return JsonConvert.DeserializeObject(json, type, GetJsonSettings(withTypeInfo));
        }

        public static string ToJson(this object data, bool withTypeInfo = false)
        {
            return Serialize(data, withTypeInfo);
        }

        public static T FromJson<T>(this string data, bool withTypeInfo = false)
        {
            return Deserialize<T>(data, withTypeInfo);
        }

        public static string ToBase64JsonString(this object o)
        {
            var json = o.ToJson();
            var bytes = Encoding.UTF8.GetBytes(json);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        public static T FromBase64JsonString<T>(this string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            var json = Encoding.UTF8.GetString(bytes);
            return json.FromJson<T>();
        }

        public static string ToJsonIndented(this object data, bool withTypeInfo = false)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented, GetJsonSettings(withTypeInfo));
        }

        static JsonSerializerSettings GetJsonSettings(bool withTypeInfo)
        {
            if (!withTypeInfo) return _serializerSettings.Value;
            
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore
            };
            return settings;
        }
    }
}