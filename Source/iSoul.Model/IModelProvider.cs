using System;
using System.Collections.Generic;

namespace iSoul.Model
{
    public interface IModelProvider
    {
        bool CanProvider(Type type);
    }
}
