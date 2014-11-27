using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iSoul.Repository
{
    public interface IDataContext : IDisposable
    {
        string ConnectionString { get; }

        IPocoSet<TPoco> Pocos<TPoco>() where TPoco : BasePoco;

        bool Add<TPoco>(TPoco poco) where TPoco : BasePoco;

        bool Update<TPoco>(Guid id, Action<TPoco> action) where TPoco : BasePoco, new();

        bool Delete<TPoco>(Guid id, bool fakeDelete = true) where TPoco : BasePoco, new();

        bool Delete<TPoco>(Expression<Func<TPoco, bool>> filter, bool fakeDelete = true) where TPoco : BasePoco, new();

        void Load<TEntity>(TEntity entity, string propertyName)
            where TEntity : BasePoco;

        void Load<TEntity, TPoco>(TEntity entity, Expression<Func<TEntity, TPoco>> expression)
            where TEntity : BasePoco
            where TPoco : BasePoco;

        void LoadAll<TEntity, TPoco>(TEntity entity, Expression<Func<TEntity, ICollection<TPoco>>> expression)
            where TEntity : BasePoco
            where TPoco : BasePoco;

        void LoadAll<TEntity>(TEntity entity, string propertyName)
            where TEntity : BasePoco;


        bool SaveChanges();
    }
}
