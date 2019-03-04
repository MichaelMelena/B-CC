using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BCC.Model.Models
{
    public partial class BCCContext : DbContext
    {
        public BCCContext()
        {
        }

        public BCCContext(DbContextOptions<BCCContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data source=localhost;Initial Catalog=BCC;Trusted_Connection=True;User Id=bcc;Password=bcc");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__User__72E12F1A40118123");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .ValueGeneratedNever();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });
        }
    }
}
