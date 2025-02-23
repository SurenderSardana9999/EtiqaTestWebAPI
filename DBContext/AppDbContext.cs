using EtiqaTestAPI.Entity;
using Microsoft.EntityFrameworkCore;

namespace EtiqaTestAPI.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Freelancer> Freelancers { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
    }
}
