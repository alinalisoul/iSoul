using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSoul.Model
{
    public interface IViewModel
    {
        Guid Identity { get; set; }

        int DisplayIdentity { get; set; }

        DateTime CreatedTime { get; set; }

        DateTime UpdatedTime { get; set; }

        DateTime? DeletedTime { get; set; }

        bool IsDeleted { get; set; }
    }
}
