using Microsoft.EntityFrameworkCore;

namespace int3306
{
    public class DataDbContext : DbContext
    {
        public DataDbContext() {}
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Certificate>()
                .HasKey(certificate => new { certificate.Timestamp, certificate.ShopId, certificate.TransactionType });
        }

        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
    }
}