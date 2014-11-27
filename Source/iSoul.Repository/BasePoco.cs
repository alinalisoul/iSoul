using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSoul.Repository
{
    public class BasePoco
    {
        public BasePoco()
        {
            this.Identity = Guid.NewGuid();
            //this.CreatedTime = DateTime.Now;
            //this.UpdatedTime = DateTime.Now;
            //this.DeletedTime = null;
            //this.IsDeleted = false;
        }

        [Key]
        public Guid Identity { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisplayIdentity { get; set; }

        //public DateTime CreatedTime { get; set; }

        public DateTime Updated { get; set; }

        //public DateTime? DeletedTime { get; set; }

        public bool IsDelete { get; set; }
    }
}
