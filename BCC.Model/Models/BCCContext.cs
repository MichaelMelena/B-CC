using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace BCC.Model.Models
{
    public partial class BCCContext : DbContext
    {
        private static string ConnectionString;
        private static IConfiguration Configuration;
        static  BCCContext()
        {

            var builder = new ConfigurationBuilder()
                .AddXmlFile(".\\App.config");

            Configuration = builder.Build();

            string databaseEnv = Environment.GetEnvironmentVariable("DATABSE_ENV") ?? "Local";
            switch (databaseEnv)
            {
                case "Local":
                    ConnectionString = Configuration.GetValue<string>("connectionStrings:add:bccLocal:connectionString");
                    break;
                case "Development":
                    ConnectionString = Configuration.GetValue<string>("connectionStrings:add:bccDevelopment:connectionString");
                    break;
            }
        }

        public BCCContext()
        {

        }

        public BCCContext(DbContextOptions<BCCContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<CurrencyMetadata> CurrencyMetadata { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<TrackedCurrency> TrackedCurrency { get; set; }
        public virtual DbSet<User> User { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = Configuration.GetValue<string>("connectionStrings:add:bccLocal:connectionString");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("bank");

                entity.Property(e => e.Id).HasColumnName("id");

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

                entity.Property(e => e.ShortName)
                    .HasColumnName("short_Name")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");
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
                    .HasConstraintName("fk_currency_iso_name");
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

                entity.HasIndex(e => new { e.BankId, e.Date })
                    .HasName("unique_bank_and_date")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BankId).HasColumnName("bank_id");

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

                entity.HasOne(d => d.Bank)
                    .WithMany(p => p.Ticket)
                    .HasForeignKey(d => d.BankId)
                    .HasConstraintName("fk_ticket_bank_id");
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
