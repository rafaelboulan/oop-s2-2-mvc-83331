using Microsoft.EntityFrameworkCore;
using onvatenter.Data;
using onvatenter.Models;
using onvatenter.Services;
using Xunit;

namespace onvatenter.Tests
{
    public class DashboardServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetDashboardData_CountsInspectionsThisMonthCorrectly()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DashboardService(context);

            var premises = new Premises { Name = "Test", Address = "St", Town = "Dorchester", RiskRating = RiskRating.Low };
            context.Premises.Add(premises);
            await context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var thisMonthInspection = new Inspection 
            { 
                PremisesId = premises.Id,
                InspectionDate = monthStart.AddDays(5), 
                Score = 80, 
                Outcome = InspectionOutcome.Pass 
            };
            var lastMonthInspection = new Inspection 
            { 
                PremisesId = premises.Id,
                InspectionDate = monthStart.AddDays(-5), 
                Score = 75, 
                Outcome = InspectionOutcome.Pass 
            };

            context.Inspections.AddRange(thisMonthInspection, lastMonthInspection);
            await context.SaveChangesAsync();

            // Act
            var dashboard = await service.GetDashboardDataAsync();

            // Assert
            Assert.Equal(1, dashboard.InspectionsThisMonth);
        }

        [Fact]
        public async Task GetDashboardData_CountsFailedInspectionsThisMonthCorrectly()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DashboardService(context);

            var premises = new Premises { Name = "Test", Address = "St", Town = "Weymouth", RiskRating = RiskRating.Medium };
            context.Premises.Add(premises);
            await context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var passInspection = new Inspection 
            { 
                PremisesId = premises.Id,
                InspectionDate = monthStart.AddDays(5), 
                Score = 90, 
                Outcome = InspectionOutcome.Pass 
            };
            var failInspection = new Inspection 
            { 
                PremisesId = premises.Id,
                InspectionDate = monthStart.AddDays(10), 
                Score = 45, 
                Outcome = InspectionOutcome.Fail 
            };

            context.Inspections.AddRange(passInspection, failInspection);
            await context.SaveChangesAsync();

            // Act
            var dashboard = await service.GetDashboardDataAsync();

            // Assert
            Assert.Equal(1, dashboard.FailedInspectionsThisMonth);
        }

        [Fact]
        public async Task GetDashboardData_FiltersBy Town()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DashboardService(context);

            var premisesDorchester = new Premises { Name = "Dorchester Premises", Address = "St", Town = "Dorchester", RiskRating = RiskRating.Low };
            var premisesWeymouth = new Premises { Name = "Weymouth Premises", Address = "St", Town = "Weymouth", RiskRating = RiskRating.Low };

            context.Premises.AddRange(premisesDorchester, premisesWeymouth);
            await context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var inspectionDorchester = new Inspection 
            { 
                PremisesId = premisesDorchester.Id,
                InspectionDate = monthStart.AddDays(5), 
                Score = 80, 
                Outcome = InspectionOutcome.Pass 
            };
            var inspectionWeymouth = new Inspection 
            { 
                PremisesId = premisesWeymouth.Id,
                InspectionDate = monthStart.AddDays(10), 
                Score = 85, 
                Outcome = InspectionOutcome.Pass 
            };

            context.Inspections.AddRange(inspectionDorchester, inspectionWeymouth);
            await context.SaveChangesAsync();

            // Act
            var dashboard = await service.GetDashboardDataAsync(town: "Dorchester");

            // Assert
            Assert.Equal(1, dashboard.InspectionsThisMonth);
        }

        [Fact]
        public async Task GetDashboardData_FiltersBy RiskRating()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DashboardService(context);

            var premisesLow = new Premises { Name = "Low Risk", Address = "St", Town = "Dorchester", RiskRating = RiskRating.Low };
            var premisesHigh = new Premises { Name = "High Risk", Address = "St", Town = "Dorchester", RiskRating = RiskRating.High };

            context.Premises.AddRange(premisesLow, premisesHigh);
            await context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var inspectionLow = new Inspection 
            { 
                PremisesId = premisesLow.Id,
                InspectionDate = monthStart.AddDays(5), 
                Score = 80, 
                Outcome = InspectionOutcome.Pass 
            };
            var inspectionHigh = new Inspection 
            { 
                PremisesId = premisesHigh.Id,
                InspectionDate = monthStart.AddDays(10), 
                Score = 50, 
                Outcome = InspectionOutcome.Fail 
            };

            context.Inspections.AddRange(inspectionLow, inspectionHigh);
            await context.SaveChangesAsync();

            // Act
            var dashboard = await service.GetDashboardDataAsync(riskRating: RiskRating.High);

            // Assert
            Assert.Equal(1, dashboard.InspectionsThisMonth);
            Assert.Equal(1, dashboard.FailedInspectionsThisMonth);
        }
    }
}
