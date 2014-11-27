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
            this.Created = DateTime.Now;
            this.Updated = DateTime.Now;
            this.Deleted = null;
            this.IsDeleted = false;
        }

        [Key]
        public Guid Identity { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisplayIdentity { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public DateTime? Deleted { get; set; }

        public bool IsDeleted { get; set; }
    }
}
