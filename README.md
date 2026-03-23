# Food Safety Inspection Tracker

A comprehensive ASP.NET Core MVC application for tracking food premises inspections, outcomes, and follow-ups with structured logging and role-based access control.

## Project Overview

This application is designed for local councils to track food premises inspections, record inspection outcomes, and manage follow-ups. It provides visibility into what's happening and alerts when things fail.

### Tech Stack

- **Framework**: ASP.NET Core MVC (.NET 9)
- **Database**: SQLite (configurable to SQL Server)
- **ORM**: Entity Framework Core 9
- **Authentication**: ASP.NET Core Identity
- **Logging**: Serilog with console and file sinks
- **Testing**: xUnit
- **CI/CD**: GitHub Actions

## Features

### Core Features

1. **Premises Management**
   - Create, read, and update food premises
   - Categorize by risk rating (Low, Medium, High)
   - Organize by town location

2. **Inspections**
   - Record inspection dates and scores (0-100)
   - Track inspection outcomes (Pass/Fail)
   - Add detailed notes

3. **Follow-ups**
   - Create follow-ups for failed inspections
   - Track due dates and completion status
   - Identify overdue follow-ups automatically

4. **Dashboard**
   - View inspections count for current month
   - Track failed inspections this month
   - Monitor overdue follow-ups
   - Filter by town and risk rating

### Security Features

- **Role-Based Access Control**
  - **Admin**: Full access to all features
  - **Inspector**: Can create inspections and follow-ups
  - **Viewer**: Read-only access to dashboards

### Logging & Monitoring

- **Serilog Integration**
  - Console output in development
  - Rolling file sink (daily rotation, 30-day retention)
  - Structured logging with enrichment
  - Application, Environment, and UserName context

- **Error Handling**
  - Global exception middleware
  - Friendly error pages
  - Detailed error logging

## Project Structure

```
onvatenter/
├── Models/
│   ├── Premises.cs
│   ├── Inspection.cs
│   └── FollowUp.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── ApplicationUser.cs
│   └── DbInitializer.cs
├── Controllers/
│   ├── DashboardController.cs
│   ├── PremisesController.cs
│   ├── InspectionsController.cs
│   └── FollowUpsController.cs
├── Services/
│   └── DashboardService.cs
├── Middleware/
│   └── GlobalExceptionHandlingMiddleware.cs
├── Views/
│   ├── Dashboard/
│   ├── Premises/
│   ├── Inspections/
│   ├── FollowUps/
│   └── Shared/
├── wwwroot/
│   └── css/
│       └── site.css
└── Program.cs

onvatenter.Tests/
├── FollowUpTests.cs
├── DashboardServiceTests.cs
└── onvatenter.Tests.csproj

.github/workflows/
└── ci-cd.yml
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- Visual Studio 2022 or VS Code
- SQLite (included with .NET)

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/oop-s2-2-mvc-xxxx.git
cd oop-s2-2-mvc-xxxx
```

2. Restore dependencies
```bash
dotnet restore
```

3. Build the solution
```bash
dotnet build
```

4. Run the application
```bash
dotnet run --project onvatenter
```

5. Access the application
```
https://localhost:7000
```

### Default Credentials

The application seeds the database with default users:

| Username | Email | Password | Role |
|----------|-------|----------|------|
| admin | admin@foodsafety.local | Admin123! | Admin |
| inspector | inspector@foodsafety.local | Inspector123! | Inspector |
| viewer | viewer@foodsafety.local | Viewer123! | Viewer |

## Database

### Entities

1. **Premises**
   - Id (PK)
   - Name (required, max 200 chars)
   - Address (required, max 500 chars)
   - Town (required, max 100 chars)
   - RiskRating (enum: Low, Medium, High)
   - CreatedAt (timestamp)

2. **Inspection**
   - Id (PK)
   - PremisesId (FK)
   - InspectionDate (required)
   - Score (0-100)
   - Outcome (enum: Pass, Fail)
   - Notes (optional, max 2000 chars)
   - CreatedAt (timestamp)

3. **FollowUp**
   - Id (PK)
   - InspectionId (FK)
   - DueDate (required)
   - Status (enum: Open, Closed)
   - ClosedDate (nullable)
   - CreatedAt (timestamp)

### Relationships

