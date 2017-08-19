using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FeralNerd.IoC;

namespace FeralNerd.IoC.Tests
{
    [TestClass]
    public class IoCContainerTests
    {
        [TestMethod]
        public void MakeAnInstance()
        {
            IoCContainer ioc = new IoCContainer();

            Assert.IsTrue(ioc._registeredTypes.Count == 0, "Count of registered types should be zero.");
        }

        [TestMethod]
        public void MakeAnUnknownTypeFails()
        {
            IoCContainer ioc = new IoCContainer();

            try
            {
                IColor color = ioc.Resolve<IColor>();
                Assert.Fail("Creating an unregistered type should have failed.");
            }
            catch (IoCContainerException)
            {
            }
        }

        [TestMethod]
        public void RegisterAType()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>();

            Assert.IsTrue(ioc._registeredTypes.Count == 1, "Count of registered types should be 1.");
        }

        [TestMethod]
        public void CannotRegisterSameTypeTwice()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>();

            Assert.IsTrue(ioc._registeredTypes.Count == 1, "Count of registered types should be 1.");

            try
            {
                ioc.Register<IColor, BlueObject>();
                Assert.Fail("Registering another IColor should have failed.");
            }
            catch (IoCContainerException)
            {
            }
        }

        [TestMethod]
        public void ResolveSimpleType()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>();

            IColor obj = ioc.Resolve<IColor>();

            Assert.IsNotNull(obj, "Container should have resolved IColor");
            Assert.AreEqual("Red", obj.Color, "Color should be \"Red\".");
        }

        [TestMethod]
        public void ResolveMultipleTransientInstances()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>();

            IColor obj1 = ioc.Resolve<IColor>();
            IColor obj2 = ioc.Resolve<IColor>();

            Assert.IsNotNull(obj1, "Container should have resolved first instance of IColor");
            Assert.IsNotNull(obj2, "Container should have resolved second instance of IColor");
            Assert.AreEqual("Red", obj1.Color, "Color of first object should be \"Red\".");
            Assert.AreEqual("Red", obj2.Color, "Color of second object should be \"Red\".");

            Assert.IsFalse(Object.ReferenceEquals(obj1, obj2), "Both objects should be separate instances.");
        }

        [TestMethod]
        public void ResolveSingletonInstance()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, RedObject>(LifecycleType.Singleton);

            IColor obj1 = ioc.Resolve<IColor>();
            IColor obj2 = ioc.Resolve<IColor>();

            Assert.IsNotNull(obj1, "Container should have resolved first instance of IColor");
            Assert.IsNotNull(obj2, "Container should have resolved second instance of IColor");
            Assert.AreEqual("Red", obj1.Color, "Color of first object should be \"Red\".");
            Assert.AreEqual("Red", obj2.Color, "Color of second object should be \"Red\".");

            Assert.IsTrue(Object.ReferenceEquals(obj1, obj2), "Both objects should be separate instances.");
        }

        [TestMethod]
        public void ConcreteTypeMustImplementInterface()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IColor, BadClass_NoInterface>();
                Assert.Fail("Registering IColor->BadObject should have failed.");
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void CannotRegesterConcreteTypeAsInterface()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<BadClass_NoInterface, RedObject>();
                Assert.Fail("IType must be an interface type.");
            }
            catch (IoCContainerException ex)
            {
               Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void RegisteredConcreteTypeMustBeClass()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IColor, IColor>();
                Assert.Fail("CType must be a class.");
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ResolveConcreteClassDefaultCtor()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IColor, YellowObject>();

            IColor obj = ioc.Resolve<IColor>();

            Assert.IsNotNull(obj, "Container should have resolved IColor");
            Assert.AreEqual("Yellow", obj.Color, "Color should be \"Yellow\".");
        }

        [TestMethod]
        public void PrivateDefaultCtor()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IColor, BadClass_PrivateCtor>();
                Assert.Fail("CType must have a public ctor.");
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ResolveClassWithOneDependency()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IFoo, Foo>();
            ioc.Register<IDoStuff, OneDependency>();

            IDoStuff thingDoer = ioc.Resolve<IDoStuff>();

            Assert.AreEqual(13 * 13, thingDoer.MakeAThing());
        }

        [TestMethod]
        public void ResolveClassWithTwoDependencies()
        {
            IoCContainer ioc = new IoCContainer();
            ioc.Register<IFoo, Foo>();
            ioc.Register<IBar, Bar>();
            ioc.Register<IDoStuff, TwoDependencies>();

            IDoStuff thingDoer = ioc.Resolve<IDoStuff>();

            Assert.AreEqual(13 + 11, thingDoer.MakeAThing());
        }

        [TestMethod]
        public void CannotResolveClassWithBadCtorArgs()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IColor, BadClass_BadCtor>();
                Assert.Fail("Should not be able to register a class that has a ctor with a param that is not an interface.");
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void CannotResolveClasWithCtorWithUnregisteredType()
        {
            try
            {
                IoCContainer ioc = new IoCContainer();
                ioc.Register<IDoStuff, OneDependency>();
                Assert.Fail("Should not be able to register class OneDependency before registering a class that implements IFoo.");
            }
            catch (IoCContainerException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
