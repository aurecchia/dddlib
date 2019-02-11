namespace dddlib.Sdk
{
    using System;
    using System.Collections.Generic;

    // NOTE (Alessio): see https://thedevstop.wordpress.com/2012/04/09/from-javascriptserializer-to-jsonnet/
    // TODO (Alessio): Thi has to potentially be removed/simplified
    public interface IJsonConverter
    {
        object Deserialize(IDictionary<string, object> dictionary, Type type, IJsonSerializer serializer);
        IDictionary<string, object> Serialize(object obj, IJsonSerializer serializer);
        IEnumerable<Type> SupportedTypes { get; }
    }
}