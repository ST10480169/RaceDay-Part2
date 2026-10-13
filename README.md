# RaceDay — Part 2 REST API (starter project)

This is the first implementation package for PROG6212 Part 2. It uses the Part 1 endpoint plan and the six Part 1 entities: Users, Venues, Events, Categories, Enrolments, and Results.

## Current implementation in this package

- ASP.NET Core Web API targeting .NET 8
- Entity Framework Core SQL Server Code-First model and DbContext
- Password hashing with ASP.NET Core `PasswordHasher<User>`
- Register/login endpoints returning a JWT as specified by the Part 1 endpoint plan
- Role-protected profile, event, category, enrolment, and result endpoints
- Organiser ownership checks on event/category/result management
- Swagger/OpenAPI with Bearer token support
- Initial unit tests and GitHub Actions build/test workflow
- Copies of the Part 1 ERD, endpoint plan, and SQL schema in `/docs`

**Important:** This is a starter milestone, not proof that the assignment is finished. Run it, resolve any environment-specific setup, then expand the test suite to cover successful and rejected API requests before submission. A green build and working local API have not been verified in this environment because the .NET SDK is not installed here.

## Requirements

- Visual Studio 2022 with the **ASP.NET and web development** workload
- .NET 8 SDK (install it if the Visual Studio installer does not show it)
- SQL Server / LocalDB and SQL Server Management Studio
- Git and a GitHub account

## Open the solution

1. Extract the project folder.
2. Open `RaceDay.sln` in Visual Studio 2022.
3. If Visual Studio asks to install or target a .NET SDK, install the .NET 8 SDK and reopen the solution.
4. Allow NuGet package restore to finish.

## Database setup (Code-First)

The connection string in `RaceDay.API/appsettings.json` initially points to LocalDB and a **new development database** called `RaceDayPart2DB`:

```text
Server=(localdb)\MSSQLLocalDB;Database=RaceDayPart2DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

If your SSMS connection uses a different server instance (for example `localhost\SQLEXPRESS`), edit `ConnectionStrings:RaceDay` to match the server name shown in SSMS. Keep the new database name for Part 2 unless your lecturer has instructed you to use another database.

In Visual Studio, open **Tools → NuGet Package Manager → Package Manager Console** and run:

```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.11
Add-Migration InitialCreate -Project RaceDay.API -StartupProject RaceDay.API
Update-Database -Project RaceDay.API -StartupProject RaceDay.API
```

If the EF tools package is already restored, you can skip the `Install-Package` command. These commands create the database schema from the C# models. The model seeds one demo venue so you can create an event.

**Do not run `docs/RaceDay_Schema.sql` against a database with data you need to keep.** The Part 1 script drops and recreates the tables. It is included as the approved schema reference; the Part 2 Code-First migration should use the separate `RaceDayPart2DB` development database.

## Run the API

1. In Visual Studio, set `RaceDay.API` as the startup project.
2. Press **F5** or **Ctrl+F5**.
3. Swagger should open at the local `/swagger` address. If it does not, open the Swagger URL shown in the browser address or project output.
4. `GET /health` should return `{ "status": "ok", "application": "RaceDay API" }`.

The development JWT key in `appsettings.json` is only a placeholder for local coursework. Replace it with a long, private secret before using the API outside your own machine. Never commit real secrets to GitHub.

## Quick test in Swagger

1. Call `POST /api/auth/register` with a unique email, password of at least 8 characters, full name, role (`Organiser` or `Participant`), and optional phone number.
2. Call `POST /api/auth/login` with the same email and password.
3. Copy the `token` value from the response.
4. Click **Authorize** in Swagger and enter `Bearer ` followed by the token (if Swagger's dialog expects the raw bearer token, paste just the token; its HTTP bearer scheme adds the prefix).
5. Call `GET /api/users/me` to confirm the authenticated profile.
6. Register an Organiser, log in as that account, then use `POST /api/events` with `title`, `description`, `eventDate`, `eventType`, and `venueId: 1`.
7. Register a Participant, log in as that account, and test public browsing and enrolment endpoints.

## API endpoints included

| Method | Route | Access |
|---|---|---|
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| GET, PUT | `/api/users/me` | Authenticated user |
| GET | `/api/users/{id}` | Organiser |
| GET | `/api/events` | Public |
| GET | `/api/events/{id}` | Public |
| POST, PUT, DELETE | `/api/events[/{id}]` | Organiser; owner for update/delete |
| GET | `/api/events/{eventId}/categories` | Public |
| POST | `/api/events/{eventId}/categories` | Organiser; owner only |
| PUT, DELETE | `/api/categories/{id}` | Organiser; owner only |
| POST | `/api/categories/{categoryId}/enrolments` | Participant |
| GET | `/api/enrolments/my` | Participant |
| GET | `/api/events/{eventId}/enrolments` | Organiser; owner only |
| DELETE | `/api/enrolments/{id}` | Participant; own enrolment only |
| POST | `/api/enrolments/{enrolmentId}/results` | Organiser; owner only |
| PUT | `/api/results/{id}` | Organiser; owner only |
| GET | `/api/results/my` | Participant |
| GET | `/api/events/{eventId}/results` | Public |

## Authentication decision to confirm

The approved Part 1 endpoint plan explicitly specifies JWT login tokens. The Part 2 rubric also says to use session management to maintain authenticated state and role. This starter follows the Part 1 plan and implements JWT bearer authentication. Ask the lecturer whether JWT is accepted for the session-management requirement, or whether a server-side session mechanism is required. Do not silently claim JWT and server-side sessions are the same thing.

## Unit tests and CI

Run tests in Visual Studio Test Explorer or from a terminal in the solution folder:

```powershell
dotnet test RaceDay.sln
```

The starter tests currently verify password hashing and role validation. Expand the tests to cover registration/login API responses, organiser event create/update/delete, unauthenticated requests, cross-role access denial, participant enrolment, duplicate/full-category enrolment, and ownership restrictions. All tests must pass locally and in GitHub Actions.

The workflow in `.github/workflows/ci.yml` restores, builds, and tests on pushes and pull requests. Push this project to your own Part 2 branch/repository and confirm the Actions run turns green. Add a real screenshot of the successful run here before submission; do not use a placeholder screenshot.

## GitHub and academic integrity checklist

- Commit your own work regularly with meaningful messages; the rubric requires at least 20 meaningful commits for Part 2. Do not create fake or backdated commits to meet the count.
- Read and understand the code, make your own decisions, and disclose AI assistance as your course rules require.
- Add your GitHub repository link, screenshot of a successful Actions build, and your own unlisted YouTube demonstration link before submitting on ARC.
