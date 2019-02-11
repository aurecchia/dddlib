﻿// <copyright file="DefaultNaturalKeySerializer.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Persistence.Sdk
{
    using System;
    using dddlib.Runtime;
    using dddlib.Sdk;

    /// <summary>
    /// Represents the default natural key serializer.
    /// </summary>
    public class DefaultNaturalKeySerializer : INaturalKeySerializer
    {
        private static readonly IJsonSerializer Serializer = new JavaScriptSerializer();

        static DefaultNaturalKeySerializer()
        {
        }

        /// <summary>
        /// Serializes the specified natural key.
        /// </summary>
        /// <param name="naturalKeyType">The type of natural key.</param>
        /// <param name="naturalKey">The natural key.</param>
        /// <returns>The serialized natural key.</returns>
        public string Serialize(Type naturalKeyType, object naturalKey)
        {
            Guard.Against.Null(() => naturalKeyType);

            if (naturalKeyType.IsSubclassOfRawGeneric(typeof(ValueObject<>)))
            {
                var runtimeType = Application.Current.GetValueObjectType(naturalKeyType);
                return runtimeType.Serializer.Serialize(naturalKey);
            }

            return Serializer.Serialize(naturalKey);
        }

        /// <summary>
        /// Deserializes the specified serialized natural key.
        /// </summary>
        /// <param name="naturalKeyType">The type of natural key.</param>
        /// <param name="serializedNaturalKey">The serialized natural key.</param>
        /// <returns>The natural key.</returns>
        public object Deserialize(Type naturalKeyType, string serializedNaturalKey)
        {
            Guard.Against.Null(() => naturalKeyType);

            if (naturalKeyType.IsSubclassOfRawGeneric(typeof(ValueObject<>)))
            {
                var runtimeType = Application.Current.GetValueObjectType(naturalKeyType);
                return runtimeType.Serializer.Deserialize(serializedNaturalKey);
            }

            return Serializer.Deserialize(serializedNaturalKey, naturalKeyType);
        }
    }
}
