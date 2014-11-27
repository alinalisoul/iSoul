namespace iSoul.Repository
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Configuration;
    using System.Linq;

    public class EFRespository : DbContext
    {
        protected ICollection<IPocoConfiguration> Configurations;


        public EFRespository(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Initialize();
        }


        public EFRespository(string nameOrConnectionString, IEnumerable<IPocoConfiguration> pocoConfigurations)
            : this(nameOrConnectionString)
        {
            this.Configurations = new List<IPocoConfiguration>(pocoConfigurations);
            this.Configuration.LazyLoadingEnabled = false;
        }

        internal bool CreateIfNotExists()
        {
            return this.Database.CreateIfNotExists();

        }

        protected virtual void Initialize()
        {
        }


        protected sealed override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            foreach (var item in this.Configurations)
            {
                var addMethod = typeof(ConfigurationRegistrar)
              .GetMethods()
              .Single(m =>
                m.Name == "Add"
             && m.GetGenericArguments().Any(a => a.Name == "TEntityType"));
                addMethod = addMethod.MakeGenericMethod(item.PocoType);
                addMethod.Invoke(modelBuilder.Configurations, new object[] { item.Configuration });
            }
        }
    }
}
