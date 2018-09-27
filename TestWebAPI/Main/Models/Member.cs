using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models {
    [Table("M_Members")]
    public class Member {

        [Key]
        [StringLength(20)]
        public string MemberID { get; set; }

        [Required]
        [StringLength(100)]
        public string MemberName { get; set; }
        
        [StringLength(20)]
        public string LoginPassword { get; set; }

        // ForeignKey
        [StringLength(20)]
        public string AuthorityID { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Remarks { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public bool Flag { get; set; }

        [Required]
        public DateTime Update { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Navigation property
        public Authority Authority { get; set; }
    }
}