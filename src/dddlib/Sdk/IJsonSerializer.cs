
namespace dddlib.Sdk
{
    using System;
    using System.Collections.Generic;

    public interface IJsonSerializer
    {
        void RegisterConverters(IEnumerable<IJsonConverter> converters);
        // TODO (Alessio): Eventually remove this and change the code to use the generic method
        object ConvertToType(IDictionary<string, object> dictionary, Type type);
        T ConvertToType<T>(IDictionary<string, object> dictionary);
        object Deserialize(string input, Type type);
        T Deserialize<T>(string input);
        string Serialize(object obj);
        string DefaultSerialize(object obj);
        Type GetType(string typeName);
    }
}