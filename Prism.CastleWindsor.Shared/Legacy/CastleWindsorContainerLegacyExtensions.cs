using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Castle.Core;
using Castle.Windsor;
using Component = Castle.MicroKernel.Registration.Component;

#pragma warning disable 1584,1711,1572,1581,1580

namespace Prism.CastleWindsor.Legacy
{
    /// <summary>
    /// 
    /// </summary>
    public static class CastleWindsorContainerLegacyExtensions
    {
        /// <summary>Register a theClassType mapping with the container.</summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for theClassType <typeparamref name="TServiceType" />,
        /// actually return an instance of theClassType <typeparamref name="TClassType" />. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TServiceType"><see cref="T:System.Type" /> that wil l be requested.</typeparam>
        /// <typeparam name="TClassType"><see cref="T:System.Type" /> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <returns>The <see cref="T:Microsoft.Practices.Unity.UnityContainer" /> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IWindsorContainer RegisterType<TServiceType, TClassType>(this IWindsorContainer container, string name) where TClassType : TServiceType
        {
            return container.Register(Component.For(typeof(TServiceType))
                .ImplementedBy(typeof(TClassType))
                .Named(name)
                .LifeStyle.Transient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IWindsorContainer RegisterInstance(this IWindsorContainer container, Type type, object instance)
        {
            return container.Register(Component.For(type)
                .Instance(instance)
                .LifeStyle.Transient);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IWindsorContainer RegisterInstance<TService, TClassType>(this IWindsorContainer container, object instance, string name = null) where TService: class
                                                                                                                                                      where TClassType: class
        {
            return container.Register(Component.For(typeof(TService))
                .ImplementedBy(typeof(TClassType))
                .Instance(instance)
                .LifeStyle.Transient);
        }

        /// <summary>
        /// Registers an object for navigation.
        /// </summary>
        /// <typeparam name="T">The Type of the object to register</typeparam>
        /// <typeparam name="TServiceImplementation"></typeparam>
        /// <param name="container"><see cref="IWindsorContainer"/> used to register type for Navigation.</param>
        /// <param name="name">The unique name to register with the object.</param>
        public static IWindsorContainer RegisterTypeForNavigation<TServiceImplementation>(this IWindsorContainer container, string name = null) where TServiceImplementation : class
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!container.IsTypeRegistered<TServiceImplementation>() &&
                !container.Kernel.HasComponent(name))
            {
                return container.Register(Component.For(typeof(TServiceImplementation))
                    .ImplementedBy(typeof(object))
                    .Named(typeof(TServiceImplementation).Namespace)
                    .LifeStyle.Transient);
            }

            return container;
        }

        /// <summary>Register a theClassType mapping with the container.</summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for theClassType <typeparamref name="TServiceInterface" />,
        /// actually return an instance of theClassType <typeparamref name="TServiceImplementation" />. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TServiceInterface"><see cref="T:System.Type" /> that will be requested.</typeparam>
        /// <typeparam name="TServiceImplementation"><see cref="T:System.Type" /> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="theLifestyleType"></param>
        /// <returns>The <see cref="T:Microsoft.Practices.Unity.UnityContainer" /> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IWindsorContainer RegisterType<TServiceInterface, TServiceImplementation>(this IWindsorContainer container, string name, LifestyleType theLifestyleType) where TServiceImplementation : class, TServiceInterface
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!Enum.IsDefined(typeof(LifestyleType), theLifestyleType))
                throw new InvalidEnumArgumentException(nameof(theLifestyleType), (int)theLifestyleType,
                    typeof(LifestyleType));

