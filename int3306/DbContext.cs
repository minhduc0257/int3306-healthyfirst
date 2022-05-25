using Microsoft.EntityFrameworkCore;

namespace int3306
{
    public class DataDbContext : DbContext
    {
        public DataDbContext() {}
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ward>()
                .HasOne(w => w.District)
                .WithMany(d => d.Wards);
            modelBuilder.Entity<Certificate>()
                .HasOne(ce => ce.Shop)
                .WithMany(sh => sh.Certificates);
            
            modelBuilder.Entity<Activity>()
                .HasOne(ce => ce.Plan)
                .WithMany(sh => sh.Activities);
        }

        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<Activity> Activity { get; set; }
    }
}