namespace DoAnNote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Notes = new HashSet<Note>();
        }

        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string PasswordHash { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        public DateTime? CreatedAt { get; set; }

        [StringLength(150)]
        public string DisplayName { get; set; }

        [StringLength(10)]
        public string Background { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Notes { get; set; }
    }
}
