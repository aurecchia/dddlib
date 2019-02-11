﻿// <copyright file="EntityEquality.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Tests.Feature
{
    using System;
    using System.Collections.Generic;
    using dddlib.Configuration;
    using dddlib.Runtime;
    using dddlib.Tests.Sdk;
    using dddlib.Tests.Support;
    using FluentAssertions;
    using Xbehave;

    // As someone who uses dddlib
    // In order to persist an aggregate root
    // I need to be able to perform equality operations against entities (as an aggregate root is an entity)
    public abstract class EntityEquality : Feature
    {
        public class UndefinedNaturalKeySelector : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2)
            {
                "Given an entity with an undefined natural key selector"
                    .x(() => { });

                "When two instances of that entity are instantiated"
                    .x(() =>
                    {
                        instance1 = new Subject();
                        instance2 = new Subject();
                    });

                "Then the first instance is not equal to the second instance"
                    .x(() => instance1.Should().NotBe(instance2));
            }

            public class Subject : Entity
            {
            }
        }

        public class NaturalKeySelectorDefinedInMetadata : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given an entity with a natural key selector defined in metadata"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that entity are instantiated with that natural key value assigned"
                    .x(() =>
                    {
                        instance1 = new Subject { NaturalKey = naturalKey };
                        instance2 = new Subject { NaturalKey = naturalKey };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class Subject : Entity
            {
                [NaturalKey]
                public string NaturalKey { get; set; }
            }
        }

        public class NaturalKeySelectorDefinedInBootstrapper : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given an entity with a natural key selector defined in the bootstrapper"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that entity are instantiated with that natural key value assigned"
                    .x(() =>
                    {
                        instance1 = new Subject { NaturalKey = naturalKey };
                        instance2 = new Subject { NaturalKey = naturalKey };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class Subject : Entity
            {
                public string NaturalKey { get; set; }
            }

            private class BootStrapper : IBootstrap<Subject>
            {
                public void Bootstrap(IConfiguration configure)
                {
                    configure.Entity<Subject>().ToUseNaturalKey(subject => subject.NaturalKey);
                }
            }
        }

        public class NonConflictingNaturalKeySelectors : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given an entity with non-conflicting natural key selectors defined in both metadata and the bootstrapper"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that entity are instantiated with that natural key value assigned"
                    .x(() =>
                    {
                        instance1 = new Subject { NaturalKey = naturalKey };
                        instance2 = new Subject { NaturalKey = naturalKey };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class Subject : Entity
            {
                [NaturalKey]
                public string NaturalKey { get; set; }
            }

            private class BootStrapper : IBootstrap<Subject>
            {
                public void Bootstrap(IConfiguration configure)
                {
                    configure.Entity<Subject>().ToUseNaturalKey(subject => subject.NaturalKey);
                }
            }
        }

        public class ConflictingNaturalKeySelectors : EntityEquality
        {
            [Scenario]
            public void Scenario(Type type, Action action)
            {
                "Given an entity with conflicting natural key selectors defined in both metadata and the bootstrapper"
                    .x(() => { });

                "When an instance of that entity is instantiated"
                    .x(() => action = () => new Subject());

                "Then a runtime exception should be thrown" // and the runtime exception should state that the natural key is defined twice
                    .x(() => action.Should().Throw<RuntimeException>());
            }

            public class Subject : Entity
            {
                [NaturalKey]
                public string FirstNaturalKey { get; set; }

                public string SecondNaturalKey { get; set; }
            }

            private class BootStrapper : IBootstrap<Subject>
            {
                public void Bootstrap(IConfiguration configure)
                {
                    configure.Entity<Subject>().ToUseNaturalKey(subject => subject.SecondNaturalKey);
                }
            }
        }

        public class CaseSensitiveUndefinedEqualityComparer : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given a value object with an undefined equality comparer"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that value object that are instantiated with different values"
                    .x(() =>
                    {
                        instance1 = new Subject { NaturalKey = naturalKey.ToUpperInvariant() };
                        instance2 = new Subject { NaturalKey = naturalKey.ToLowerInvariant() };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().NotBe(instance2));
            }

            public class Subject : Entity
            {
                [NaturalKey]
                public string NaturalKey { get; set; }
            }
        }

        // NOTE (Cameron): I made a choice here to force introduction of a type to address non-primitive equality.
        // LINK (Cameron): http://blog.ploeh.dk/2015/01/19/from-primitive-obsession-to-domain-modelling/
        public class CaseInsensitiveEqualityComparerDefinedInBootstrapper : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given a value object with an undefined equality comparer"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that value object that are instantiated with different values"
                    .x(() =>
                    {
                        instance1 = new Subject { NaturalKey = new Key { Value = naturalKey.ToUpperInvariant() } };
                        instance2 = new Subject { NaturalKey = new Key { Value = naturalKey.ToLowerInvariant() } };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class Key : ValueObject<Key>
            {
                public string Value { get; set; }
            }

            public class Subject : Entity
            {
                public Key NaturalKey { get; set; }
            }

            private class BootStrapper : IBootstrap<Subject>, IBootstrap<Key>
            {
                public void Bootstrap(IConfiguration configure)
                {
                    configure.Entity<Subject>().ToUseNaturalKey(subject => subject.NaturalKey);
                    configure.ValueObject<Key>().ToUseEqualityComparer(new KeyEqualityComparer());
                }

                private class KeyEqualityComparer : IEqualityComparer<Key>
                {
                    public bool Equals(Key x, Key y)
                    {
                        return string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
                    }

                    public int GetHashCode(Key obj)
                    {
                        return obj.GetHashCode();
                    }
                }
            }
        }

        public class CompositeNaturalKeyEqualityComparer : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string component1, string component2)
            {
                "Given a value object with an undefined equality comparer"
                    .x(() => { });

                "And a natural key value"
                    .x(() =>
                    {
                        component1 = "key1";
                        component2 = "key2";
                    });

                "When two instances of that value object that are instantiated with different values"
                    .x(() =>
                    {
                        var naturalKey1 = new NaturalKeyValue { Component1 = component1, Component2 = component2 };
                        var naturalKey2 = new NaturalKeyValue { Component1 = component1, Component2 = component2 };

                        instance1 = new Subject { NaturalKey = naturalKey1 };
                        instance2 = new Subject { NaturalKey = naturalKey2 };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class NaturalKeyValue : ValueObject<NaturalKeyValue>
            {
                public string Component1 { get; set; }

                public string Component2 { get; set; }
            }

            public class Subject : Entity
            {
                [NaturalKey]
                public NaturalKeyValue NaturalKey { get; set; }
            }
        }

        public class UndefinedNaturalKeySelectorWithInheritance : EntityEquality
        {
            [Scenario]
            public void Scenario(SuperSubject instance1, SuperSubject instance2)
            {
                "Given an entity with an undefined natural key selector"
                    .x(() => { });

                "When two instances of that entity are instantiated"
                    .x(() =>
                    {
                        instance1 = new SuperSubject();
                        instance2 = new SuperSubject();
                    });

                "Then the first instance is not equal to the second instance"
                    .x(() => instance1.Should().NotBe(instance2));
            }

            public class Subject : Entity
            {
            }

            public class SuperSubject : Subject
            {
            }
        }

        public class NaturalKeySelectorDefinedInBaseClass : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given an entity with a natural key selector defined in the base class"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that entity are instantiated with that natural key value assigned"
                    .x(() =>
                    {
                        instance1 = new SuperSubject { NaturalKey = naturalKey };
                        instance2 = new SuperSubject { NaturalKey = naturalKey };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class Subject : Entity
            {
                [NaturalKey]
                public string NaturalKey { get; set; }
            }

            public class SuperSubject : Subject
            {
            }
        }

        public class NaturalKeySelectorDefinedISubclass : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given an entity with a natural key selector defined in the subclass"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that entity are instantiated with that natural key value assigned"
                    .x(() =>
                    {
                        instance1 = new SuperSubject { NaturalKey = naturalKey };
                        instance2 = new SuperSubject { NaturalKey = naturalKey };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class Subject : Entity
            {
            }

            public class SuperSubject : Subject
            {
                [NaturalKey]
                public string NaturalKey { get; set; }
            }
        }

        public class NaturalKeySelectorDefinedInBothBaseClassAndSubclass : EntityEquality
        {
            [Scenario]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given an entity with a natural key selector defined in the base class and the subclass"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that entity are instantiated with the subclass natural key value assigned"
                    .x(() =>
                    {
                        instance1 = new SuperSubject { NaturalKey = "unequalValue", NaturalKey2 = naturalKey };
                        instance2 = new SuperSubject { NaturalKey = "unequalValue2", NaturalKey2 = naturalKey };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class Subject : Entity
            {
                [NaturalKey]
                public string NaturalKey { get; set; }
            }

            public class SuperSubject : Subject
            {
                [NaturalKey]
                public string NaturalKey2 { get; set; }
            }
        }

        public class InheritedNaturalKeySelector : EntityEquality
        {
            [Scenario]
            public void Scenario(Registration registration, Car car, Car otherCar, IRegistrationService registrationService)
            {
                "Given a registration service"
                    .x(() => registrationService = new RegistrationService());

                "And a registration"
                    .x(() => registration = new Registration("abc", registrationService));

                "When two cars are instantiated with that registration"
                    .x(() =>
                    {
                        car = new Car(registration);
                        otherCar = new Car(registration);
                    });

                "Then the cars are equal"
                    .x(() => car.Should().Be(otherCar));
            }

            public class Car : Vehicle
            {
                public Car(Registration registration)
                    : base(registration)
                {
                }
            }

            public class RegistrationService : IRegistrationService
            {
                public bool ConfirmValid(string registrationNumber)
                {
                    return true;
                }
            }
        }

        public class InheritedNaturalKeySelectorOveriddenInSubclass : EntityEquality
        {
            [Scenario]
            public void Scenario(Registration registration, Registration otherRegistration, Car car, Car otherCar, IRegistrationService registrationService)
            {
                "Given a registration service"
                    .x(() => registrationService = new RegistrationService());

                "And a registration"
                    .x(() => registration = new Registration("abc", registrationService));

                "And a registration with a number that differs by case"
                    .x(() => otherRegistration = new Registration("ABC", registrationService));

                "When two cars are instantiated with those registrations"
                    .x(() =>
                    {
                        car = new Car(registration);
                        otherCar = new Car(otherRegistration);
                    });

                "Then the cars are equal"
                    .x(() => car.Should().NotBe(otherCar));
            }

            public class Car : Vehicle
            {
                public Car(Registration registration)
                    : base(registration)
                {
                    this.OtherRegistration = registration.Number;
                }

                [NaturalKey]
                public string OtherRegistration { get; set; }
            }

            public class RegistrationService : IRegistrationService
            {
                public bool ConfirmValid(string registrationNumber)
                {
                    return true;
                }
            }
        }

        public class InheritedNaturalKeySelectorOveriddenInBootstrapper : EntityEquality
        {
            [Scenario]
            public void Scenario(Registration registration, Registration otherRegistration, Car car, Car otherCar, IRegistrationService registrationService)
            {
                "Given a registration service"
                    .x(() => registrationService = new RegistrationService());

                "And a registration"
                    .x(() => registration = new Registration("abc", registrationService));

                "And a registration with a number that differs by case"
                    .x(() => otherRegistration = new Registration("ABC", registrationService));

                "When two cars are instantiated with those registrations"
                    .x(() =>
                    {
                        car = new Car(registration);
                        otherCar = new Car(otherRegistration);
                    });

                "Then the cars are equal"
                    .x(() => car.Should().NotBe(otherCar));
            }

            public class Car : Vehicle
            {
                public Car(Registration registration)
                    : base(registration)
                {
                    this.OtherRegistration = registration.Number;
                }

                public string OtherRegistration { get; set; }
            }

            public class RegistrationService : IRegistrationService
            {
                public bool ConfirmValid(string registrationNumber)
                {
                    return true;
                }
            }

            private class BootStrapper : IBootstrap<Car>
            {
                public void Bootstrap(IConfiguration configure)
                {
                    configure.Entity<Car>().ToUseNaturalKey(car => car.OtherRegistration);
                }
            }
        }

        // NOTE (Cameron): It doesn't make sense to support nested natural key definition in metadata so this is the only way.
        public class NestedNaturalKeySelector : EntityEquality
        {
            [Scenario(Skip = "https://github.com/dddlib/dddlib/issues/72")]
            public void Scenario(Subject instance1, Subject instance2, string naturalKey)
            {
                "Given an entity with a nested natural key selector"
                    .x(() => { });

                "And a natural key value"
                    .x(() => naturalKey = "key");

                "When two instances of that entity are instantiated"
                    .x(() =>
                    {
                        instance1 = new Subject { Details = new Subject.SubjectDetails { NaturalKey = naturalKey } };
                        instance2 = new Subject { Details = new Subject.SubjectDetails { NaturalKey = naturalKey } };
                    });

                "Then the first instance is equal to the second instance"
                    .x(() => instance1.Should().Be(instance2));
            }

            public class Subject : Entity
            {
                public SubjectDetails Details { get; set; }

                public class SubjectDetails
                {
                    public string NaturalKey { get; set; }
                }
            }

            private class BootStrapper : IBootstrap<Subject>
            {
                public void Bootstrap(IConfiguration configure)
                {
                    configure.Entity<Subject>().ToUseNaturalKey(subject => subject.Details.NaturalKey);
                }
            }
        }

        public class NestedNaturalKeySelectorWithSingleInstanceHavingNullReference : EntityEquality
        {
            [Scenario(Skip = "https://github.com/dddlib/dddlib/issues/72")]
            public void Scenario(Subject instance1, Subject instance2)
            {
                "Given an entity with a nested natural key selector"
                    .x(() => { });

                "When two instances of that entity are instantiated (one with no defined natural key object)"
                    .x(() =>
                    {
                        instance1 = new Subject();
                        instance2 = new Subject { Details = new Subject.SubjectDetails { NaturalKey = "key" } };
                    });

                "Then the first instance is not equal to the second instance"
                    .x(() => instance1.Should().NotBe(instance2));
            }

            public class Subject : Entity
            {
                public SubjectDetails Details { get; set; }

                public class SubjectDetails
                {
                    public string NaturalKey { get; set; }
                }
            }

            private class BootStrapper : IBootstrap<Subject>
            {
                public void Bootstrap(IConfiguration configure)
                {
                    configure.Entity<Subject>().ToUseNaturalKey(subject => subject.Details.NaturalKey);
                }
            }
        }

        public class NestedNaturalKeySelectorWithBothInstancesHavingNullReference : EntityEquality
        {
            [Scenario(Skip = "https://github.com/dddlib/dddlib/issues/72")]
            public void Scenario(Subject instance1, Subject instance2)
            {
                "Given an entity with a nested natural key selector"
                    .x(() => { });

                "When two instances of that entity are instantiated (one with no defined natural key object)"
                    .x(() =>
                    {
                        instance1 = new Subject();
                        instance2 = new Subject();
                    });

                "Then the first instance is not equal to the second instance"
                    .x(() => instance1.Should().NotBe(instance2));
            }

            public class Subject : Entity
            {
                public SubjectDetails Details { get; set; }

                public class SubjectDetails
                {
                    public string NaturalKey { get; set; }
                }
            }

            private class BootStrapper : IBootstrap<Subject>
            {
                public void Bootstrap(IConfiguration configure)
                {
                    configure.Entity<Subject>().ToUseNaturalKey(subject => subject.Details.NaturalKey);
                }
            }
        }
    }
}
