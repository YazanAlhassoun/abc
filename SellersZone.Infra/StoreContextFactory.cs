using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Infra
{
    internal class StoreContextFactory : IDesignTimeDbContextFactory<StoreContext>
    {
        public StoreContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../SellersZone.Server");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath) // Base path of the startup project
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                 .AddEnvironmentVariables() // Include environment variables
                .Build();
            // Create DbContextOptions using the configuration
            var optionsBuilder = new DbContextOptionsBuilder<StoreContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            // Return the context with the built options
            return new StoreContext(optionsBuilder.Options);
        }
    }
}
