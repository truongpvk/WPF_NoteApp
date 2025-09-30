namespace DoAnNote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NoteAttachment
    {
        [Key]
        public int AttachmentID { get; set; }

        public int NoteID { get; set; }

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Note Note { get; set; }
    }
}
