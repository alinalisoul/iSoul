namespace iSoul.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;


    class DataContext : IDataContext
    {
        internal EFRespository Respository { get; private set; }

        public DataContext(string nameOrConnectionString, IEnumerable<IPocoConfiguration> pocoConfigurations)
        {
            this.Respository = new EFRespository(nameOrConnectionString, pocoConfigurations);
        }

        #region IDataContext 成员

        public string ConnectionString
        {
            get
            {
                return this.Respository.Database.Connection.ConnectionString;
            }
        }

        public IPocoSet<TPoco> Pocos<TPoco>() where TPoco : BasePoco
        {
            var pocos = this.Respository.Set<TPoco>();
            return new PocoSet<TPoco>(pocos);
        }

        public bool Add<TPoco>(TPoco poco) where TPoco : BasePoco
        {
            var dbSet = this.Respository.Set<TPoco>();
            dbSet.Add(poco);
            return true;
        }

        public bool Update<TPoco>(Guid id, Action<TPoco> action) where TPoco : BasePoco, new()
        {
            var dbSet = this.Respository.Set<TPoco>();
            var poco = dbSet.SingleOrDefault(p => p.Identity.Equals(id));
            action.Invoke(poco);
            return true;
        }

        public bool Delete<TPoco>(Guid id, bool fakeDelete = true) where TPoco : BasePoco, new()
        {
            var dbSet = this.Respository.Set<TPoco>();
            if (fakeDelete)
            {
                var poco = dbSet.SingleOrDefault(d => d.Identity.Equals(id));
                poco.IsDeleted = true;
            }
            else
            {

                var poco = dbSet.SingleOrDefault(d => d.Identity.Equals(id));
                dbSet.Remove(poco);
            }
            return true;
        }

        public bool Delete<TPoco>(Expression<Func<TPoco, bool>> filter, bool fakeDelete = true) where TPoco : BasePoco, new()
        {
            var dbSet = this.Respository.Set<TPoco>();
            if (fakeDelete)
            {
                var pocos = dbSet.Where(filter).AsParallel();
                pocos.ForAll(p => p.IsDeleted = true);
            }
            else
            {
                var pocos = dbSet.Where(filter).ToList();
                dbSet.RemoveRange(pocos);
            }
            return true;
        }

        public bool SaveChanges()
        {
            if (this.Respository.ChangeTracker.HasChanges())
            {
                var changes = this.Respository.ChangeTracker.Entries();
                foreach (var item in changes)
                {
                    var poco = item.Entity as BasePoco;
                    poco.Updated = DateTime.Now;
                }
                var errors = this.Respository.GetValidationErrors();
                this.Respository.SaveChanges();
            }
            return true;
        }

        public void Load<TEntity, TPoco>(TEntity entity, Expression<Func<TEntity, TPoco>> expression)
            where TEntity : BasePoco
            where TPoco : BasePoco
        {
            var entityReference = this.Respository.Entry(entity).Reference(expression);
            if (!entityReference.IsLoaded)
            {
                entityReference.Load();
            }
        }

        public void Load<TEntity>(TEntity entity, string propertyName)
            where TEntity : BasePoco
        {
            var entityReference = this.Respository.Entry(entity).Reference(propertyName);
            if (!entityReference.IsLoaded)
            {
                entityReference.Load();
            }
        }

        public void LoadAll<TEntity, TPoco>(TEntity entity, Expression<Func<TEntity, ICollection<TPoco>>> expression)
            where TEntity : BasePoco
            where TPoco : BasePoco
        {
            var entityReference = this.Respository.Entry(entity).Collection(expression);
            if (!entityReference.IsLoaded)
            {
                entityReference.Load();
            }
        }

        public void LoadAll<TEntity>(TEntity entity, string propertyName)
            where TEntity : BasePoco
        {
            var entityReference = this.Respository.Entry(entity).Collection(propertyName);
            if (!entityReference.IsLoaded)
            {
                entityReference.Load();
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (this.Respository != null)
            {
                this.Respository.Dispose();
                this.Respository = null;
            }
        }

        #endregion

    }
}
