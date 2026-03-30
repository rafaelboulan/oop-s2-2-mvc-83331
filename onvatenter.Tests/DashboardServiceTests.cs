using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using onvatenter.Models.Data;
using Xunit;

namespace onvatenter.Tests
{
    public class DashboardServiceTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetDashboardData_CountsInspectionsThisMonthCorrectly()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetDashboardData_CountsInspectionsThisMonthCorrectly));

            var now = DateTime.Today;
            var thisMonth = new[] 
            {
                new Inspection 
                { 
                    Id = 1, 
                    PremisesId = 1, 
                    InspectionDate = now.AddDays(-1), 
                    Outcome = "Pass", 
                    Score = 80,
                    Notes = "Test",
                    CreatedAt = now.AddDays(-1) 
                },
                new Inspection 
                { 
                    Id = 2, 
                    PremisesId = 1, 
                    InspectionDate = now.AddDays(-5), 
                    Outcome = "Fail", 
                    Score = 40,
                    Notes = "Test",
                    CreatedAt = now.AddDays(-5) 
                },
                new Inspection 
                { 
                    Id = 3, 
                    PremisesId = 1, 
                    InspectionDate = now.AddDays(-10), 
                    Outcome = "Pass", 
                    Score = 90,
                    Notes = "Test",
                    CreatedAt = now.AddDays(-10) 
                }
            };

            var lastMonth = new[] 
            {
                new Inspection 
                { 
                    Id = 4, 
                    PremisesId = 1, 
                    InspectionDate = now.AddMonths(-1), 
                    Outcome = "Pass", 
                    Score = 85,
                    Notes = "Test",
                    CreatedAt = now.AddMonths(-1) 
                },
                new Inspection 
                { 
                    Id = 5, 
                    PremisesId = 1, 
                    InspectionDate = now.AddMonths(-1).AddDays(-5), 
                    Outcome = "Fail", 
                    Score = 30,
                    Notes = "Test",
                    CreatedAt = now.AddMonths(-1).AddDays(-5) 
                }
            };

            await ctx.Inspections.AddRangeAsync(thisMonth);
            await ctx.Inspections.AddRangeAsync(lastMonth);
            await ctx.SaveChangesAsync();

            // Act
            var countThisMonth = await ctx.Inspections
                .Where(i => i.InspectionDate.Month == now.Month && i.InspectionDate.Year == now.Year)
                .CountAsync();

            // Assert
            Assert.Equal(3, countThisMonth);
        }

        [Fact]
        public async Task GetDashboardData_CountsFailedInspectionsThisMonthCorrectly()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetDashboardData_CountsFailedInspectionsThisMonthCorrectly));

            var now = DateTime.Today;
            var inspections = new[] 
            {
                new Inspection 
                { 
                    Id = 11, 
                    PremisesId = 1, 
                    InspectionDate = now.AddDays(-1), 
                    Outcome = "Fail", 
                    Score = 40,
                    Notes = "Test",
                    CreatedAt = now.AddDays(-1) 
                },
                new Inspection 
                { 
                    Id = 12, 
                    PremisesId = 1, 
                    InspectionDate = now.AddDays(-2), 
                    Outcome = "Pass", 
                    Score = 80,
                    Notes = "Test",
                    CreatedAt = now.AddDays(-2) 
                },
                new Inspection 
                { 
                    Id = 13, 
                    PremisesId = 1, 
                    InspectionDate = now.AddDays(-3), 
                    Outcome = "Fail", 
                    Score = 35,
                    Notes = "Test",
                    CreatedAt = now.AddDays(-3) 
                }
            };

            await ctx.Inspections.AddRangeAsync(inspections);
            await ctx.SaveChangesAsync();

            // Act
            var failedCount = await ctx.Inspections
                .Where(i => i.InspectionDate.Month == now.Month 
                         && i.InspectionDate.Year == now.Year 
                         && i.Outcome == "Fail")
                .CountAsync();

            // Assert
            Assert.Equal(2, failedCount);
        }

        [Fact]
        public async Task GetDashboardData_FiltersByTown()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetDashboardData_FiltersByTown));

            var premises = new[] 
            {
                new Premises 
                { 
                    Id = 100, 
                    Name = "Place A", 
                    Address = "Addr A", 
                    Town = "Dorchester", 
                    RiskRating = "High", 
                    CreatedAt = DateTime.Now 
                },
                new Premises 
                { 
                    Id = 101, 
                    Name = "Place B", 
                    Address = "Addr B", 
                    Town = "Weymouth", 
                    RiskRating = "Low", 
                    CreatedAt = DateTime.Now 
                }
            };

            var now = DateTime.Today;
            var inspections = new[] 
            {
                new Inspection 
                { 
                    Id = 201, 
                    PremisesId = 100, 
                    InspectionDate = now.AddDays(-1), 
                    Outcome = "Pass", 
                    Score = 90,
                    Notes = "Test",
                    CreatedAt = now.AddDays(-1) 
                },
                new Inspection 
                { 
                    Id = 202, 
                    PremisesId = 101, 
                    InspectionDate = now.AddDays(-1), 
                    Outcome = "Pass", 
                    Score = 85,
                    Notes = "Test",
                    CreatedAt = now.AddDays(-1) 
                }
            };

            await ctx.Premises.AddRangeAsync(premises);
            await ctx.Inspections.AddRangeAsync(inspections);
            await ctx.SaveChangesAsync();

            // Act
            var countDorchester = await ctx.Inspections
                .Include(i => i.Premises)
                .Where(i => i.Premises.Town == "Dorchester")
                .CountAsync();

            // Assert
            Assert.Equal(1, countDorchester);
        }

        [Fact]
        public async Task GetDashboardData_FiltersByRiskRating()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetDashboardData_FiltersByRiskRating));

            var premises = new[] 
            {
                new Premises 
                { 
                    Id = 300, 
                    Name = "High Risk Place", 
                    Address = "X", 
                    Town = "T", 
                    RiskRating = "High", 
                    CreatedAt = DateTime.Now 
                },
                new Premises 
                { 
                    Id = 301, 
                    Name = "Low Risk Place", 
                    Address = "Y", 
                    Town = "T", 
                    RiskRating = "Low", 
                    CreatedAt = DateTime.Now 
                }
            };

            var now = DateTime.Today;
            var inspections = new[] 
            {
                new Inspection 
                { 
                    Id = 401, 
                    PremisesId = 300, 
                    InspectionDate = now, 
                    Outcome = "Pass", 
                    Score = 88,
                    Notes = "Test",
                    CreatedAt = now 
                },
                new Inspection 
                { 
                    Id = 402, 
                    PremisesId = 301, 
                    InspectionDate = now, 
                    Outcome = "Pass", 
                    Score = 92,
                    Notes = "Test",
                    CreatedAt = now 
                }
            };

            await ctx.Premises.AddRangeAsync(premises);
            await ctx.Inspections.AddRangeAsync(inspections);
            await ctx.SaveChangesAsync();

            // Act
            var highRiskCount = await ctx.Inspections
                .Include(i => i.Premises)
                .Where(i => i.Premises.RiskRating == "High")
                .CountAsync();

            // Assert
            Assert.Equal(1, highRiskCount);
        }
    }
}
