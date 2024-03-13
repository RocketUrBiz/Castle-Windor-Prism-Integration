using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using CommonServiceLocator;
using Prism.Regions;

namespace Prism.CastleWindsor.Shared.Regions
{
    /// <summary>
    /// Specialization of the default RegionNavigationContentLoader that queries the corresponding <see cref="IUnityContainer"/>
    /// to obtain the name of the view's type registered for the contract name.
    /// </summary>
    public class CastleWindsorRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private IWindsorContainer windsorcontainer;
        

        /// <summary>
        /// Initializes a new instance of the <see /> class.
        /// </summary>
        /// <param name="serviceLocator"><see cref="IServiceLocator"/> used to create the instance of the view from its <see cref="Type"/>.</param>
        /// <param name="container"><see /> where the views are registered.</param>
        public CastleWindsorRegionNavigationContentLoader(IServiceLocator serviceLocator, IWindsorContainer container)
            : base(container)
        {
            this.windsorcontainer = container;
        }

        public IWindsorContainer Container { get => windsorcontainer; set => windsorcontainer = value; }

        /// <summary>
        /// Returns the set of candidates that may satisfy this navigation request.
        /// </summary>
        /// <param name="region">The region containing items that may satisfy the navigation request.</param>
        /// <param name="candidateNavigationContract">The candidate navigation target.</param>
        /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
        protected override IEnumerable<object> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
        {
            if (candidateNavigationContract == null || candidateNavigationContract.Equals(string.Empty))
                throw new ArgumentNullException(nameof(candidateNavigationContract));

            IEnumerable<object> contractCandidates = base.GetCandidatesFromRegion(region, candidateNavigationContract);

            var candidatesFromRegion = contractCandidates as object[] ?? contractCandidates.ToArray();

            if (candidatesFromRegion.Any()) return candidatesFromRegion;

            //First try friendly name registration. If not found, try type registration
            var matchingRegistration = windsorcontainer.Kernel.ConfigurationStore.GetComponents().FirstOrDefault(r => candidateNavigationContract.Equals(r.Name, StringComparison.Ordinal)) ??
                                       windsorcontainer.Kernel.ConfigurationStore.GetComponents().FirstOrDefault(r => candidateNavigationContract.Equals(r.GetType().Name, StringComparison.Ordinal));
            if (matchingRegistration == null) return new object[0];

            string typeCandidateName = matchingRegistration.GetType().FullName;

            contractCandidates = base.GetCandidatesFromRegion(region, typeCandidateName);

            return candidatesFromRegion;
        }
    }
}


