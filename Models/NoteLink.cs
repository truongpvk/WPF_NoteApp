namespace DoAnNote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NoteLink
    {
        [Key]
        public int LinkID { get; set; }

        public int NoteID { get; set; }

        [Required]
        [StringLength(500)]
        public string URL { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Note Note { get; set; }
    }
}
