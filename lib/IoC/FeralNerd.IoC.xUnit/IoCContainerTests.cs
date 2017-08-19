using System;
using System.Linq;
using Xunit;

namespace FeralNerd.IoC.xUnit
{
    public class IoCContainerTests
    {
        [Fact]
        public void MakeAnInstance()
        {
            IoCContainer ioc = new IoCContainer();

            Assert.True(ioc._registeredTypes.Count == 0);
        }

        [Fact]
        public void MakeAnUnknownTypeFails()
        {
            IoCContainer ioc = new IoCContainer();

            try
            {
                IColor color = ioc.Resolve<IColor>();
                Assert.False(true);
            }
            catch (IoCContainerException)
            {
            }
        }

        [Fact]
        public void RegisterAType()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>();

            Assert.True(ioc._registeredTypes.Count == 1);
        }

        [Fact]
        public void CannotRegisterSameTypeTwice()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>();

            Assert.True(ioc._registeredTypes.Count == 1);

            try
            {
                ioc.Register<IColor, BlueObject>();
                Assert.False(true);
            }
            catch (IoCContainerException)
            {
            }
        }

        [Fact]
        public void ResolveSimpleType()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>();

            IColor obj = ioc.Resolve<IColor>();

            Assert.NotNull(obj);
            Assert.Equal("Red", obj.Color);
        }

        [Fact]
        public void ResolveMultipleTransientInstances()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>();

            IColor obj1 = ioc.Resolve<IColor>();
            IColor obj2 = ioc.Resolve<IColor>();

            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            Assert.Equal("Red", obj1.Color);
            Assert.Equal("Red", obj2.Color);

            Assert.False(Object.ReferenceEquals(obj1, obj2));
        }

        [Fact]
        public void ResolveSingletonInstance()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>(LifecycleType.Singleton);

            IColor obj1 = ioc.Resolve<IColor>();
            IColor obj2 = ioc.Resolve<IColor>();

            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            Assert.Equal("Red", obj1.Color);
            Assert.Equal("Red", obj2.Color);

            Assert.True(Object.ReferenceEquals(obj1, obj2));
        }

        [Fact]
        public void ConcreteTypeMustImplementInterface()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IColor, BadClass_NoInterface>();
                Assert.False(true);
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Fact]
        public void CannotRegesterConcreteTypeAsInterface()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<BadClass_NoInterface, RedObject>();
                Assert.False(true);
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Fact]
        public void RegisteredConcreteTypeMustBeClass()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IColor, IColor>();
                Assert.False(true);
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Fact]
        public void ResolveConcreteClassDefaultCtor()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, YellowObject>();

            IColor obj = ioc.Resolve<IColor>();

            Assert.NotNull(obj);
            Assert.Equal("Yellow", obj.Color);
        }

        [Fact]
        public void PrivateDefaultCtor()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IColor, BadClass_PrivateCtor>();
                Assert.False(true);
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Fact]
        public void ResolveClassWithOneDependency()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IFoo, Foo>();
            ioc.Register<IDoStuff, OneDependency>();

            IDoStuff thingDoer = ioc.Resolve<IDoStuff>();

            Assert.Equal(13 * 13, thingDoer.MakeAThing());
        }

        [Fact]
        public void ResolveClassWithTwoDependencies()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IFoo, Foo>();
            ioc.Register<IBar, Bar>();
            ioc.Register<IDoStuff, TwoDependencies>();

            IDoStuff thingDoer = ioc.Resolve<IDoStuff>();

            Assert.Equal(13 + 11, thingDoer.MakeAThing());
        }

        [Fact]
        public void CannotResolveClassWithBadCtorArgs()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IColor, BadClass_BadCtor>();
                Assert.False(true);
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Fact]
        public void CannotResolveClasWithCtorWithUnregisteredType()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IDoStuff, OneDependency>();
                Assert.False(true);
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
