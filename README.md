# JobTracker API

A REST API to track job applications, interview rounds and reminders.
Built with **C#, ASP.NET Core (.NET 10), Entity Framework Core and SQL Server**.

## Features

- Register and login with **JWT authentication** and BCrypt password hashing
- Job application **CRUD** with search, filter, sort and pagination
- **Status workflow** with enforced rules (e.g. Rejected cannot go back to Applied) and an audit history
- **Interview rounds** per application
- **Dashboard summary**: counts per status, applications per week, upcoming interviews
- **Background service** that sends interview reminders 24 hours ahead
- **Global exception handling** with consistent error responses
- **Serilog** structured logging to console and rolling files
- **25+ unit tests** with xUnit and Moq
- Each user can only access their own data

## Architecture

Layered (clean-style) architecture. Dependencies point inward.

```
JobTracker.Api            Controllers, middleware, Program.cs
JobTracker.Application    Services, DTOs, interfaces, business rules
JobTracker.Domain         Entities and enums (no dependencies)
JobTracker.Infrastructure EF Core, repositories, JWT, email, background jobs
JobTracker.Tests          xUnit + Moq unit tests
```

Request flow: Controller → Service → Repository → EF Core → Database

## API endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/auth/register | Create an account |
| POST | /api/auth/login | Get a JWT |
| GET | /api/auth/me | Current user |
| GET | /api/applications | List (search, status, sortBy, page, pageSize) |
| POST | /api/applications | Create |
| GET/PUT/DELETE | /api/applications/{id} | Read, update, delete |
| PATCH | /api/applications/{id}/status | Change status (rules enforced) |
| GET/POST | /api/applications/{id}/interviews | List or add interview rounds |
| PUT/DELETE | /api/interviews/{id} | Update or delete a round |
| GET | /api/dashboard/summary | Dashboard statistics |

## Run locally

Requirements: .NET 10 SDK and SQL Server (or LocalDB).

```bash
git clone https://github.com/kartikmnd99/JobTracker.git
cd JobTracker

dotnet user-secrets init --project src/JobTracker.Api
dotnet user-secrets set "Jwt:Key" "any-random-string-of-at-least-32-characters" --project src/JobTracker.Api

dotnet ef database update -p src/JobTracker.Infrastructure -s src/JobTracker.Api
dotnet run --project src/JobTracker.Api
```

Open `http://localhost:5212/swagger`, register, login, click **Authorize** and paste the token.

## Run tests

```bash
dotnet test
```

## Tech stack

C#, ASP.NET Core Web API, EF Core, SQL Server, JWT, BCrypt, Serilog, xUnit, Moq, Swagger/OpenAPI

## Author

Kartik Kumar, MCA 2023 | [LinkedIn](your-linkedin-link) | bindasskartik97@gmail.com