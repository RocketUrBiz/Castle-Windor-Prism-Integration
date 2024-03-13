using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Prism.CastleWindsor.Legacy;
using Prism.Ioc;
using Prism.Ioc.Internals;
using Prism.Regions;
// ReSharper disable MemberCanBePrivate.Global

namespace Prism.CastleWindsor
{
    /// <summary>
    /// The <see cref="IContainerExtension" /> Implementation to use with CastleWindsor
    /// </summary>
#if ContainerExtensions
    internal partial
#else
    public
#endif
    class CastleWindsorContainerExtension : IContainerExtension<IWindsorContainer>, IContainerInfo
    {
        private CastleWindsorScopedProvider _currentScope;
        /// <summary>
        /// The instance of the wrapped container
        /// </summary>
        public IWindsorContainer Instance { get; }

#if !ContainerExtensions
        /// <summary>
        /// Constructs a default <see cref="CastleWindsorContainerExtension" />
        /// </summary>
        public CastleWindsorContainerExtension()
            : this(new WindsorContainer())
        {
        }

        /// <summary>
        /// Constructs a <see cref="CastleWindsorContainerExtension" /> with the specified <see cref="IWindsorContainer" />
        /// </summary>
        /// <param name="container"></param>
        public CastleWindsorContainerExtension(IWindsorContainer container)
        {
            // Make native API calls into Castle Windsor

            Instance = container;

            // First Register the current instance of the Castle Windsor Container

            if (!Instance.IsTypeRegistered(typeof(IWindsorContainer), container.Name))
            {
                Instance.Register(Component.For<IWindsorContainer>()
                    .Instance(container)
                    .Named(container.Name)
                    .LifeStyle.Singleton);
            }

            var interfaceName = typeof(IContainerExtension).FullName;

            if (!Instance.IsTypeRegistered(typeof(IContainerExtension)))
            {
                Instance.Register(Component.For<IContainerExtension>()
                    .Instance(this)
                    .Named(interfaceName)
                    .LifeStyle.Singleton);
            }

            interfaceName = typeof(IContainerProvider).FullName;

            if (!Instance.IsTypeRegistered(typeof(IContainerProvider)))
            {
                Instance.Register(Component.For<IContainerProvider>()
                    .Instance(this)
                    .Named(interfaceName)
                    .LifeStyle.Singleton);
            }

            interfaceName = typeof(Prism.Regions.IRegionBehaviorFactory).FullName;
            var implementationName = typeof(Prism.Regions.RegionBehaviorFactory).FullName;

            if (!Instance.IsTypeRegistered<Prism.Regions.IRegionBehaviorFactory>())
            {
                //Instance.RegisterType<Prism.Regions.IRegionBehaviorFactory, Prism.Regions.RegionBehaviorFactory>(
                //    implementationName, LifestyleType.Singleton);

                Instance.Register(Component.For<Prism.Regions.IRegionBehaviorFactory>()
                    .ImplementedBy<Prism.Regions.RegionBehaviorFactory>()
                    .Named(interfaceName)
                    .LifeStyle.Singleton);
            }

            // Self-Registration of Prism.Ioc.IContainerExtension

            Instance.Register(Classes.FromAssemblyContaining<Prism.Ioc.IContainerExtension>().BasedOn<Prism.Ioc.IContainerExtension>().LifestyleSingleton());

            // register region adapters
            Instance.Register(Classes.FromAssemblyContaining<IRegionAdapter>().BasedOn<IRegionAdapter>().LifestyleTransient());

            // register region behaviors
            Instance.Register(Classes.FromAssemblyContaining<IRegionBehavior>().BasedOn<IRegionBehavior>().LifestyleTransient());

            // Prism.Regions.RegionBehaviorFactory

            //Instance.Register(Classes.FromAssemblyContaining<Prism.Regions.IRegionBehaviorFactory>().BasedOn<Prism.Regions.RegionBehaviorFactory>().LifestyleSingleton());

            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ComponentResolutionException));
        }
