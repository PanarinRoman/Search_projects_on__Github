using System;
using Search_projects_on__Github.Models.Search;
using Microsoft.EntityFrameworkCore;


namespace Search_projects_on__Github.Models.MySql
{
    public class MySqlDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<SearchModel> SearchList { get; set; }

        private readonly string connectionString;
        private readonly Version version;

        public MySqlDbContext(string connectionString, Version version)
        {
            this.connectionString = connectionString;
            this.version = version;

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(version));
        }
    }
}
