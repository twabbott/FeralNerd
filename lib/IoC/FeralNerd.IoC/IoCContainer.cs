using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FeralNerd.IoC
{
    public enum LifecycleType
    {
        Transient,
        Singleton
    }

    public class IoCContainer
    {
        internal struct ConcreteTypeInfo
        {
            public Type Type;
            public LifecycleType Lifecycle;
            public ParameterInfo[] ConstructorArgs;
        }

        /// <summary>
        ///     Set of registered types.  Marked as internal so it can be unit-tested.
        /// </summary>
        internal Dictionary<Type, ConcreteTypeInfo> _registeredTypes = new Dictionary<Type, ConcreteTypeInfo>();

        private Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

        public IType Resolve<IType>()
            where  IType: class
        {
            return __FetchInstance(typeof(IType)) as IType;
        }

        private object __FetchInstance(Type oftype)
        {
            ConcreteTypeInfo concreteType;
            if (!_registeredTypes.TryGetValue(oftype, out concreteType))
                throw new IoCContainerException("Interface \"{0}\" has not been registered with container.", oftype);

            // Fetch all ctor args.  Yes, this is a recursive operation.
            object[] args = new object[concreteType.ConstructorArgs.Length];
            for (int i = 0; i < concreteType.ConstructorArgs.Length; i++)
            {
                args[i] = __FetchInstance(concreteType.ConstructorArgs[i].ParameterType);
            }

            if (concreteType.Lifecycle == LifecycleType.Transient)
                return Activator.CreateInstance(concreteType.Type, args);

            object singleton;
            if (_singletons.TryGetValue(oftype, out singleton))
                return singleton;

            singleton = Activator.CreateInstance(concreteType.Type, args);
            _singletons[oftype] = singleton;

            return singleton;
        }


        public void Register<IType, CType>(LifecycleType lifecycle = LifecycleType.Transient)
            where IType: class
            where CType: class
        {
            Type interfaceType = typeof(IType);
            if (!interfaceType.IsInterface)
                throw new IoCContainerException("IType must be an interface: \"{0}\".", interfaceType);

            Type concreteType = typeof(CType);
            if (!concreteType.IsClass)
                throw new IoCContainerException("CType must be a class: \"{0}\".", interfaceType);

            if (_registeredTypes.ContainsKey(interfaceType))
                throw new IoCContainerException("Interface \"{0}\" has already been registered with container.", typeof(IType));

            Type ifac = concreteType.GetInterfaces().FirstOrDefault(i => i == interfaceType);
            if (ifac == null)
                throw new IoCContainerException("CType \"{0}\" does not inherit IType \"{1}\".", concreteType, interfaceType);

            ConstructorInfo[] ctors = concreteType.GetConstructors();
            if (ctors.Length == 0)
                throw new IoCContainerException("CType \"{0}\" must have one public constructor.", concreteType);
            else if (ctors.Length > 1)
                throw new IoCContainerException("CType \"{0}\" may have one and only one public constructor.", concreteType);

            ParameterInfo[] ctorArgs = ctors[0].GetParameters();
            foreach (ParameterInfo pi in ctorArgs)
            {
                if (!pi.ParameterType.IsInterface)
                    throw new IoCContainerException("CType \"{0}\" constructor has parameter that is not an interface: \"{1} {2}\".  All parameters must be interfaces.", concreteType, pi.ParameterType, pi.Name);
                if (!_registeredTypes.ContainsKey(pi.ParameterType))
                    throw new IoCContainerException("Constructor for CType \"{0}\" has parameter of unregistered type: \"{1} {2}\".", concreteType, pi.ParameterType, pi.Name);
            }

            _registeredTypes[interfaceType] = new ConcreteTypeInfo()
            {
                Type = typeof(CType),
                Lifecycle = lifecycle,
                ConstructorArgs = ctorArgs
            };
        }
    }
}
