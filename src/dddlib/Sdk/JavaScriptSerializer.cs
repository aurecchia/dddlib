using System;
using System.Collections.Concurrent;

namespace dddlib.Sdk
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    // NOTE (Alessio): see https://thedevstop.wordpress.com/2012/04/09/from-javascriptserializer-to-jsonnet/
    // TODO (Alessio): Thi has to potentially be removed/simplified
    public class JavaScriptSerializer : IJsonSerializer
    {
        private static readonly ConcurrentDictionary<string, Type> ResolvedTypes = new ConcurrentDictionary<string, Type>();
        private JsonSerializerSettings _settings = new JsonSerializerSettings();

        public JavaScriptSerializer()
        {
//            this._settings.Converters.Add(new JsonInt32Converter());
        }

        public void RegisterConverters(IEnumerable<IJsonConverter> converters)
        {
            foreach (IJsonConverter converter in converters)
                _settings.Converters.Add(new JavaScriptConverter(this, converter));
        }

        public object ConvertToType(IDictionary<string, object> dictionary, Type type)
        {
            string intermediate = JsonConvert.SerializeObject(dictionary);
            return JsonConvert.DeserializeObject(intermediate, type);
        }

        public T ConvertToType<T>(IDictionary<string, object> dictionary)
        {
            string intermediate = JsonConvert.SerializeObject(dictionary);
            T value = JsonConvert.DeserializeObject<T>(intermediate);
            return value;
        }

        public object Deserialize(string input, Type type)
        {
            return JsonConvert.DeserializeObject(input, type, _settings);
        }

        public T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input, _settings);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, _settings);
        }

        public string DefaultSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public Type GetType(string typeName)
        {
            return ResolvedTypes.GetOrAdd(typeName, key => Type.GetType(key));
        }
    }
}