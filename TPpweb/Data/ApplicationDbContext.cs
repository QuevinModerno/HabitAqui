using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TPpweb.Models;

namespace TPpweb.Data
{
    public class ApplicationDbContext : IdentityDbContext<Person>
    {
        public DbSet<Housing> Houses { get; set; }
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Rating> Rates { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Employer> Employer { get; set; }
        public DbSet<DeliveryDetails> Details { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DeliveryDetails>()
            .HasOne(d => d.Reservation)
            .WithOne(r => r.Delivery)
            .HasForeignKey<DeliveryDetails>(d => d.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(builder);

            // Desativar a coluna Discriminator
            builder.Entity<Person>().ToTable("AspNetUsers").HasNoDiscriminator();

        }
    }
}