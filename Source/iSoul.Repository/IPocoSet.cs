using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSoul.Repository
{
    public interface IPocoSet<TPoco> where TPoco : BasePoco
    {
        IQueryable<TPoco> Pocos { get; }
    }
}
