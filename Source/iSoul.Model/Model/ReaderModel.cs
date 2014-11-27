using System;

namespace iSoul.Model
{
    public class ReaderModel : IReaderModel
    {
        #region IReaderModel 成员

        public Guid Identity { get; set; }

        public int DisplayIdentity { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime { get; set; }

        public DateTime? DeletedTime { get; set; }

        public bool IsDeleted { get; set; }

        #endregion
    }
}
