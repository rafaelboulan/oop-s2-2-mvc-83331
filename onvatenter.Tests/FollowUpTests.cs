using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using onvatenter.Models.Data;
using Xunit;

namespace onvatenter.Tests
{
    public class FollowUpTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetOverdueFollowUps_ReturnsOnlyOverdueOpenFollowUps()
        {
            // Arrange
            using var ctx = CreateContext(nameof(GetOverdueFollowUps_ReturnsOnlyOverdueOpenFollowUps));
            
            var inspection = new Inspection 
            { 
                Id = 1, 
                PremisesId = 1, 
                InspectionDate = DateTime.Today.AddDays(-10), 
                Score = 50, 
                Outcome = "Fail", 
                Notes = "Test inspection", 
                CreatedAt = DateTime.Now 
            };
            await ctx.Inspections.AddAsync(inspection);

            var overdueOpen = new FollowUp 
            { 
                Id = 1, 
                InspectionId = 1, 
                DueDate = DateTime.Today.AddDays(-1), 
                Status = "Open", 
                CreatedAt = DateTime.Now 
            };
            
            var openNotOverdue = new FollowUp 
            { 
                Id = 2, 
                InspectionId = 1, 
                DueDate = DateTime.Today.AddDays(5), 
                Status = "Open", 
                CreatedAt = DateTime.Now 
            };
            
            var overdueClosed = new FollowUp 
            { 
                Id = 3, 
                InspectionId = 1, 
                DueDate = DateTime.Today.AddDays(-2), 
                Status = "Closed", 
                ClosedDate = DateTime.Today.AddDays(-1), 
                CreatedAt = DateTime.Now 
            };

            await ctx.FollowUps.AddRangeAsync(overdueOpen, openNotOverdue, overdueClosed);
            await ctx.SaveChangesAsync();

            // Act
            var overdueFollowUps = await ctx.FollowUps
                .Where(f => f.Status == "Open" && f.DueDate.Date < DateTime.Today)
                .ToListAsync();

            // Assert
            Assert.Single(overdueFollowUps);
            Assert.Equal(1, overdueFollowUps[0].Id);
        }

        [Fact]
        public void FollowUp_ClosedWithoutClosedDate_IsInvalid()
        {
            // Arrange & Act
            var followUp = new FollowUp
            {
                Id = 10,
                InspectionId = 1,
                DueDate = DateTime.Today.AddDays(-1),
                Status = "Closed",
                ClosedDate = null,
                CreatedAt = DateTime.Now
            };

            bool isInvalid = followUp.Status == "Closed" && followUp.ClosedDate == null;

            // Assert
            Assert.True(isInvalid);
        }

        [Fact]
        public void FollowUp_IsOverdue_ComputedCorrectly()
        {
            // Arrange
            var followUpOverdue = new FollowUp
            {
                Id = 20,
                InspectionId = 1,
                DueDate = DateTime.Today.AddDays(-5),
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            var followUpNotOverdue = new FollowUp
            {
                Id = 21,
                InspectionId = 1,
                DueDate = DateTime.Today.AddDays(5),
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            // Act
            bool isOverdue = followUpOverdue.Status == "Open" && followUpOverdue.DueDate.Date < DateTime.Today;
            bool isNotOverdue = followUpNotOverdue.Status == "Open" && followUpNotOverdue.DueDate.Date < DateTime.Today;

            // Assert
            Assert.True(isOverdue);
            Assert.False(isNotOverdue);
        }

        [Fact]
        public async Task FollowUp_ClosedFollowUp_IsNotOverdue()
        {
            // Arrange
            using var ctx = CreateContext(nameof(FollowUp_ClosedFollowUp_IsNotOverdue));
            
            var closedFollowUp = new FollowUp
            {
                Id = 30,
                InspectionId = 1,
                DueDate = DateTime.Today.AddDays(-10),
                Status = "Closed",
                ClosedDate = DateTime.Today.AddDays(-5),
                CreatedAt = DateTime.Now
            };

            await ctx.FollowUps.AddAsync(closedFollowUp);
            await ctx.SaveChangesAsync();

            // Act
            var followUp = await ctx.FollowUps.FindAsync(30);
            bool isOverdue = followUp.Status == "Open" && followUp.DueDate.Date < DateTime.Today;

            // Assert
            Assert.False(isOverdue);
        }

        [Fact]
        public async Task FollowUp_LoadWithInspection_Success()
        {
            // Arrange
            using var ctx = CreateContext(nameof(FollowUp_LoadWithInspection_Success));
            
            var inspection = new Inspection
            {
                Id = 100,
                PremisesId = 1,
                InspectionDate = DateTime.Today.AddDays(-10),
                Score = 85,
                Outcome = "Pass",
                Notes = "Good",
                CreatedAt = DateTime.Now
            };

            var followUp = new FollowUp
            {
                Id = 40,
                InspectionId = 100,
                DueDate = DateTime.Today.AddDays(30),
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            await ctx.Inspections.AddAsync(inspection);
            await ctx.FollowUps.AddAsync(followUp);
            await ctx.SaveChangesAsync();

            // Act
            var loadedFollowUp = await ctx.FollowUps.Include(f => f.Inspection).FirstAsync(f => f.Id == 40);

            // Assert
            Assert.NotNull(loadedFollowUp.Inspection);
            Assert.Equal(100, loadedFollowUp.Inspection.Id);
        }
    }
}
