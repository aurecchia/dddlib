namespace dddlib.Sdk
{
    using System;
    using System.Collections.Generic;

    public interface IJsonConverter
    {
        object Deserialize(IDictionary<string, object> dictionary, Type type, IJsonSerializer serializer);
        IDictionary<string, object> Serialize(object obj, IJsonSerializer serializer);
        IEnumerable<Type> SupportedTypes { get; }
    }
}