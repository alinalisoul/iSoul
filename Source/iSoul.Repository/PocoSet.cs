namespace iSoul.Repository
{
    using System.Data.Entity;
    using System.Linq;

    internal class PocoSet<TPoco> : IPocoSet<TPoco> where TPoco : BasePoco
    {
        private DbSet<TPoco> pocos;

        public PocoSet(DbSet<TPoco> pocos)
        {
            this.pocos = pocos;
        }

        #region IPocoSet<TPoco> 成员

        public IQueryable<TPoco> Pocos
        {
            get
            {
                return this.pocos;
            }
        }

        #endregion
    }
}
