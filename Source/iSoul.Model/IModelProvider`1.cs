using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSoul.Model
{
    public interface IModelProvider<TModel> : IModelProvider where TModel : IViewModel
    {
        TModel Get(Guid id);

        TModel Get(int id);

        ICollection<TModel> Models { get; }

        TModel Edit(TModel model);

        TModel Create(TModel model);

        TModel Delete(Guid id);
    }
}
