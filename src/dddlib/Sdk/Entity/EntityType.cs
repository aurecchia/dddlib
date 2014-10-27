﻿// <copyright file="EntityType.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using dddlib.Sdk;

    internal class EntityType
    {
        // TODO (Cameron): Mess.
        public EntityType(Type runtimeType, NaturalKeySelector naturalKeySelector, IEqualityComparer<string> naturalKeyStringEqualityComparer)
        {
            Guard.Against.Null(() => runtimeType);

            if (!typeof(Entity).IsAssignableFrom(runtimeType))
            {
                throw new RuntimeException(string.Format(CultureInfo.InvariantCulture, "The specified type '{0}' is not an entity.", runtimeType));
            }

            if (naturalKeySelector != null && naturalKeySelector.RuntimeType != runtimeType)
            {
                throw new RuntimeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The specified natural key selector '{0}' does not match the specified runtime type '{1}'.",
                        naturalKeySelector.RuntimeType,
                        runtimeType));
            }

            this.NaturalKeySelector = naturalKeySelector ?? NaturalKeySelector.Undefined;
            this.NaturalKeyEqualityComparer = naturalKeyStringEqualityComparer == null 
                ? (IEqualityComparer<object>)EqualityComparer<object>.Default
                : new StringObjectEqualityComparer(naturalKeyStringEqualityComparer);
        }

        public NaturalKeySelector NaturalKeySelector { get; private set; }

        public IEqualityComparer<object> NaturalKeyEqualityComparer { get; private set; }

        private class StringObjectEqualityComparer : IEqualityComparer<object>
        {
            private readonly IEqualityComparer<string> stringEqualityComparer;

            public StringObjectEqualityComparer(IEqualityComparer<string> stringEqualityComparer)
            {
                this.stringEqualityComparer = stringEqualityComparer;
            }

            public new bool Equals(object x, object y)
            {
                return this.stringEqualityComparer.Equals((string)x, (string)y);
            }

            public int GetHashCode(object obj)
            {
                return this.stringEqualityComparer.GetHashCode((string)obj);
            }
        }
    }
}
