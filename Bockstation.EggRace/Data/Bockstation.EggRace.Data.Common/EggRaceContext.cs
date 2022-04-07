using Bockstation.EggRace.Data.Common.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Bockstation.EggRace.Data.Common
{
    public class EggRaceContext : DbContext
    {
        #region Properties
        public DbSet<Player> Players { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Team> Teams { get; set; }
        #endregion Properties

        #region Methods
        #region Overridden
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "EggRace.db"
            };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }
        #endregion Overridden
        #endregion Methods
    }
}
