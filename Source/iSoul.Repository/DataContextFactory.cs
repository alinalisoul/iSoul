namespace iSoul.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    public sealed class DataContextFactory : IDataContextFactory
    {
        private IEnumerable<IPocoConfiguration> pocoConfigurations;

        public DataContextFactory(IPocoConfiguration[] pocoConfigurations)
        {
            this.pocoConfigurations = pocoConfigurations;
        }

        #region IDataContextFactory 成员

        public IDataContext Create(string nameOrConnectionString, string configNameSpace)
        {
            return new DataContext(nameOrConnectionString, pocoConfigurations.Where(p => p.GetType().Namespace.StartsWith(configNameSpace)));
        }

        #endregion
    }
}
