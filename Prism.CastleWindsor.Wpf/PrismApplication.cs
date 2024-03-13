using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Prism.Ioc;
using Prism.Regions;

namespace Prism.CastleWindsor
{
    /// <summary>
    /// InKnowWorks Contropolus Prism Application
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase
    {
        private IWindsorContainer windsorContainer = null;
        /// <summary>
        /// Create a new <see cref="CastleWindsorContainerExtension"/> used by Prism.
        /// </summary>
        /// <returns>A new <see cref="CastleWindsorContainerExtension"/>.</returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new CastleWindsorContainerExtension();
        }

        /// <summary>
        /// Castle Windsor dependency model runs much deeper than Dryloc and Unity so we have to
        /// register dependent modules
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            if (containerRegistry == null) throw new ArgumentNullException(nameof(containerRegistry));

            base.RegisterRequiredTypes(containerRegistry);

            windsorContainer = containerRegistry.GetContainer();

            if (!containerRegistry.IsRegistered<IRegionBehaviorFactory>())
            {
                containerRegistry.RegisterSingleton<IRegionBehaviorFactory>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ConfigurationProcessingException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ComponentResolutionException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ComponentNotFoundException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ComponentRegistrationException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(CircularDependencyException));
        }
    }
}
