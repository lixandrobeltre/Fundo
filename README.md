# Fundo Solution

This repository contains the **Fundo** project, a full-stack application with a .NET backend and an Angular frontend. The solution is organized into two main parts:

- **backend/**: ASP.NET Core Web API, domain logic, infrastructure, and tests
- **frontend/**: Angular application

---
### Project Structure

```
├── backend/
│   └── src/
│       ├── Fundo.Application/
│       ├── Fundo.Applications.WebApi/
│       ├── Fundo.Domain/
│       ├── Fundo.Infraestructure/
│       ├── Fundo.Services.Tests/
│       └── Fundo.sln
├── frontend/
│   ├── src/
│   ├── angular.json
│   └── package.json
├── scripts/
│   ├── db.sql
│   └── sample_data.sql 
```

---
### Setup Database

The `db.sql` script creates the FundoLoanDb database, sets up the required tables, and creates a SQL user with the necessary permissions for the application to connect and operate.. You can do this using SQL Server Management Studio or the `sqlcmd` utility.

---

### Setup Connection String (using dotnet user-secrets)

To securely set your database connection string for local development, use [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```sh
cd backend/src/Fundo.Applications.WebApi
dotnet user-secrets init
dotnet user-secrets set "FundoLoan:ConnectionString" "Server=;Database=FundoLoanDb;User Id=<User Id>;Password=<Password>;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

Replace `<User Id>` and `<Password>` with your actual credentials.

---

## Backend

Located in [`backend/src`](backend/src/README.md), the backend is a .NET solution with the following projects:

- **Fundo.Applications.WebApi**: Main ASP.NET Core Web API project
- **Fundo.Application**: Application layer (business logic, services)
- **Fundo.Domain**: Domain models and interfaces
- **Fundo.Infraestructure**: Data access, repositories, and infrastructure services
- **Fundo.Services.Tests**: Unit and integration tests

### Build & Run

To build and run the backend API:

```sh
cd backend/src
dotnet build
cd Fundo.Applications.WebApi
dotnet run
```

The API will be available at `https://localhost:61355` by default.
