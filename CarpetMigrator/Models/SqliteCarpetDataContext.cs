namespace CarpetMigrator.Models
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    ///
    /// </summary>
    public class SqliteCarpetDataContext : DbContext
    {
        private string Path { get; set; }

        public SqliteCarpetDataContext(string path)
        {
            Path = path;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={Path}");
        }

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