#endif

        /// <summary>
        /// Gets the current <see cref="IScopedProvider"/>
        /// </summary>
        public IScopedProvider CurrentScope => _currentScope;

        /// <summary>
        /// Used to perform any final steps for configuring the extension that may be required by the container.
        /// </summary>
        public void FinalizeExtension() { }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            if (!Instance.Kernel.HasComponent(type))
            {
                Instance.Register(Component.For(type)
                    .Instance(instance)
                    .LifeStyle.Transient);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            if (!Instance.Kernel.HasComponent(type) &&
                !Instance.Kernel.HasComponent(name))
            {
                Instance.Register(Component.For(type)
                    .Instance(instance)
                    .Named(name)
                    .LifeStyle.Transient);
            }

            return this;
        }

        /// <summary>
        /// Register a TServiceImplementation object type instance as a singleton
        /// </summary>
        /// <typeparam name="TServiceImplementation"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IContainerRegistry RegisterSingletonType<TServiceImplementation>(object instance, string name) where TServiceImplementation : class
        {
            if (!Instance.Kernel.HasComponent(typeof(TServiceImplementation)) &&
                !Instance.Kernel.HasComponent(name))
            {
                Instance.Register(Component.For(typeof(TServiceImplementation))
                                           .Instance(instance)
                                           .Named(name)
                                           .LifeStyle.Singleton);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lifeStyleScopeType"></param>
        /// <typeparam name="TServiceInterface"></typeparam>
        /// <typeparam name="TServiceImplementation"></typeparam>
        /// <returns></returns>
        public IContainerRegistry RegisterTypeWithLifeStyleType<TServiceInterface, TServiceImplementation>(string name, LifestyleType lifeStyleScopeType)
        {
            return RegisterSingleton(typeof(TServiceInterface), typeof(TServiceImplementation), name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceInterfaceType"></param>
        /// <param name="serviceImplementationType"></param>
        /// <returns></returns>
        public IContainerRegistry RegisterSingleton(Type serviceInterfaceType, Type serviceImplementationType)
        {
            if (serviceInterfaceType == null) throw new ArgumentNullException(nameof(serviceInterfaceType));
            if (serviceImplementationType == null) throw new ArgumentNullException(nameof(serviceImplementationType));

            if (!Instance.Kernel.HasComponent(serviceInterfaceType) &&
                !Instance.Kernel.HasComponent(serviceImplementationType) &&
                !Instance.Kernel.HasComponent(serviceImplementationType.FullName))
            {
                Instance.Register(Component.For(serviceInterfaceType)
                    .ImplementedBy(serviceImplementationType)
                    .Named(serviceImplementationType.FullName)
                    .LifeStyle.Singleton);
            }

            return this;
        }

        /// <summary>
        /// Call-down Castle API to Register a Singleton via Typed Object Instance type using a supplied name
        /// </summary>
        /// <param name="serviceInterfaceType"></param>
        /// <param name="serviceImplementationType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IContainerRegistry RegisterSingleton(Type serviceInterfaceType, Type serviceImplementationType, string name)
        {
            if (!Instance.IsTypeRegistered(serviceInterfaceType) &&
                !Instance.IsTypeRegistered(serviceImplementationType) &&
                !Instance.Kernel.HasComponent(name))
            {
                Instance.Register(Component.For(serviceInterfaceType)
                    .ImplementedBy(serviceImplementationType)
                    .Named(name)
                    .LifeStyle.Singleton);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serviceTypes"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call-down to the Castle API to Register a Singleton Type via Generics using a supplied name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IContainerRegistry RegisterSingleton<TServiceInterface, TServiceImplementation>(string name) where TServiceImplementation : class, TServiceInterface
        {
            if (!Instance.IsTypeRegistered<TServiceInterface>() &&
                !Instance.IsTypeRegistered<TServiceImplementation>() &&
                !Instance.Kernel.HasComponent(name))
            {
                Instance.RegisterType<TServiceInterface, TServiceImplementation>(name, LifestyleType.Singleton);
            }

            return this;
        }

        /// <summary>
        /// This method takes no parameters and will register the TInterfaceService and TServiceImplementation using the Fullname of the of the
        /// types in the host assembly
        /// </summary>
        /// <returns></returns>
        public IContainerRegistry RegisterSingleton<TServiceInterface, TServiceImplementation>() where TServiceImplementation : class, TServiceInterface
        {
            // we pass the true value to create a Singleton instance
            Instance.RegisterType<TServiceInterface, TServiceImplementation>(true);

            //if (!Instance.Kernel.HasComponent(typeof(TServiceInterface))      &&
            //    !Instance.Kernel.HasComponent(typeof(TServiceImplementation)) &&
            //    !Instance.Kernel.HasComponent(typeof(TServiceInterface).FullName) &&
            //    !Instance.Kernel.HasComponent(typeof(TServiceImplementation).FullName))
            //{
            //    Instance.Register(Component.For(typeof(TServiceInterface))
            //                               .ImplementedBy(typeof(TServiceImplementation))
            //                               .Named(typeof(TServiceInterface).FullName)
            //                               .LifeStyle.Singleton);
            //}

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromServiceType"></param>
        /// <param name="toServiceType"></param>
        /// <returns></returns>
        public IContainerRegistry Register(Type fromServiceType, Type toServiceType)
        {
            if (!Instance.Kernel.HasComponent(fromServiceType) &&
                !Instance.Kernel.HasComponent(toServiceType.FullName))
            {
                Instance.Register(Component.For(fromServiceType)
                    .ImplementedBy(toServiceType)
                    .Named(toServiceType.FullName)
                    .LifeStyle.Transient);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromServiceType"></param>
        /// <param name="toServiceType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IContainerRegistry Register(Type fromServiceType, Type toServiceType, string name)
        {
            if (!Instance.Kernel.HasComponent(fromServiceType) &&
                !Instance.Kernel.HasComponent(name))
            {
                Instance.Register(Component.For(fromServiceType)
                    .ImplementedBy(toServiceType)
                    .Named(name)
                    .LifeStyle.Transient);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            Instance.Register(Component.For(type).UsingFactoryMethod(factoryMethod));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.Register(Component.For(type)
                .UsingFactoryMethod(factoryMethod as Func<IKernel, IContainerProvider>));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serviceTypes"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContainerRegistry RegisterScoped(Type @from, Type to)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            if (!Instance.Kernel.HasComponent(type))
                Instance.Register(Component.For(type)
                    .Named(type.FullName)
                    .LifeStyle.Transient);

            return Instance.Resolve(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Resolve(Type type, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));
            // Check to see if we can Resolve the type by name

            var typeRegisteredByName = Instance.Kernel.HasComponent(name);
            var typeRegisteredByType = Instance.Kernel.HasComponent(type);

            // Custom Fix-up logic to resolve registration inconsistency 

            switch (typeRegisteredByName)
            {
                case true when !typeRegisteredByType:
                    Instance.Register(Component.For(type)
                        .Named(type.FullName)
                        .LifeStyle.Transient);

                    return Instance.IsTypeRegistered(type) ? Instance.Resolve(type) : Instance.Resolve(name, type);
                case false when typeRegisteredByType:
                    Instance.Register(Component.For(type)
                        .Named(name)
                        .LifeStyle.Transient);

                    return Instance.Kernel.HasComponent(name) ? Instance.Resolve(type, name) : Instance.Resolve(name, type);
                case false:
                    Instance.Register(Component.For(type)
                        .Named(type.FullName)
                        .LifeStyle.Transient);
                    return Instance.Resolve(type);
                default:
                    return Instance.Resolve(name, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            if (parameters.Length == 0)
                throw new ArgumentException($@"Value cannot be an empty collection.", nameof(parameters));

            var overrides = parameters.Select(p => new KeyValuePair<object, object>(p.Type, p.Instance)).ToList();

            var resolveParameters = new Arguments().Add(overrides);

            return Instance.Resolve(type, resolveParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            var overrides = parameters.Select(p => new KeyValuePair<object, object>(p.Type, p.Instance)).ToList();

            var resolveParameters = new Arguments().Add(overrides);

            return Instance.Resolve(name, type, resolveParameters);
        }

        /// <summary>
        /// Creates a new Scope
        /// </summary>
        public virtual IScopedProvider CreateScope() => CreateScopeInternal();

        /// <summary>
        /// Creates a new Scope
        /// </summary>
        public virtual IScopedProvider CreateScope(string containerName) => CreateScopeInternal(containerName);

        private const string defaultChildContainer = @"internalDefaultChildContainer";

        /// <summary>
        /// Creates a new Scope and provides the updated ServiceProvider
        /// </summary>
        /// <returns>A child <see cref="IWindsorContainer" />.</returns>
        /// <remarks>
        /// This should be called by custom implementations that Implement IServiceScopeFactory
        /// </remarks>
        protected IScopedProvider CreateScopeInternal()
        {
            if (IsRegistered(typeof(IWindsorContainer), defaultChildContainer))
            {
                Instance.AddChildContainer(new WindsorContainer(defaultChildContainer));
                var childContainer = Instance.GetChildContainer(defaultChildContainer);
                _currentScope = new CastleWindsorScopedProvider(childContainer);
            }

            return _currentScope;
        }

        /// <summary>
        /// Creates a new Scope and provides the updated ServiceProvider
        /// </summary>
        /// <returns>A child <see cref="IWindsorContainer" />.</returns>
        /// <remarks>
        /// This should be called by custom implementations that Implement IServiceScopeFactory
        /// </remarks>
        protected IScopedProvider CreateScopeInternal(string childContainerName)
        {
            var childContainer = Instance.GetChildContainer(childContainerName);

            if (childContainer is null)
            {
                Instance.AddChildContainer(new WindsorContainer(childContainerName));
                childContainer = Instance.GetChildContainer(childContainerName);
                _currentScope = new CastleWindsorScopedProvider(childContainer);

            }
            else
            {
                _currentScope = new CastleWindsorScopedProvider(childContainer);
            }

            return _currentScope;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsRegistered(Type type) => Instance.Kernel.HasComponent(type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsRegistered(Type type, string name) => !Instance.Kernel.HasComponent(type) && !Instance.Kernel.HasComponent(name);

        public Type GetRegistrationType(string key)
        {
            return null;
        }

        public Type GetRegistrationType(Type serviceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        private class CastleWindsorScopedProvider : IScopedProvider
        {
            public CastleWindsorScopedProvider(IWindsorContainer container)
            {
                Container = container;
            }

            public IWindsorContainer Container { get; private set; }
            public bool IsAttached { get; set; }
            public IScopedProvider CurrentScope => this;

            public IScopedProvider CreateScope() => this;

            public void Dispose()
            {
                Container.Dispose();
                Container = null;
            }

            public object Resolve(Type type) =>
                Resolve(type, Array.Empty<(Type, object)>());

            public object Resolve(Type type, string name) =>
                Resolve(type, name, Array.Empty<(Type, object)>());

            public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    var overrides = parameters.Select(p => p.Instance).ToArray();

                    Arguments overRideArgs = new Arguments().AddProperties(parameters);

                    return Container.Resolve(type, overRideArgs);
                }
                catch (Exception ex)
                {
                    throw new ContainerResolutionException(type, ex);
                }
            }

            public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    // Unity will simply return a new object() for unregistered Views
                    if (!Container.IsTypeRegistered(type, name))
                        throw new KeyNotFoundException($"No registered type {type.Name} with the key {name}.");

                    var overrides = parameters.Select(p => p.Instance).ToArray();

                    Arguments overRideArgs = new Arguments().AddProperties(parameters);

                    return Container.Resolve(type, overRideArgs);
                }
                catch (Exception ex)
                {
                    throw new ContainerResolutionException(type, name, ex);
                }
            }
        }
    }
}

