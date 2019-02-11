// <copyright file="AssertionExtensions.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Tests.Sdk
{
    using dddlib.Sdk;

    public static class AssertionExtensions
    {
        private static readonly IJsonSerializer Serializer = new JavaScriptSerializer();

        public static void ShouldMatch(this object actualValue, object expectedValue)
        {
            var actual = Serializer.Serialize(actualValue);
            var expected = Serializer.Serialize(expectedValue);

            if (actual != expected)
            {
                throw new FluentAssertions.Execution.AssertionFailedException("Mementos don't match.");
            }
        }
    }
}
