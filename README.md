# 🚀 Task Management System API (TskMngmntSys)

A secure, multi-tenant Task Management System API built using **ASP.NET Core**, **ABP Framework (Classic)**, and **Entity Framework Core**.

The system supports:

- JWT Authentication
- Google Authenticator based Two-Factor Authentication (2FA)
- Role-based access control
- Multi-tenant data isolation
- Enterprise-style reporting using SQL Stored Procedures

---

# ✨ Key Features

---

## 🔐 Authentication & Authorization

- JWT-based authentication
- Google Authenticator (TOTP) based Two-Factor Authentication
- Role-based authorization (Manager, Employee)
- Permission-based access using ABP Authorization Provider
- Claims-based identity handling
- Owner-based authorization for sensitive settings

---

## 🔒 Two-Factor Authentication (2FA)

This project implements Google Authenticator-based 2FA integrated into the login workflow.

### 🔁 Authentication Flow

1. User logs in with Username & Password.
2. If 2FA is enabled:
   - API returns `RequiresTwoFactor = true`.
3. User submits OTP from Google Authenticator.
4. System verifies OTP using ASP.NET Identity.
5. JWT token is issued only after successful verification.

### 🛡 Security Enhancements

- TOTP validation using `TokenOptions.DefaultAuthenticatorProvider`
- Lockout protection against OTP brute-force attempts
- Owner-only 2FA enable/disable endpoint
- Secure JWT token issuance after full verification
- Multi-tenant aware authentication

---

## 🏢 Multi-Tenancy

- Tenant-based data isolation
- Each tenant manages its own users and tasks
- Supports Host and Tenant administrators
- Tenant-aware authentication & authorization

---

## 📋 Task Management

- Create tasks (Manager only)
- Update tasks (Manager only)
- Delete tasks (Manager only)
- Assign tasks to employees
- Employees can update task status only

### Task Statuses

- Pending
- In Progress
- Completed

---

## 📊 Reporting (Stored Procedures)

Production-style reporting using SQL Server stored procedures.

### Task Progress Report

Displays:

- User name
- Task title
- Task status

Optimized for performance and enterprise reporting patterns.

---

# 🧱 Clean Architecture

The project follows Domain-Driven Design principles with proper separation of concerns:

---

TskMngmntSys
│
├── TskMngmntSys.Core
│ ├── Authorization
│ ├── Entities
│
├── TskMngmntSys.Application
│ ├── Tasks
│ │ ├── Dtos
│ │ ├── TaskAppService.cs
│
├── TskMngmntSys.EntityFrameworkCore
│ ├── QueryModels
│ ├── DbContext
│ ├── StoredProcedures
│
├── TskMngmntSys.Web.Host
│ ├── Controllers
│ ├── TokenAuthController (JWT + 2FA)
│ ├── Swagger


---

# 🛠 Tech Stack

| Technology              | Usage                        |
|-------------------------|------------------------------|
| ASP.NET Core            | Web API                      |
| ABP Framework (Classic) | Authorization, Multi-Tenancy |
| Entity Framework Core   | ORM                          |
| ASP.NET Identity        | Authentication & 2FA         |
| SQL Server              | Database                     |
| Stored Procedures       | Reporting                    |
| JWT                     | Secure API Authentication    |
| AutoMapper              | DTO Mapping                  |
| Swagger                 | API Testing                  |

---

# 👥 Roles & Permissions

## 👔 Manager

- Create tasks
- Edit tasks
- Delete tasks
- Assign tasks
- View reports

## 👨‍💼 Employee

- View assigned tasks
- Update task status only

---

## 🔑 Permissions Used

```text
Pages.Tasks
Pages.Tasks.Create
Pages.Tasks.Edit
Pages.Tasks.Delete
Pages.Tasks.View
```

## API Endpoints (Examples)

### Create Task (Manager)

```
POST /api/services/app/Task/Create
```

### Assign Task to User (Manager)

```
POST /api/services/app/Task/Assign
```

### Change Task Status (Employee)

```
PUT /api/services/app/Task/ChangeStatus
```

### Task Progress Report (Stored Procedure)

```
GET /api/services/app/Task/GetTaskProgressReport
```

## 🔐 Authentication

### Login
```
POST /api/TokenAuth/Authenticate
```

### Verify 2FA

```
POST /api/TokenAuth/VerifyTwoFactor
```

### Enable / Disable 2FA (Owner Only)

```
POST /api/TokenAuth/UpdateTwoFactor
```

---

## Stored Procedure Example

```sql
CREATE PROCEDURE GetTaskProgressReport
AS
BEGIN
    SELECT 
        u.UserName,
        t.Title AS TaskName,
        t.Status
    FROM Tasks t
    INNER JOIN AbpUsers u ON t.AssignedUserId = u.Id
END
```

---

## Setup Instructions

### Clone Repository

```bash
git clone https://github.com/your-username/TskMngmntSys.git
```

### Update Connection String

Edit `appsettings.json`:

```json
"Default": "Server=.;Database=TskMngmntSysDb;Trusted_Connection=True;"
```

### Run Migrations

```bash
Add-Migration Initial
Update-Database
```

### Run Application

```bash
dotnet run
```

### Open Swagger

```
https://localhost:{port}/swagger
```

---

## Testing

* All APIs tested via **Swagger & Postman**
* Role-based access validated
* Multi-tenant behavior verified

## Project Characteristics 

✔ Clean architecture
✔ Uses Stored Procedures for reports
✔ Proper role separation
✔ Multi-tenant design
✔ Enterprise authorization model
✔ Production-style reporting
✔ Secure authentication workflow
✔ Real-world backend architecture

## Author

**MB Malik**
BSIT Graduate | ASP.NET Core & ABP Developer
