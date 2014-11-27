using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSoul.Model
{
    public interface IWriteProvider<TModel> : IModelProvider where TModel : IWriteModel
    {
        TReaderModel Edit<TEidtModel, TReaderModel>(TEidtModel model)
            where TEidtModel : IEditModel<TReaderModel>
            where TReaderModel : IReaderModel;

        TCreateModel GetCreate<TCreateModel, TReaderModel>()
            where TCreateModel : ICreatedModel<TReaderModel>
            where TReaderModel : IReaderModel;

        TReaderModel Create<TCreateModel, TReaderModel>(TCreateModel model)
            where TCreateModel : ICreatedModel<TReaderModel>
            where TReaderModel : IReaderModel;

        TReaderModel Delete<TReaderModel>(Guid id);

    }
}
