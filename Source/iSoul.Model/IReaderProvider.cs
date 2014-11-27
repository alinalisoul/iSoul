using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSoul.Model
{
    public interface IReaderProvider<TModel> : IModelProvider where TModel : IViewModel
    {
        TModel Get(Guid id);

        ICollection<TModel> Get();
    }
}
