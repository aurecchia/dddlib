﻿// <copyright file="IEntityConfigurationWrapper.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using dddlib.Sdk;

#pragma warning disable 1591
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not visible in editor.")]
    public interface IEntityConfigurationWrapper<T> : IEntityConfigurationWrapper<IEntityConfigurationWrapper<T>, T>, IFluentExtensions
        where T : Entity
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not visible in editor.")]
    public interface IEntityConfigurationWrapper<TConfiguration, T> : IFluentExtensions
        where T : Entity
        where TConfiguration : IEntityConfigurationWrapper<TConfiguration, T>
    {
#pragma warning restore 1591
        /// <summary>
        /// Configures the runtime to assign the natural key of the entity using the specified natural key selector.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="naturalKeySelector">The natural key selector.</param>
        /// <returns>The configuration.</returns>
        TConfiguration ToUseNaturalKey<TKey>(Expression<Func<T, TKey>> naturalKeySelector);

        /// <summary>
        /// Configures the runtime to map the entity to the specified event type.
        /// </summary>
        /// <typeparam name="TEvent">The event type to map to.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>The configuration.</returns>
        TConfiguration ToMapToEvent<TEvent>(Action<T, TEvent> mapping);

        /// <summary>
        /// Configures the runtime to map the entity to the specified event type.
        /// </summary>
        /// <typeparam name="TEvent">The event type to map to.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <param name="reverseMapping">The reverse mapping.</param>
        /// <returns>The configuration.</returns>
        TConfiguration ToMapToEvent<TEvent>(Action<T, TEvent> mapping, Func<TEvent, T> reverseMapping);
    }
}
