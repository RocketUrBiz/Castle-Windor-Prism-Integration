using System;
using Prism.CastleWindsor.ExceptionResolution;
using Prism.Ioc;

namespace Prism.CastleWindsor
{
    /// <summary>
    /// Base bootstrapper class that uses <see cref="CastleWindsorContainerExtension"/> as it's container.
    /// </summary>
    public abstract class PrismBootstrapper : PrismBootstrapperBase
    {
        /// <summary>
        /// Create a new <see cref="CastleWindsorContainerExtension"/> used by Prism.
        /// </summary>
        /// <returns>A new <see cref="CastleWindsorContainerExtension"/>.</returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new CastleWindsorContainerExtension();
        }

        /// <summary>
        /// Registers the <see cref="Type"/>s of the Exceptions that are not considered 
        /// root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ResolutionFailedException));
        }
    }
}
