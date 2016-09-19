﻿// <copyright file="DefaultValueObjectEqualityComparer.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The default value object equality comparer.
    /// </summary>
    /// <typeparam name="T">The type of value object.</typeparam>
    public sealed class DefaultValueObjectEqualityComparer<T> : IEqualityComparer<ValueObject<T>>
        where T : ValueObject<T>
    {
        private static readonly MethodInfo CastToObjects = MakeEnumerableCastMethod(typeof(object));
        private static readonly MethodInfo ObjectsEqual = MakeEnumerableSequenceEqualMethod(typeof(object));
        private static readonly MethodInfo ObjectEqualityOperator = GetDeclaredEqualityOperatorOrDefault(typeof(object));

        private readonly Func<ValueObject<T>, ValueObject<T>, bool> equalsMethod;
        private readonly Func<ValueObject<T>, int> hashCodeMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueObjectEqualityComparer{T}"/> class.
        /// </summary>
        public DefaultValueObjectEqualityComparer()
        {
            var type = typeof(T);

            var left = Expression.Parameter(typeof(ValueObject<T>), "left");
            var right = Expression.Parameter(typeof(ValueObject<T>), "right");

            var body = type.GetProperties()
                .Select(property => Generate(
                    property.PropertyType,
                    Expression.Property(Expression.Convert(left, type), property),
                    Expression.Property(Expression.Convert(right, type), property)))
                .Aggregate((Expression)Expression.Constant(true), (current, expression) => Expression.AndAlso(current, expression));

            this.equalsMethod = Expression.Lambda<Func<ValueObject<T>, ValueObject<T>, bool>>(body, left, right).Compile();
            
            var obj = Expression.Parameter(typeof(object), "obj");
            var body2 = type.GetProperties()
                .Select(property => Expression.Call(
                    this.GetType().GetMethod("GetHashCodeOrDefault", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(property.PropertyType),
                    Expression.Property(Expression.TypeAs(obj, type), property)))
                .Aggregate(
                    (Expression)Expression.Constant(17),
                    (expression, propertyExpression) => Expression.Add(Expression.Multiply(expression, Expression.Constant(23)), propertyExpression));
             
            this.hashCodeMethod = Expression.Lambda<Func<ValueObject<T>, int>>(body2, obj).Compile();
        }

        /// <summary>
        /// Determines whether the specified value objects are equal.
        /// </summary>
        /// <param name="x">The first value object to compare.</param>
        /// <param name="y">The second value object to compare.</param>
        /// <returns>Returns <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(ValueObject<T> x, ValueObject<T> y)
        {
            return this.equalsMethod(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified value object.
        /// </summary>
        /// <param name="obj">The value object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the value object, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public int GetHashCode(ValueObject<T> obj)
        {
            Guard.Against.Null(() => obj);

            return unchecked(this.hashCodeMethod(obj));
        }

        private static Expression Generate(Type type, Expression left, Expression right)
        {
            var equalityOperator = ObjectEqualityOperator;

            for (; type != typeof(object) && type.BaseType != null; type = type.BaseType)
            {
                var method = GetDeclaredEqualityOperatorOrDefault(type);
                if (method != null)
                {
                    equalityOperator = method;
                }
            }            

            // TODO (Adam): Optimize for IList. See http://stackoverflow.com/a/486781/49241
            if (equalityOperator == null && typeof(IEnumerable).IsAssignableFrom(type))
            {
                var interfaces = type.IsInterface
                    ? type.GetInterfaces().Union(new[] { type }).ToArray()
                    : type.GetInterfaces();

                var genericInterfaces = interfaces
                    .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .ToArray();

                if (genericInterfaces.Any())
                {
                    return genericInterfaces
                        .Select(@interface => Expression.Call(
                            MakeEnumerableSequenceEqualMethod(@interface.GetGenericArguments()[0]),
                            Expression.Convert(left, @interface),
                            Expression.Convert(right, @interface)))
                        .Aggregate((Expression)Expression.Constant(true), (current, expression) => Expression.AndAlso(current, expression));
                }

                return Expression.Equal(Expression.Call(CastToObjects, left), Expression.Call(CastToObjects, right), false, ObjectsEqual);
            }

            return Expression.Equal(left, right, false, equalityOperator);
        }

        private static int GetHashCodeOrDefault<T1>(T1 value)
        {
            return object.Equals(value, default(T1)) ? 0 : value.GetHashCode();
        }

        private static MethodInfo MakeEnumerableCastMethod(Type type)
        {
            return typeof(EnumerableProxy).GetMethod("Cast").MakeGenericMethod(type);
        }

        private static MethodInfo MakeEnumerableSequenceEqualMethod(Type type)
        {
            return typeof(EnumerableProxy).GetMethod("SequenceEqual").MakeGenericMethod(type);
        }

        private static MethodInfo GetDeclaredEqualityOperatorOrDefault(Type type)
        {
            return type.GetMethods()
                .Where(candidate => candidate.Name == "op_Equality")
                .FirstOrDefault(candidate =>
                {
                    var parameters = candidate.GetParameters();
                    return parameters.Length == 2 && parameters[0].ParameterType == type && parameters[1].ParameterType == type;
                });
        }

        private static class EnumerableProxy
        {
            public static IEnumerable<TResult> Cast<TResult>(IEnumerable source)
            {
                return source.Cast<TResult>();
            }

            public static bool SequenceEqual<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second)
            {
                return first.SequenceEqual(second);
            }
        }
    }
}
