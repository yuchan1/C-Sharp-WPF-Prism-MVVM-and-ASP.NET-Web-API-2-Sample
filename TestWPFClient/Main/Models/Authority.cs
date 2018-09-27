using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models {
    [Table("M_Authorities")]
    public class Authority {

        [Key]
        [StringLength(20)]
        public string AuthorityID { get; set; }

        [Required]
        [StringLength(100)]
        public string AuthorityName { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public bool Flag { get; set; }

        [Required]
        public DateTime Update { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
