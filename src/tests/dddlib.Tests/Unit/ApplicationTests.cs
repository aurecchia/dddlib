﻿// <copyright file="ApplicationTests.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Tests.Unit
{
    using System;
    using dddlib.Runtime;
    using dddlib.Sdk.Configuration.Model;
    using dddlib.Tests.Sdk;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    /*  TODO (Cameron):
        Test for ObjectDisposedException
        Test caching of type descriptors  */

    public class ApplicationTests
    {
        [Fact]
        public void CanCreateApplication()
        {
            // arrange
            var defaultApplication = Application.Current;
            using (new Application())
            {
                // act
                var currentApplication = Application.Current;

                // assert
                currentApplication.Should().NotBe(defaultApplication);
            }
        }

        [Fact]
        public void CanDisposeApplication()
        {
            // arrange
            var defaultApplication = Application.Current;
            using (new Application())
            {
            }

            // act
            var currentApplication = Application.Current;

            // assert
            currentApplication.Should().Be(defaultApplication);
        }

        [Fact]
        public void CanDisposeApplicationMultipleTimes()
        {
            // arrange
            var application = new Application();
            
            // act
            application.Dispose();
            Action action = () => application.Dispose();

            // assert
            action.Should().NotThrow();
        }

        [Fact]
        public void CanNestApplication()
        {
            // arrange
            var defaultApplication = Application.Current;
            using (new Application())
            {
                var firstApplication = Application.Current;
                using (new Application())
                {
                    var secondApplication = Application.Current;

                    // assert
                    firstApplication.Should().NotBe(defaultApplication);
                    secondApplication.Should().NotBe(defaultApplication);
                    firstApplication.Should().NotBe(secondApplication);
                }
            }
        }

        [Fact]
        public void CannotDisposeDefaultApplication()
        {
            // arrange
            var defaultApplication = Application.Current;

            // act
            ((IDisposable)defaultApplication).Dispose();

            // assert
            defaultApplication.Should().Be(Application.Current);
        }

        [Fact(Skip = "Internals not exposed any more... not quite sure what to do here?")]
        public void ApplicationCanCreateRuntimeTypeForValidType()
        {
            // arrange
            var type = typeof(Aggregate);

            var typeAnalyzerService = A.Fake<ITypeAnalyzerService>(o => o.Strict());
            A.CallTo(() => typeAnalyzerService.IsValidAggregateRoot(type)).Returns(true);
            A.CallTo(() => typeAnalyzerService.IsValidEntity(type)).Returns(true);
            A.CallTo(() => typeAnalyzerService.GetUninitializedFactory(type)).Returns(null);
            A.CallTo(() => typeAnalyzerService.GetNaturalKey(type)).Returns(null);
            A.CallTo(() => typeAnalyzerService.IsValidAggregateRoot(typeof(AggregateRoot))).Returns(true);
            A.CallTo(() => typeAnalyzerService.IsValidEntity(typeof(AggregateRoot))).Returns(true);
            A.CallTo(() => typeAnalyzerService.GetUninitializedFactory(typeof(AggregateRoot))).Returns(null);
            A.CallTo(() => typeAnalyzerService.GetNaturalKey(typeof(AggregateRoot))).Returns(null);
            A.CallTo(() => typeAnalyzerService.IsValidEntity(typeof(Entity))).Returns(true);
            A.CallTo(() => typeAnalyzerService.GetUninitializedFactory(typeof(Entity))).Returns(null);
            A.CallTo(() => typeAnalyzerService.GetNaturalKey(typeof(Entity))).Returns(null);
            var entityType = new EntityType(typeof(Entity), typeAnalyzerService);
            var aggregateType = new AggregateRootType(typeof(AggregateRoot), typeAnalyzerService, entityType);
            var expectedType = new AggregateRootType(type, typeAnalyzerService, aggregateType);
            var factory = A.Fake<Func<Type, AggregateRootType>>(o => o.Strict());
            A.CallTo(() => factory.Invoke(type)).Returns(expectedType);

            /*
            using (new Application(factory, t => null, t => null))
            {
                // act
                var actualType = Application.Current.GetAggregateRootType(type);

                // assert
                actualType.Should().Be(expectedType);
            }
            */
        }

        [Fact(Skip = "Internals not exposed any more... not quite sure what to do here?")]
        public void ApplicationThrowsRuntimeExceptionOnFactoryException()
        {
            // arrange
            var innerException = new Exception();
            var factory = A.Fake<Func<Type, AggregateRootType>>(o => o.Strict());
            A.CallTo(() => factory.Invoke(A<Type>.Ignored)).Throws(innerException);

            /*
            using (new Application(factory, t => null, t => null))
            {
                // act
                Action action = () => Application.Current.GetAggregateRootType(typeof(Aggregate));

                // assert
                action.Should().Throw<RuntimeException>().And.InnerException.Should().Be(innerException);
            }
            */
        }

        [Fact(Skip = "Internals not exposed any more... not quite sure what to do here?")]
        public void ApplicationThrowsRuntimeExceptionOnFactoryRuntimeException()
        {
            // arrange
            var runtimeException = new RuntimeException();
            var factory = A.Fake<Func<Type, AggregateRootType>>(o => o.Strict());
            A.CallTo(() => factory.Invoke(A<Type>.Ignored)).Throws(runtimeException);

            /*
            using (new Application(factory, t => null, t => null))
            {
                // act
                Action action = () => Application.Current.GetAggregateRootType(typeof(Aggregate));

                // assert
                action.Should().Throw<RuntimeException>().And.Should().Be(runtimeException);
            }
            */
        }

        private class Aggregate : AggregateRoot
        {
        }
    }
}
