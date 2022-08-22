namespace CarpetPlannerB2c.Models
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    ///
    /// </summary>
    public class CarpetDataContext : DbContext
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="options"></param>
        public CarpetDataContext(DbContextOptions<CarpetDataContext> options) : base(options)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CarpetEntity>()
                .Property(carpet => carpet.Width)
                .HasDefaultValue(100);
        }

        /// <summary>
        ///
        /// </summary>
        public DbSet<AliasEntity> Aliases { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DbSet<CarpetEntity> Carpets { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DbSet<StripeEntity> Stripes { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DbSet<ColorEntity> Colors { get; set; }
    }
}
