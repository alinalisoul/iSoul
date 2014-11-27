using System;

namespace iSoul.Repository
{
    public abstract class PocoConfiguration<TPoco> : IPocoConfiguration where TPoco : BasePoco
    {
        #region IPocoConfiguration 成员

        public Type PocoType
        {
            get { return typeof(TPoco); }
        }

        public object Configuration
        {
            get
            {
                return this.Config();
            }
        }

        #endregion

        protected abstract object Config();
    }
}
