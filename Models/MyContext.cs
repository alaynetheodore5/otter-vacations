using System;
using Microsoft.EntityFrameworkCore;

namespace OtterVacations.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options){}

        public DbSet<Otter> Otters { get; set; }

        public DbSet<Vacation> Vacations { get; set; }

        public DbSet<Association> Associations { get; set; }
    }
}