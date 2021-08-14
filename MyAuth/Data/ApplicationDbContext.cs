using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyAuth.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<MyAuthUser> MyAuthUsers { get; set; }
        public DbSet<ExternalApp> ExternalApps{ get; set; }
        public DbSet<ExternalAppAuthUser> ExternalAppsAuthUsers{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.BuildModels("MyAuth");

            base.OnModelCreating(builder);
        }
    }
}