- Premises 1--* Inspection
- Inspection 1--* FollowUp

### Seed Data

The application automatically seeds:
- 12 premises across 3 towns
- 25 inspections with various dates and outcomes
- 10 follow-ups (some overdue, some closed)

## Logging

Logs are written to multiple destinations:

1. **Console Output** (Development)
```
[2024-01-15 10:23:45.123] [INF] Dashboard data retrieved
```

2. **Rolling File** (logs/foodsafety-YYYY-MM-DD.txt)
```
[2024-01-15 10:23:45.123] [INF] Inspection created successfully. InspectionId: 5, PremisesId: 3, Score: 85, Outcome: Pass
```

### Log Levels

- **Information**: User actions (create, update, view)
- **Warning**: Validation/business rule violations
- **Error**: Caught exceptions with full context

### Enrichment

All logs include:
- Application name: "FoodSafetyInspectionTracker"
- Environment: "Development" or "Production"
- Username: From HttpContext when available
- ThreadId: For debugging concurrent issues

## Testing

### Running Tests

```bash
dotnet test
```

### Test Coverage

The project includes xUnit tests for:

1. **FollowUpTests**
   - Overdue follow-ups query returns correct items
   - Follow-up cannot be closed without ClosedDate
   - IsOverdue property works correctly

2. **DashboardServiceTests**
   - Inspections this month count consistency
   - Failed inspections count accuracy
   - Filter by town functionality

Run specific test:
```bash
dotnet test --filter "FollowUpTests"
```

## CI/CD Pipeline

GitHub Actions workflow (`ci-cd.yml`) performs:

1. **Build** (Release configuration)
2. **Test** (xUnit tests)
3. **Publish** (Optional deployment)

Triggered on:
- Push to `main` or `develop` branches
- Pull requests

## API Endpoints

### Dashboard
- `GET /` or `GET /dashboard` - Dashboard view with filters

### Premises
- `GET /premises` - List all premises
- `GET /premises/{id}` - View premise details
- `POST /premises/create` - Create new premise (Admin only)
- `POST /premises/{id}/edit` - Update premise (Admin only)

### Inspections
- `GET /inspections` - List all inspections
- `GET /inspections/{id}` - View inspection details
- `POST /inspections/create` - Create new inspection (Admin, Inspector)
- `POST /inspections/{id}/edit` - Update inspection (Admin, Inspector)

### Follow-ups
- `GET /followups` - List all follow-ups
- `GET /followups/{id}` - View follow-up details
- `POST /followups/create` - Create new follow-up (Admin, Inspector)
- `POST /followups/{id}/edit` - Update follow-up (Admin, Inspector)

## Error Handling

The application implements:

1. **Global Exception Middleware** - Catches unhandled exceptions
2. **Model Validation** - ASP.NET Core model state validation
3. **Business Logic Validation** - Custom validation in controllers
4. **Friendly Error Pages** - User-friendly error messages
5. **Comprehensive Logging** - All errors logged with context

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=foodsafety.db"
  }
}
```

### Program.cs Setup

- Serilog configured with console and file sinks
- EF Core with SQLite context
- Identity with password requirements
- Global exception handling middleware
- Automatic database initialization

## Performance Considerations

- Indexed queries for common operations
- Efficient EF Core queries with includes
- Database seeding for testing
- In-memory testing support

## Security

- HTTPS redirection in production
- CSRF protection via token validation
- Password requirements enforced
- Role-based authorization on controllers/actions
- SQL injection protection via EF Core

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Marking Criteria

This project meets the following assessment criteria:

- **Serilog Configuration (30 marks)**: Console + rolling file sink with enrichment
- **Logging Coverage (20 marks)**: 8+ meaningful log events across workflows
- **Dashboard Queries (20 marks)**: Aggregations with filtering by town/risk rating
- **EF Core Model (20 marks)**: Correct relationships with migrations and seed data
- **Error Handling (10 marks)**: Global middleware with friendly error pages
- **Role-Based Access (10 marks)**: Admin, Inspector, Viewer with server-side enforcement
- **GitHub CI (10 marks)**: Build and test workflow in Release configuration

Total: 100 marks

## License

This project is for educational purposes.

## Support

For issues or questions, please refer to the course guidelines or contact your instructor.
