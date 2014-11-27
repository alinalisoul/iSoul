using iSoul.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSoul.Model.Core
{
    public abstract class ModelProvider : IModelProvider
    {
        #region IModelProvider 成员

        public abstract bool CanProvider(Type type);

        #endregion

        internal void Init(IDataContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory;
        }

        protected IDataContextFactory ContextFactory { get; private set; }

        protected abstract string NameOrConnectionString { get; }

        protected abstract string ConfigNameSpace { get; }

        protected IDataContext GetContext(string nameOrConnectionString = "", string configNameSpace = "")
        {
            if (String.IsNullOrEmpty(nameOrConnectionString))
            {
                nameOrConnectionString = this.NameOrConnectionString;
            }
            if (String.IsNullOrEmpty(configNameSpace))
            {
                configNameSpace = this.ConfigNameSpace;
            }
            return this.ContextFactory.Create(nameOrConnectionString, configNameSpace);
        }
    }
}
