# Food Safety Inspection Tracker - Installation Guide

## Quick Start (5 minutes)

### 1. Prerequisites
```bash
# Verify .NET 9 installation
dotnet --version
```

Should output: `9.0.x`

### 2. Clone & Navigate
```bash
git clone https://github.com/yourusername/oop-s2-2-mvc-xxxx.git
cd oop-s2-2-mvc-xxxx
```

### 3. Restore Dependencies
```bash
dotnet restore
```

### 4. Build Solution
```bash
dotnet build
```

### 5. Run Application
```bash
dotnet run --project onvatenter
```

### 6. Access Application
```
https://localhost:7000
```

### 7. Login with Default Credentials

**Admin User:**
- Email: `admin@foodsafety.local`
- Password: `Admin123!`

**Inspector User:**
- Email: `inspector@foodsafety.local`
- Password: `Inspector123!`

**Viewer User:**
- Email: `viewer@foodsafety.local`
- Password: `Viewer123!`

## What Happens on First Run

1. **Database Created** - SQLite database `foodsafety.db` created in project root
2. **Migrations Applied** - All EF Core migrations executed
3. **Seed Data Inserted** - 12 premises, 25 inspections, 10 follow-ups added
4. **Default Users Created** - Admin, Inspector, Viewer accounts created
5. **Log Directory** - `logs/` directory created for Serilog files

## File Locations

```
Project Root/
├── foodsafety.db          (SQLite database file)
├── logs/
│   └── foodsafety-2024-01-15.txt  (Daily log file)
├── onvatenter/
│   ├── Views/
│   ├── Models/
│   ├── Controllers/
│   └── wwwroot/
└── onvatenter.Tests/
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FollowUpTests"
dotnet test --filter "DashboardServiceTests"
```

### Run with Verbose Output
```bash
dotnet test --verbosity normal
```

## Development vs Production

### Development Mode
```bash
dotnet run --project onvatenter
```
- Serilog logs to console AND file
- DeveloperExceptionPage enabled
- Hot reload enabled

### Release/Production Mode
```bash
dotnet run --project onvatenter -c Release
```
- Only file logging (console sink disabled)
- Exception details hidden
- Optimized build

## Database Reset

### Option 1: Delete Database
```bash
rm foodsafety.db
dotnet run --project onvatenter
```

### Option 2: Clear with EF Core
```bash
dotnet ef database drop --project onvatenter
dotnet ef database update --project onvatenter
```

## Troubleshooting

### Port Already in Use
If `https://localhost:7000` shows "connection refused":

```bash
# Change port in launchSettings.json
# Or kill the process using the port
```

### Database Locked
```bash
# Close all connections
# Delete foodsafety.db
dotnet run --project onvatenter
```

### Migrations Out of Sync
```bash
dotnet ef migrations add InitialCreate --project onvatenter
dotnet ef database update --project onvatenter
```

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

## Deployment Checklist

- [ ] Build in Release mode
- [ ] Run all tests
- [ ] Update appsettings.json for production database
- [ ] Set environment to Production
- [ ] Configure HTTPS certificate
- [ ] Set up GitHub Secrets for deployment
- [ ] Test CI/CD pipeline

## Project Structure Explanation

```
onvatenter/
├── Models/              - Data models (Premises, Inspection, FollowUp)
├── Data/               - EF Core DbContext and seeding
├── Controllers/        - MVC controllers for requests
├── Services/           - Business logic (DashboardService)
├── Views/              - Razor templates
├── Middleware/         - Custom middleware (exception handling)
├── wwwroot/            - Static files (CSS, JS)
└── Program.cs          - Application startup configuration

onvatenter.Tests/
├── FollowUpTests.cs    - xUnit tests for FollowUp entity
├── DashboardServiceTests.cs  - xUnit tests for Dashboard
└── onvatenter.Tests.csproj

.github/workflows/
└── ci-cd.yml          - GitHub Actions workflow
```

## Key Files to Review

1. **Program.cs** - Serilog configuration and dependency injection
2. **ApplicationDbContext.cs** - Database schema and relationships
3. **DbInitializer.cs** - Seed data and user creation
4. **DashboardController.cs** - Dashboard implementation
5. **GlobalExceptionHandlingMiddleware.cs** - Error handling

## Common Tasks

### View Logs
```bash
# Open latest log file
cat logs/foodsafety-*.txt | tail -100
```

### Add New Premises
1. Login as Admin
2. Navigate to Premises
3. Click "Add New Premises"
4. Fill form and submit

### Create Inspection
1. Login as Inspector or Admin
2. Navigate to Inspections
3. Click "Create New Inspection"
4. Select premises, date, score, outcome
5. Submit

### Create Follow-up
1. Go to Inspection Details
2. Click "Create Follow-up"
3. Set due date and status
4. Submit

### View Dashboard
1. Click "Dashboard" in navbar
2. Use filters to narrow results
3. View aggregated metrics

## Performance Tips

- Use filters on dashboard for large datasets
- Monitor log file size (clean up old logs periodically)
- Database indexes are automatically created for foreign keys
- Consider SQL Server for production high-traffic scenarios

## Support

- Read REQUIREMENTS.md for detailed specifications
- Review README.md for full documentation
- Check GitHub Issues for known problems
- Contact your instructor for course-related questions

---

**Last Updated**: 2024
**Version**: 1.0
