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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<Person>().ToTable("Persons");

            modelBuilder.Entity<Person>()
                .HasMany(p => p.Friends)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "Friendships",
                    j => j
                        .HasOne<Person>()
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j
                        .HasOne<Person>()
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j =>
                    {
                        j.HasKey("PersonId", "FriendId");
                        j.ToTable("Friendships");
                    });


        }
    }
}
