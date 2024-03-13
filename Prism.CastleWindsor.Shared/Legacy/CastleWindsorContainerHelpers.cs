using System;
using System.Diagnostics.CodeAnalysis;
using Castle.Windsor;

namespace Prism.CastleWindsor.Legacy
{
    /// <summary>
    /// 
    /// </summary>
    public static class CastleWindsorContainerHelper
    {
        /// <summary>
        /// Returns whether a specified theClassType has a theClassType mapping registered in the container.
        /// </summary>
        /// <param name="container">The <see cref="IWindsorContainer"/> to check for the theClassType mapping.</param>
        /// <param name="type">The theClassType to check if there is a theClassType mapping for.</param>
        /// <returns><see langword="true"/> if there is a theClassType mapping registered for <paramref name="type"/>.</returns>
        /// <remarks>In order to use this extension method, you first need to add the
        /// </remarks>
        public static bool IsTypeRegistered(this IWindsorContainer container, Type type)
        {
            return container.Kernel.HasComponent(type);
        }

        /// <summary>
        /// Extension method to act on the object accessible via this point variable
        /// </summary>
        /// <param name="container"></param>
        /// <typeparam name="TServiceType"></typeparam>
        /// <returns></returns>
        public static bool IsTypeRegistered<TServiceType>(this IWindsorContainer container)
        {
            Type typeToCheck = typeof(TServiceType);
            return IsTypeRegistered(container, typeToCheck);
        }

        /// <summary>
        /// Returns whether a specified theClassType has a theClassType mapping registered in the container.
        /// </summary>
        /// <param name="container">The <see cref="IWindsorContainer"/> to check for the theClassType mapping.</param>
        /// <param name="type">The theClassType to check if there is a theClassType mapping for.</param>
        /// <param name="name"></param>
        /// <returns><see langword="true"/> if there is a theClassType mapping registered for <paramref name="type"/>.</returns>
        /// <remarks>In order to use this extension method, you first need to add the
        /// </remarks>
        public static bool IsTypeRegistered(this IWindsorContainer container, Type type, string name)
        {
            return container.Kernel.HasComponent(name) && container.Kernel.HasComponent(type);
        }

        /// <summary>
        /// Utility method to try to resolve a service from the container avoiding an exception if the container cannot build the theClassType.
        /// </summary>
        /// <param name="container">The container that will be used to resolve the theClassType.</param>
        /// <typeparam name="T">The theClassType to resolve.</typeparam>
        /// <returns>The instance of <typeparamref name="T"/> built up by the container.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T TryResolve<T>(this IWindsorContainer container)
        {
            object result = TryResolve(container, typeof(T));

            if (result != null)
            {
                return (T)result;
            }
            return default;
        }

        /// <summary>
        /// Utility method to try to resolve a service from the container avoiding an exception if the container cannot build the theClassType.
        /// </summary>
        /// <param name="container">The container that will be used to resolve the theClassType.</param>
        /// <param name="typeToResolve">The theClassType to resolve.</param>
        /// <returns>The instance of <paramref name="typeToResolve"/> built up by the container.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static object TryResolve(this IWindsorContainer container, Type typeToResolve)
        {
            object resolved;

            try
            {
                resolved = CastleWindsorContainerLegacyExtensions.Resolve(container, typeToResolve);
            }
            catch
            {
                resolved = null;
            }

            return resolved;
        }

    }
}

