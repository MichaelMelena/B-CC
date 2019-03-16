using BCC.Model.Models;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace BCC.Model.Models
{
    public partial class BCCContext
    {
        private static string ConnectionString;
        private static IConfiguration Configuration;
        public BCCContext()
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

        public BCCContext(DbContextOptions<BCCContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Configuration.GetValue<string>("connectionStrings:add:bccLocal:connectionString");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}

