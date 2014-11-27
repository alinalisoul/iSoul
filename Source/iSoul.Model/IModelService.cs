using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSoul.Model
{
    public interface IModelService
    {
        TModel Get<TModel>(Guid id) where TModel : IViewModel;

        TModel Get<TModel>(int id) where TModel : IViewModel;

        ICollection<TModel> GetAll<TModel>() where TModel : IViewModel;

        ICollection<TMany> GetMany<TOne, TMany>(Guid id)
            where TOne : IViewModel
            where TMany : IViewModel;

        void Create<TModel>(TModel model) where TModel : IViewModel;

        void Edit<TModel>(TModel model) where TModel : IViewModel;

        void Delete<TModel>(Guid id) where TModel : IViewModel;

        void Add<TTarget, TModel>(Guid id, TModel model)
            where TTarget : IViewModel
            where TModel : IViewModel;

        void Remove<TTarget, TModel>(Guid id, TModel model)
            where TTarget : IViewModel
            where TModel : IViewModel;
    }
}
