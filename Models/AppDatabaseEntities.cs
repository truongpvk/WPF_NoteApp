using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DoAnNote
{
    public partial class AppDatabaseEntities : DbContext
    {
        public AppDatabaseEntities()
            : base("name=AppDatabaseEntities")
        {
        }

        public virtual DbSet<NoteAttachment> NoteAttachments { get; set; }
        public virtual DbSet<NoteLink> NoteLinks { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>()
                .Property(e => e.Background)
                .IsFixedLength();

            modelBuilder.Entity<Note>()
                .HasMany(e => e.NoteAttachments)
                .WithRequired(e => e.Note)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Note>()
                .HasMany(e => e.NoteLinks)
                .WithRequired(e => e.Note)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.PasswordHash)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Background)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .HasMany(e => e.Notes)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}
