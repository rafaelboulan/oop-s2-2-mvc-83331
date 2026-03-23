# API Documentation

## Base URL
```
https://localhost:7000
```

## Authentication

All endpoints except the home page require authentication. Use ASP.NET Core Identity login.

### Login Flow
1. Navigate to `/Identity/Account/Login`
2. Enter credentials
3. Session established
4. Redirect to dashboard

## Endpoints

### Home & Welcome

#### GET /
```
Description: Welcome page with feature overview
Access: Public
Parameters: None
Response: HTML page
```

#### GET /home
```
Description: Home page (same as /)
Access: Public
Parameters: None
Response: HTML page
```

---

### Dashboard

#### GET /dashboard
```
Description: Main dashboard with metrics and filters
Access: Public (no login required, but optionally shows user)
Parameters:
  - town (string, optional): Filter by town name
  - riskRating (string, optional): Filter by risk rating (Low/Medium/High)
Response: Dashboard view with metrics
Example: GET /dashboard?town=Dorchester&riskRating=High
```

---

### Premises

#### GET /premises
```
Description: List all premises
Access: Public
Parameters: None
Response: Table view with all premises
```

#### GET /premises/{id}
```
Description: View details of a specific premises
Access: Public
Parameters:
  - id (int, required): Premises ID
Response: Premises details with inspection history
Example: GET /premises/1
```

#### GET /premises/create
```
Description: Show create premises form
Access: Admin only
Parameters: None
Response: HTML form
```

#### POST /premises/create
```
Description: Create a new premises
Access: Admin only
Parameters (form data):
  - Name (string, required): Premises name
  - Address (string, required): Street address
  - Town (string, required): Town name
  - RiskRating (enum, required): Low, Medium, or High
Response: Redirect to premises details
Example:
  POST /premises/create
  Content-Type: application/x-www-form-urlencoded
  
  Name=Pizza+Palace&Address=123+Main+St&Town=Dorchester&RiskRating=Medium
```

#### GET /premises/{id}/edit
```
Description: Show edit premises form
Access: Admin only
Parameters:
  - id (int, required): Premises ID
Response: HTML form with current data
Example: GET /premises/1/edit
```

#### POST /premises/{id}/edit
```
Description: Update a premises
Access: Admin only
Parameters (form data):
  - Id (int, required): Premises ID
  - Name (string, required): New premises name
  - Address (string, required): New address
  - Town (string, required): New town
  - RiskRating (enum, required): New risk rating
  - CreatedAt (datetime, required): Original creation date
Response: Redirect to premises details
Example: POST /premises/1/edit
```

---

### Inspections

#### GET /inspections
```
Description: List all inspections (most recent first)
Access: Public
Parameters: None
Response: Table view with all inspections
```

#### GET /inspections/{id}
```
Description: View details of a specific inspection
Access: Public
Parameters:
  - id (int, required): Inspection ID
Response: Inspection details with follow-ups
Example: GET /inspections/1
```

#### GET /inspections/create
```
Description: Show create inspection form
Access: Admin, Inspector
Parameters: None
Response: HTML form with premises dropdown
```

#### POST /inspections/create
```
Description: Create a new inspection
Access: Admin, Inspector
Parameters (form data):
  - PremisesId (int, required): ID of premises being inspected
  - InspectionDate (datetime, required): Date/time of inspection
  - Score (int, required): Score 0-100
  - Outcome (enum, required): Pass or Fail
  - Notes (string, optional): Inspection notes
Response: Redirect to inspection details
Validation: PremisesId must exist
Example:
  POST /inspections/create
  PremisesId=3&InspectionDate=2024-01-15T10:00&Score=85&Outcome=Pass&Notes=Good
```

#### GET /inspections/{id}/edit
```
Description: Show edit inspection form
Access: Admin, Inspector
Parameters:
  - id (int, required): Inspection ID
Response: HTML form with current data
Example: GET /inspections/1/edit
```

#### POST /inspections/{id}/edit
```
Description: Update an inspection
Access: Admin, Inspector
Parameters (form data):
  - Id (int, required): Inspection ID
  - PremisesId (int, required): Premises ID
  - InspectionDate (datetime, required): New date
  - Score (int, required): New score
  - Outcome (enum, required): New outcome
  - Notes (string, optional): Updated notes
  - CreatedAt (datetime, required): Original creation date
Response: Redirect to inspection details
Example: POST /inspections/1/edit
```

---

### Follow-ups

#### GET /followups
```
Description: List all follow-ups (by due date)
Access: Public
Parameters: None
Response: Table view with all follow-ups
Notes: Overdue follow-ups highlighted in red
```

