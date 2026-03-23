using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace onvatenter.Models.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Premises> Premises => Set<Premises>();
        public DbSet<Inspection> Inspections => Set<Inspection>();
        public DbSet<FollowUp> FollowUps => Set<FollowUp>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Foreign Keys
            builder.Entity<Inspection>()
                .HasOne(i => i.Premises)
                .WithMany(p => p.Inspections)
                .HasForeignKey(i => i.PremisesId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FollowUp>()
                .HasOne(f => f.Inspection)
                .WithMany(i => i.FollowUps)
                .HasForeignKey(f => f.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed 12 Premises
            builder.Entity<Premises>().HasData(
                new Premises { Id = 1, Name = "Restaurant Le Gourmet", Address = "123 Rue de Paris", Town = "Paris", RiskRating = "High", CreatedAt = DateTime.Now },
                new Premises { Id = 2, Name = "Café du Coin", Address = "456 Rue de Lyon", Town = "Lyon", RiskRating = "Medium", CreatedAt = DateTime.Now },
                new Premises { Id = 3, Name = "Boulangerie Artisanale", Address = "789 Rue de Marseille", Town = "Marseille", RiskRating = "Low", CreatedAt = DateTime.Now },
                new Premises { Id = 4, Name = "Supermarché Central", Address = "321 Rue de Toulouse", Town = "Toulouse", RiskRating = "High", CreatedAt = DateTime.Now },
                new Premises { Id = 5, Name = "Pizzeria Milano", Address = "654 Rue de Nice", Town = "Nice", RiskRating = "Medium", CreatedAt = DateTime.Now },
                new Premises { Id = 6, Name = "Salon de Thé", Address = "987 Rue de Bordeaux", Town = "Bordeaux", RiskRating = "Low", CreatedAt = DateTime.Now },
                new Premises { Id = 7, Name = "Restaurant Asiatique", Address = "111 Rue de Nantes", Town = "Nantes", RiskRating = "High", CreatedAt = DateTime.Now },
                new Premises { Id = 8, Name = "Bar à Vin", Address = "222 Rue de Lille", Town = "Lille", RiskRating = "Low", CreatedAt = DateTime.Now },
                new Premises { Id = 9, Name = "Épicerie Fine", Address = "333 Rue de Strasbourg", Town = "Strasbourg", RiskRating = "Medium", CreatedAt = DateTime.Now },
                new Premises { Id = 10, Name = "Food Truck", Address = "444 Rue de Reims", Town = "Reims", RiskRating = "High", CreatedAt = DateTime.Now },
                new Premises { Id = 11, Name = "Cantine d'Entreprise", Address = "555 Rue du Havre", Town = "Le Havre", RiskRating = "Medium", CreatedAt = DateTime.Now },
                new Premises { Id = 12, Name = "Brasserie Traditionnelle", Address = "666 Rue de Rennes", Town = "Rennes", RiskRating = "Low", CreatedAt = DateTime.Now }
            );

            // Seed 25 Inspections
            for (int i = 1; i <= 25; i++)
            {
                builder.Entity<Inspection>().HasData(new Inspection
                {
                    Id = i,
                    PremisesId = ((i - 1) % 12) + 1,
                    InspectionDate = DateTime.Now.AddDays(-(i * 5)),
                    Score = 70 + (i % 30),
                    Outcome = i % 3 == 0 ? "Pass" : (i % 3 == 1 ? "Pass with Observations" : "Fail"),
                    Notes = $"Inspection #{i}",
                    CreatedAt = DateTime.Now.AddDays(-(i * 5))
                });
            }

            // Seed 10 Follow-ups
            for (int i = 1; i <= 10; i++)
            {
                builder.Entity<FollowUp>().HasData(new FollowUp
                {
                    Id = i,
                    InspectionId = i,
                    DueDate = DateTime.Now.AddDays(30),
                    Status = i % 2 == 0 ? "Pending" : "Completed",
                    ClosedDate = i % 2 == 0 ? null : DateTime.Now,
                    CreatedAt = DateTime.Now.AddDays(-(i * 5))
                });
            }
        }
    }

    public class Premises
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string RiskRating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    }

    public class Inspection
    {
        public int Id { get; set; }
        public int PremisesId { get; set; }
        public DateTime InspectionDate { get; set; }
        public int Score { get; set; }
        public string Outcome { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Premises Premises { get; set; }
        public ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();
    }

    public class FollowUp
    {
        public int Id { get; set; }
        public int InspectionId { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Inspection Inspection { get; set; }
    }
}
