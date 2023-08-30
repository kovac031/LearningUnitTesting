using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace PlayPalMini.DAL
{
    public partial class EFContext : DbContext
    {
        public EFContext()
            : base("name=PlayPalMiniConnection")
        {
        }

        public virtual DbSet<BoardGame> BoardGames { get; set; }
        public virtual DbSet<RegisteredUser> RegisteredUsers { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BoardGame>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<BoardGame>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<BoardGame>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<BoardGame>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<BoardGame>()
                .HasMany(e => e.Reviews)
                .WithRequired(e => e.BoardGame)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RegisteredUser>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<RegisteredUser>()
                .Property(e => e.Pass)
                .IsUnicode(false);

            modelBuilder.Entity<RegisteredUser>()
                .Property(e => e.UserRole)
                .IsUnicode(false);

            modelBuilder.Entity<RegisteredUser>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<RegisteredUser>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Review>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Review>()
                .Property(e => e.Comment)
                .IsUnicode(false);

            modelBuilder.Entity<Review>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Review>()
                .Property(e => e.UpdatedBy)
                .IsUnicode(false);
        }
    }
}
