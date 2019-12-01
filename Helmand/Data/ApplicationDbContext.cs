using System;
using System.Collections.Generic;
using System.Text;
using Helmand.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Helmand.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category>Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
    }
}
