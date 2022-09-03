namespace CarpetMigrator.Models
{
    using Microsoft.EntityFrameworkCore;
    using MySqlConnector;

    public class MySqlCarpetDataContext : DbContext
    {
        private string Host { get; set; }
        private string DatabaseName { get; set; }
        private string User { get; set; }
        private string Password { get; set; }

        public MySqlCarpetDataContext(string host, string databaseName, string user, string password)
        {
            Host = host;
            DatabaseName = databaseName;
            User = user;
            Password = password;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = Host,
                Database = DatabaseName,
                UserID = User,
                Password = Password
            };

            var connectionString = connectionStringBuilder.ToString();
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        /// <summary>
        ///
        /// </summary>
        public DbSet<MySqlCarpetEntity> Carpets { get; set; }

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
