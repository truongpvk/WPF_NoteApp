namespace DoAnNote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Note
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Note()
        {
            NoteAttachments = new HashSet<NoteAttachment>();
            NoteLinks = new HashSet<NoteLink>();
        }

        public int NoteID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        public string Summary { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [StringLength(10)]
        public string Background { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoteAttachment> NoteAttachments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoteLink> NoteLinks { get; set; }

        public virtual User User { get; set; }
    }
}
