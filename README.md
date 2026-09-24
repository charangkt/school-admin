# School Admin

An offline **School Administration Management System**. It runs on the school's local network with no internet connection.

Built with C# .NET 10, SQL Server and Entity Framework Core, in two front ends that share one data layer:

- **SchoolAdmin.Web**: Blazor web app (MudBlazor). Also deployed as an online demo on Render.
- **SchoolAdmin.App**: WPF desktop app for Windows.

## Online demo

The web version is deployed to Render from this repo (`Dockerfile` + `render.yaml` at the root). The demo uses a throw-away SQLite database that is filled with sample data on every start, and the login page lists the demo accounts.

On Render's free plan the site sleeps after 15 minutes without visitors; the first visit afterwards takes about a minute, and any changes made in the demo are reset.

Run the web version locally:

```bash
cd SchoolAdmin
dotnet run --project src/SchoolAdmin.Web
```

It uses SQLite with demo data by default (`appsettings.json`). For a school installation set `Database:Provider` to `SqlServer`, point `ConnectionStrings:SchoolDb` at their SQL Server Express, and set `DemoMode` to `false`.

## Features

| Module | Status |
|---|---|
| Secure login with role-based access (Admin, Accountant, Exam Staff, Teacher, Student) | ✅ |
| Students: add, edit, search, filter by class | ✅ |
| Staff records | ✅ |
| Classes and subjects | ✅ |
| User management and change password | ✅ |
| Fees: fee heads, concessions, receipts, outstanding reports | 🔜 |
| Exams: scheduling, grading, results dashboard, admit cards | 🔜 |
| PDF / Excel export | 🔜 |
| Windows installer | 🔜 |

## Tech stack

| Layer | Technology |
|---|---|
| Desktop UI | WPF (.NET 10), MaterialDesignInXaml, MVVM (CommunityToolkit.Mvvm) |
| Database | SQL Server (LocalDB for development, SQL Server Express at the school) |
| Data access | Entity Framework Core 10 (migrations create and upgrade the database automatically) |
| Security | BCrypt password hashing, role-based menus |

## Project structure

```
SchoolAdmin/
├── SchoolAdmin.sln
├── scripts/seed-demo.sql      Demo data (60 students, 12 staff, demo logins)
└── src/
    ├── SchoolAdmin.Data/      Entities, DbContext, migrations, login, demo data
    ├── SchoolAdmin.Web/       Blazor web app: Components/Pages (screens)
    └── SchoolAdmin.App/       WPF app: Views (screens) and ViewModels (logic)
```

## Run on a development PC

Requirements: .NET 10 SDK and SQL Server LocalDB (installed with SQL Server Express or Visual Studio).

```bash
cd SchoolAdmin
dotnet run --project src/SchoolAdmin.App
```

On first start the app creates the `SchoolAdmin` database and a default admin login:

- Username: `admin`
- Password: `Admin@123` (change it from the key icon in the top bar)

The database connection is set in `src/SchoolAdmin.App/appsettings.json`.

## Demo data (optional)

Start the app once so the tables exist, then run:

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d SchoolAdmin -i scripts\seed-demo.sql
```

This adds 14 classes, 6 subjects, 12 staff, 60 students and these demo logins:

| Username | Password | Role |
|---|---|---|
| accountant | Accountant@123 | Accountant |
| examstaff | Exam@123 | Exam Staff |
| teacher | Teacher@123 | Teacher |
| student | Student@123 | Student |

> ⚠️ Never run the demo script on a real school database. The demo passwords are public.

## Database changes (for developers)

After changing an entity, add a migration, **then** build:

```bash
cd SchoolAdmin
dotnet tool restore
dotnet ef migrations add <Name> --project src/SchoolAdmin.Data --startup-project src/SchoolAdmin.Data --output-dir Migrations
dotnet build
```

The app applies new migrations automatically on startup.