#### GET /followups/{id}
```
Description: View details of a specific follow-up
Access: Public
Parameters:
  - id (int, required): Follow-up ID
Response: Follow-up details with inspection info
Example: GET /followups/1
```

#### GET /followups/create
```
Description: Show create follow-up form
Access: Admin, Inspector
Parameters:
  - inspectionId (int, optional): Pre-select inspection
Response: HTML form with inspections dropdown
Example: GET /followups/create?inspectionId=5
```

#### POST /followups/create
```
Description: Create a new follow-up
Access: Admin, Inspector
Parameters (form data):
  - InspectionId (int, required): ID of related inspection
  - DueDate (date, required): When follow-up is due
  - Status (enum, required): Open or Closed
  - ClosedDate (date, optional): When follow-up was closed
Response: Redirect to follow-up details
Validation:
  - DueDate must be after inspection date
  - If Status is Closed, ClosedDate is required
Warnings:
  - DueDate before inspection date logs warning
Example:
  POST /followups/create
  InspectionId=1&DueDate=2024-02-15&Status=Open
```

#### GET /followups/{id}/edit
```
Description: Show edit follow-up form
Access: Admin, Inspector
Parameters:
  - id (int, required): Follow-up ID
Response: HTML form with current data
Example: GET /followups/1/edit
```

#### POST /followups/{id}/edit
```
Description: Update a follow-up
Access: Admin, Inspector
Parameters (form data):
  - Id (int, required): Follow-up ID
  - InspectionId (int, required): Related inspection
  - DueDate (date, required): New due date
  - Status (enum, required): New status
  - ClosedDate (date, optional): New closed date
  - CreatedAt (datetime, required): Original creation date
Response: Redirect to follow-up details
Validation: If Status is Closed, ClosedDate is required
Example: POST /followups/1/edit
```

---

## Response Codes

### Success Responses
- **200 OK** - Request succeeded, content returned
- **302 Found** - Redirect after POST (to details page)

### Error Responses
- **400 Bad Request** - Invalid form data
- **401 Unauthorized** - Authentication required
- **403 Forbidden** - Insufficient permissions
- **404 Not Found** - Resource doesn't exist
- **500 Internal Server Error** - Server error

---

## Status Codes and Role Access

### Public Endpoints (No Login Required)
- GET /
- GET /home
- GET /dashboard
- GET /premises
- GET /premises/{id}
- GET /inspections
- GET /inspections/{id}
- GET /followups
- GET /followups/{id}

### Admin Only
- GET /premises/create
- POST /premises/create
- GET /premises/{id}/edit
- POST /premises/{id}/edit

### Admin + Inspector
- GET /inspections/create
- POST /inspections/create
- GET /inspections/{id}/edit
- POST /inspections/{id}/edit
- GET /followups/create
- POST /followups/create
- GET /followups/{id}/edit
- POST /followups/{id}/edit

### All Authenticated Users
- GET /dashboard (shows data)

---

## Error Responses

### Validation Error
```
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "errors": {
    "PremisesId": ["The PremisesId field is required."],
    "Score": ["Score must be between 0 and 100"]
  }
}
```

### Not Found
```
HTTP/1.1 404 Not Found
Content-Type: text/html

Error page displayed
```

### Unauthorized
```
HTTP/1.1 401 Unauthorized
Location: /Identity/Account/Login?ReturnUrl=/premises/create
```

### Forbidden
```
HTTP/1.1 403 Forbidden
Content-Type: text/html

Access Denied page displayed
```

---

## Example Workflows

### Create Inspection Workflow

1. Login as Inspector
2. Navigate to GET /inspections/create
3. Select premises from dropdown
4. Enter inspection date, score, outcome
5. POST /inspections/create
6. Redirect to GET /inspections/{newId}

### Create Follow-up Workflow

1. Navigate to GET /inspections/1 (failed inspection)
2. Click "Create Follow-up"
3. GET /followups/create?inspectionId=1
4. Form pre-selects inspection #1
5. Enter due date and status
6. POST /followups/create
7. Redirect to GET /followups/{newId}

### Dashboard Filtering Workflow

1. Navigate to GET /dashboard
2. Select town from dropdown
3. Select risk rating from dropdown
4. Click Filter button
5. GET /dashboard?town=Dorchester&riskRating=High
6. Page displays filtered metrics

---

## Rate Limiting
No rate limiting implemented (suitable for internal use)

## CORS
CORS not enabled (same-origin only)

## Pagination
No pagination implemented for list views

---

**Last Updated**: 2024
**API Version**: 1.0
