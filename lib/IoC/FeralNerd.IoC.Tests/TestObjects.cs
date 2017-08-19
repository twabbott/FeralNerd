using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralNerd.IoC.Tests
{
    interface IColor
    {
        string Color { get; }
    }

    class RedObject : IColor
    {
        public string Color
        {
            get
            {
                return "Red";
            }
        }
    }

    class BlueObject : IColor
    {
        public string Color
        {
            get
            {
                return "Blue";
            }
        }
    }

    class YellowObject: IColor
    {
        public YellowObject()
        {
        }

        public string Color
        {
            get
            {
                return "Yellow";
            }
        }
    }

    class BadClass_NoInterface
    {
        public string Color
        {
            get
            {
                return "This object does not implement IColor, and IoCContainer should reject it.";
            }
        }
    }

    class BadClass_PrivateCtor: IColor
    {
        private BadClass_PrivateCtor()
        {
            Console.WriteLine("It's built!");
        }

        public string Color
        {
            get
            {
                return "Foobar";
            }
        }

        static BadClass_PrivateCtor Factory()
        {
            return new BadClass_PrivateCtor();
        }
    }

    class BadClass_BadCtor: IColor
    {
        public BadClass_BadCtor(YellowObject obj)
        {
            Console.WriteLine(obj.Color);
        }

        public string Color
        {
            get
            {
                return "This object has an unusable ctor.";
            }
        }
    }


    interface IFoo
    {
        int GetValue();
    }
        
    interface IBar
    {
        int GetValue();
    }

    interface IDoStuff
    {
        int MakeAThing();
    }

    class Foo : IFoo
    {
        public int GetValue()
        {
            return 13;
        }
    }

    class Bar : IBar
    {
        public int GetValue()
        {
            return 11;
        }
    }

    class OneDependency : IDoStuff
    {
        private IFoo _foo;

        public OneDependency(IFoo foo)
        {
            _foo = foo;
        }

        public int MakeAThing()
        {
            return _foo.GetValue() * _foo.GetValue();
        }
    }

    class TwoDependencies: IDoStuff
    {
        private IFoo _foo;
        private IBar _bar;

        public TwoDependencies(IFoo foo, IBar bar)
        {
            _foo = foo;
            _bar = bar;
        }

        public int MakeAThing()
        {
            return _foo.GetValue() + _bar.GetValue();
        }
    }
}
