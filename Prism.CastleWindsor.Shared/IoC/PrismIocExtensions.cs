using Castle.Windsor;
using Prism.Ioc;

namespace Prism.CastleWindsor
{
    /// <summary>
    /// 
    /// </summary>
    public static class PrismIocExtensions
    {
        /// <summary>
        /// Gets the <see cref="IWindsorContainer" /> from the <see cref="IContainerProvider" />
        /// </summary>
        /// <param name="containerProvider">The current <see cref="IContainerProvider" /></param>
        /// <returns>The underlying <see cref="IWindsorContainer" /></returns>
        public static IWindsorContainer GetContainer(this IContainerProvider containerProvider)
        {
            return ((IContainerExtension<IWindsorContainer>)containerProvider).Instance;
        }

        /// <summary>
        /// Gets the <see cref="IWindsorContainer" /> from the <see cref="IContainerProvider" />
        /// </summary>
        /// <param name="containerRegistry">The current <see cref="IContainerRegistry" /></param>
        /// <returns>The underlying <see cref="IWindsorContainer" /></returns>
        public static IWindsorContainer GetContainer(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<IWindsorContainer>)containerRegistry).Instance;
        }
    }
}
