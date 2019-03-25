using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BCC.Model.Models
{
    public partial class BCCContext : DbContext
    {
      
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<BankConnector> BankConnector { get; set; }
        public virtual DbSet<Bednar> Bednar { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<CurrencyMetadata> CurrencyMetadata { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<TrackedCurrency> TrackedCurrency { get; set; }
        public virtual DbSet<User> User { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.HasKey(e => e.ShortName)
                    .HasName("PK__bank__2711634E9E838BB3");

                entity.ToTable("bank");

                entity.HasIndex(e => e.Name)
                    .HasName("UQ__bank__72E12F1B1445BABF")
                    .IsUnique();

                entity.Property(e => e.ShortName)
                    .HasColumnName("short_name")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<BankConnector>(entity =>
            {
                entity.ToTable("bank_connector");

                entity.HasIndex(e => e.Name)
                    .HasName("UQ__bank_con__72E12F1B4110037D")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BankShortName)
                    .IsRequired()
                    .HasColumnName("bank_short_name")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DllName)
                    .IsRequired()
                    .HasColumnName("dll_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasColumnName("enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.BankShortNameNavigation)
                    .WithMany(p => p.BankConnector)
                    .HasForeignKey(d => d.BankShortName)
                    .HasConstraintName("fk_bank_connector_bank");
            });

            modelBuilder.Entity<Bednar>(entity =>
            {
                entity.ToTable("bednar");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Json)
                    .IsRequired()
                    .HasColumnName("json")
                    .HasColumnType("text")
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => new { e.IsoName, e.TicketId })
                    .HasName("pk_currency");

                entity.ToTable("currency");

                entity.Property(e => e.IsoName)
                    .HasColumnName("iso_name")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.TicketId).HasColumnName("ticket_id");

                entity.Property(e => e.Buy).HasColumnName("buy");

                entity.Property(e => e.Sell).HasColumnName("sell");

                entity.HasOne(d => d.IsoNameNavigation)
                    .WithMany(p => p.Currency)
                    .HasForeignKey(d => d.IsoName)
                    .HasConstraintName("fk_currency_to_iso_name");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.Currency)
                    .HasForeignKey(d => d.TicketId)
                    .HasConstraintName("fk_currency_to_ticket_di");
            });

            modelBuilder.Entity<CurrencyMetadata>(entity =>
            {
                entity.HasKey(e => e.IsoName)
                    .HasName("pk_currency_metadata");

                entity.ToTable("currency_metadata");

                entity.Property(e => e.IsoName)
                    .HasColumnName("iso_name")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("ticket");

                entity.HasIndex(e => new { e.BankShortName, e.Date })
                    .HasName("unique_bank_and_date")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BankShortName)
                    .IsRequired()
                    .HasColumnName("bank_short_name")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.BankShortNameNavigation)
                    .WithMany(p => p.Ticket)
                    .HasForeignKey(d => d.BankShortName)
                    .HasConstraintName("fk_ticket_bank_short_name");
            });

            modelBuilder.Entity<TrackedCurrency>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.IsoName })
                    .HasName("pk_tracked_currency");

                entity.ToTable("tracked_currency");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.IsoName)
                    .HasColumnName("iso_name")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.IsoNameNavigation)
                    .WithMany(p => p.TrackedCurrency)
                    .HasForeignKey(d => d.IsoName)
                    .HasConstraintName("fk_tracked_currency_iso_name");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TrackedCurrency)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_tracked_currency_user_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastLogin)
                    .HasColumnName("last_login")
                    .HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });
        }
    }
}
