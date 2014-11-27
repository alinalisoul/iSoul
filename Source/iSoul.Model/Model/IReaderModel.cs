using System;

namespace iSoul.Model
{
    public interface IReaderModel
    {
        Guid Identity { get; set; }

        int DisplayIdentity { get; set; }

        DateTime CreatedTime { get; set; }

        DateTime UpdatedTime { get; set; }

        DateTime? DeletedTime { get; set; }

        bool IsDeleted { get; set; }
    }
}
