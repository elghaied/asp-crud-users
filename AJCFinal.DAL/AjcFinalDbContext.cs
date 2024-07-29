using AJCFinal.DAL.Entites;
using Microsoft.EntityFrameworkCore;

namespace AJCFinal.DAL
{
    public sealed class AjcFinalDbContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Person> Persons { get; set; }

        public AjcFinalDbContext(DbContextOptions<AjcFinalDbContext> options)
            : base(options)
        {
        }
    }
}
