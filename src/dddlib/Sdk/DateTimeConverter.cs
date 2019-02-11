// <copyright file="DateTimeConverter.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

#if DDDLIB
namespace dddlib.Sdk
#elif DISPATCHER
namespace dddlib.Persistence.EventDispatcher.Sdk
#elif PROJECTIONS
namespace dddlib.Projections.Sdk
#endif
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the <see cref="System.DateTime"/> converter for JavaScript serialization.
    /// </summary>
    /// <seealso cref="JavaScriptConverter" />
    //// LINK (Cameron): http://blog.calyptus.eu/seb/2011/12/custom-datetime-json-serialization/
    public class DateTimeConverter : JsonConverter
    {
        private static readonly IJsonSerializer Serializer = new JavaScriptSerializer();

        /// <summary>
        /// Gets a collection of the supported types.
        /// </summary>
        /// <value>The supported types.</value>
        public IEnumerable<Type> SupportedTypes
        {
            get { return new[] { typeof(DateTime) }; }
        }

        /// <summary>
        /// Converts the provided dictionary into an object of the specified type.
        /// </summary>
        /// <param name="dictionary">An <see cref="T:System.Collections.Generic.IDictionary`2" /> instance of property data stored as name/value pairs.</param>
        /// <param name="type">The type of the resulting object.</param>
        /// <param name="serializer">The <see cref="T:dddlib.Sdk.JavaScriptSerializer" /> instance.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(IDictionary<string, object> dictionary, Type type, IJsonSerializer serializer)
        {
            return Serializer.ConvertToType(dictionary, type);
        }

        /// <summary>
        /// Builds a dictionary of name/value pairs.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The object that is responsible for the serialization.</param>
        /// <returns>An object that contains key/value pairs that represent the object’s data.</returns>
        public IDictionary<string, object> Serialize(object obj, IJsonSerializer serializer)
        {
            return null;
//            return obj is DateTime
//                ? new DateTimeString((DateTime)obj)
//                : null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