            if (!container.IsTypeRegistered<TServiceInterface>() &&
                !container.IsTypeRegistered<TServiceImplementation>() &&
                !container.Kernel.HasComponent(name))
            {
                switch (theLifestyleType)
                {
                    case LifestyleType.Undefined:
                        return container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(name));
                    case LifestyleType.Singleton:
                        return container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Singleton);
                    case LifestyleType.Thread:
                        return container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.PerThread);
                    case LifestyleType.Transient:
                        return container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Transient);
                    case LifestyleType.Pooled:
                        return container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Pooled);
                    case LifestyleType.Custom:
                        break;
                    case LifestyleType.Scoped:
                        return container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Scoped(typeof(TServiceImplementation)));
                    case LifestyleType.Bound:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(theLifestyleType), theLifestyleType, null);
                }

                return container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Transient);
            }

            return container;
        }

        /// <summary>Register a theClassType mapping with the container.</summary>
        /// <remarks>
        /// This method is used to tell the container that when asked for theClassType <typeparamref name="TFrom" />,
        /// actually return an instance of theClassType <typeparamref name="TServiceImplementation" />. This is very useful for
        /// getting instances of interfaces.
        /// </remarks>
        /// <typeparam name="TFrom"><see cref="T:System.Type" /> that will be requested.</typeparam>
        /// <typeparam name="TServiceImplementation"><see cref="T:System.Type" /> that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        /// <param name="name">Name of this mapping.</param>
        /// <param name="theLifestyleType"></param>
        /// <returns>The <see cref="T:Microsoft.Practices.Unity.UnityContainer" /> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As designed")]
        public static IWindsorContainer RegisterType<TServiceImplementation>(this IWindsorContainer container, string name, LifestyleType theLifestyleType) where TServiceImplementation : class
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!Enum.IsDefined(typeof(LifestyleType), theLifestyleType))
                throw new InvalidEnumArgumentException(nameof(theLifestyleType), (int)theLifestyleType,
                    typeof(LifestyleType));

            if (!container.IsTypeRegistered<TServiceImplementation>() &&
                !container.Kernel.HasComponent(name))
            {
                switch (theLifestyleType)
                {
                    case LifestyleType.Undefined:
                        return container.Register(Component.For(typeof(TServiceImplementation))
                            .Named(name));
                    case LifestyleType.Singleton:
                        return container.Register(Component.For(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Singleton);
                    case LifestyleType.Thread:
                        return container.Register(Component.For(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.PerThread);
                    case LifestyleType.Transient:
                        return container.Register(Component.For(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Transient);
                    case LifestyleType.Pooled:
                        return container.Register(Component.For(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Pooled);
                    case LifestyleType.Custom:
                        break;
                    case LifestyleType.Scoped:
                        return container.Register(Component.For(typeof(TServiceImplementation))
                            .Named(name)
                            .LifeStyle.Scoped(typeof(TServiceImplementation)));
                    case LifestyleType.Bound:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(theLifestyleType), theLifestyleType, null);
                }

                return container.Register(Component.For(typeof(TServiceImplementation))
                    .Named(name)
                    .LifeStyle.Transient);
            }

            return container;
        }

        /// <summary>
        /// Resolves a service from the container. If the theClassType does not exist on the container, 
        /// first registers it with transient lifestyle.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="theServiceImplementationType"></param>
        /// <param name="serviceTypeName"></param>
        /// <returns></returns>
        public static object Resolve(this IWindsorContainer container, Type theServiceImplementationType, string serviceTypeName)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (!container.Kernel.HasComponent(theServiceImplementationType)) // &&
                //!container.Kernel.HasComponent(serviceTypeName))
                container.Register(Component.For(theServiceImplementationType)
                    .Named(theServiceImplementationType.FullName)
                    .LifeStyle.Transient);

            if (string.IsNullOrWhiteSpace(serviceTypeName))
            {
                throw new ArgumentException("message", nameof(serviceTypeName));
            }

            return container.Resolve(theServiceImplementationType);
        }

        /// <summary>
        /// Registers the theClassType on the container.
        /// </summary>
        /// <typeparam name="TServiceInterface">The theClassType of the interface.</typeparam>
        /// <typeparam name="TServiceImplementation">The theClassType of the service.</typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterType<TServiceInterface, TServiceImplementation>(this IWindsorContainer container)
        {
            var serviceImplementationObject = container.TryResolve<TServiceImplementation>();
            var serviceInterfaceObject      = container.TryResolve<TServiceInterface>();

            if (serviceImplementationObject == null &&
                serviceInterfaceObject == null)
                RegisterType<TServiceInterface, TServiceImplementation>(container, true);
        }

        /// <summary>
        /// Registers the theClassType on the container.
        /// </summary>
        /// <typeparam name="TServiceInterface">The theClassType of interface.</typeparam>
        /// <typeparam name="TServiceImplementation">The theClassType of the service.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="singleton">if set to <c>true</c> theClassType will be registered as singleton.</param>
        public static void RegisterType<TServiceInterface, TServiceImplementation>(this IWindsorContainer container, bool singleton)
        {
            var serviceInterfaceTypeRegistered      = container.Kernel.HasComponent(typeof(TServiceInterface).FullName);
            var serviceImplementationTypeRegistered = container.Kernel.HasComponent(typeof(TServiceImplementation).FullName);

            if (!serviceInterfaceTypeRegistered && !serviceImplementationTypeRegistered)
            {
                //var serviceType = container.TryResolve(typeof(TServiceInterface));

                switch (singleton)
                {
                    case true:
                        container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(typeof(TServiceImplementation).FullName)
                            .LifeStyle.Singleton);
                        break;
                    default:
                        container.Register(Component.For(typeof(TServiceInterface))
                            .ImplementedBy(typeof(TServiceImplementation))
                            .Named(typeof(TServiceImplementation).FullName)
                            .LifeStyle.Transient);
                        break;
                }
            }
        }

        /// <summary>
        /// Basic function for Resolve 
        /// </summary>
        /// <param name="windsorContainer"></param>
        /// <param name="serviceImplementationType"></param>
        /// <returns></returns>
        internal static object Resolve(IWindsorContainer windsorContainer, Type serviceImplementationType)
        {
            if (serviceImplementationType.IsClass && 
                !windsorContainer.Kernel.HasComponent(serviceImplementationType))
                windsorContainer.Register(Component.For(serviceImplementationType)
                    .Named(serviceImplementationType.FullName)
                    .LifeStyle.Transient);

            return windsorContainer.Resolve(serviceImplementationType);
        }
    }
}

