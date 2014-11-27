﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSoul.Model
{
    public interface IEditModel<TReaderModel> : IWriteModel where TReaderModel : IReaderModel
    {
        Guid Identity { get; set; }
    }
}
