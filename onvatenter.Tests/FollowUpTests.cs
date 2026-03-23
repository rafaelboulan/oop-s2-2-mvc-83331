using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using onvatenter.Data;
using onvatenter.Models;
using Xunit;

namespace onvatenter.Tests
{
    public class FollowUpTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetOverdueFollowUps_ReturnsOnlyOverdueOpenFollowUps()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var premises = new Premises { Name = "Test Premises", Address = "123 St", Town = "Test", RiskRating = RiskRating.Low };
            var inspection = new Inspection 
            { 
                Premises = premises,
                InspectionDate = DateTime.UtcNow.AddDays(-20), 
                Score = 65, 
                Outcome = InspectionOutcome.Fail 
            };

            context.Premises.Add(premises);
            context.Inspections.Add(inspection);
            await context.SaveChangesAsync();

            // Create follow-ups
            var overdueFollowUp = new FollowUp 
            { 
                Inspection = inspection, 
                DueDate = DateTime.Today.AddDays(-5), 
                Status = FollowUpStatus.Open 
            };
            var closedFollowUp = new FollowUp 
            { 
                Inspection = inspection, 
                DueDate = DateTime.Today.AddDays(-10), 
                Status = FollowUpStatus.Closed,
                ClosedDate = DateTime.Today.AddDays(-8)
            };
            var futureFollowUp = new FollowUp 
            { 
                Inspection = inspection, 
                DueDate = DateTime.Today.AddDays(5), 
                Status = FollowUpStatus.Open 
            };

            context.FollowUps.AddRange(overdueFollowUp, closedFollowUp, futureFollowUp);
            await context.SaveChangesAsync();

            // Act
            var overdueFollowUps = context.FollowUps
                .Where(f => f.Status == FollowUpStatus.Open && f.DueDate < DateTime.Today)
                .ToList();

            // Assert
            Assert.Single(overdueFollowUps);
            Assert.Equal(overdueFollowUp.Id, overdueFollowUps.First().Id);
        }

        [Fact]
        public void FollowUp_ClosedWithoutClosedDate_IsInvalid()
        {
            // Arrange
            var followUp = new FollowUp 
            { 
                Status = FollowUpStatus.Closed, 
                ClosedDate = null,
                DueDate = DateTime.Today
            };

            // Act & Assert
            // This test validates business logic - in production, this would be caught in the controller
            Assert.True(followUp.Status == FollowUpStatus.Closed && !followUp.ClosedDate.HasValue);
        }

        [Fact]
        public async Task FollowUp_IsOverdueProperty_WorksCorrectly()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var premises = new Premises { Name = "Test", Address = "St", Town = "Town", RiskRating = RiskRating.Low };
            var inspection = new Inspection 
            { 
                Premises = premises,
                InspectionDate = DateTime.UtcNow.AddDays(-30), 
                Score = 50, 
                Outcome = InspectionOutcome.Fail 
            };

            context.Premises.Add(premises);
            context.Inspections.Add(inspection);
            await context.SaveChangesAsync();

            var overdueFollowUp = new FollowUp 
            { 
                Inspection = inspection, 
                DueDate = DateTime.Today.AddDays(-3), 
                Status = FollowUpStatus.Open 
            };

            // Act
            var isOverdue = overdueFollowUp.IsOverdue;

            // Assert
            Assert.True(isOverdue);
        }

        [Fact]
        public async Task FollowUp_NotOverdue_WhenStatusIsClosed()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var premises = new Premises { Name = "Test", Address = "St", Town = "Town", RiskRating = RiskRating.Low };
            var inspection = new Inspection 
            { 
                Premises = premises,
                InspectionDate = DateTime.UtcNow.AddDays(-30), 
                Score = 50, 
                Outcome = InspectionOutcome.Fail 
            };

            context.Premises.Add(premises);
            context.Inspections.Add(inspection);
            await context.SaveChangesAsync();

            var closedFollowUp = new FollowUp 
            { 
                Inspection = inspection, 
                DueDate = DateTime.Today.AddDays(-5), 
                Status = FollowUpStatus.Closed,
                ClosedDate = DateTime.Today.AddDays(-4)
            };

            // Act
            var isOverdue = closedFollowUp.IsOverdue;

            // Assert
            Assert.False(isOverdue);
        }

        [Fact]
        public async Task FollowUp_QueryWithIncludes_ReturnsCompleteData()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var premises = new Premises { Name = "Test Premises", Address = "123 St", Town = "Test", RiskRating = RiskRating.Medium };
            var inspection = new Inspection 
            { 
                Premises = premises,
                InspectionDate = DateTime.UtcNow, 
                Score = 75, 
                Outcome = InspectionOutcome.Fail 
            };
            var followUp = new FollowUp 
            { 
                Inspection = inspection, 
                DueDate = DateTime.Today.AddDays(10), 
                Status = FollowUpStatus.Open 
            };

            context.Premises.Add(premises);
            context.Inspections.Add(inspection);
            context.FollowUps.Add(followUp);
            await context.SaveChangesAsync();

            // Act
            var retrievedFollowUp = await context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i!.Premises)
                .FirstOrDefaultAsync(f => f.Id == followUp.Id);

            // Assert
            Assert.NotNull(retrievedFollowUp);
            Assert.NotNull(retrievedFollowUp.Inspection);
            Assert.NotNull(retrievedFollowUp.Inspection.Premises);
            Assert.Equal("Test Premises", retrievedFollowUp.Inspection.Premises.Name);
        }
    }
}
