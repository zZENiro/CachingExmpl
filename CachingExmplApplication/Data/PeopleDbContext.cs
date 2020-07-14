using CachingExmplApplication.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CachingExmplApplication.Data
{
    public class PeopleDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public PeopleDbContext(DbContextOptions<PeopleDbContext> options) : base(options) =>
            Database.EnsureCreated();

        public PeopleDbContext() =>
            Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost; Database=cacingExample; Trusted_Connection=True;");
        }
    }
}
