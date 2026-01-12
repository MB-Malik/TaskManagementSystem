# Task Management System API (TskMngmntSys)

A **multi-tenant Task Management System API** built with **ASP.NET Core**, **ABP Framework**, and **Entity Framework Core**, supporting **role-based access control**, **task assignment**, **status tracking**, and **reporting via Stored Procedures**.

## Features

### Authentication & Authorization

* JWT-based authentication
* Role-based authorization (Manager, Employee)
* Permission-based access using ABP Authorization Provider

### Multi-Tenancy

* Tenant-based data isolation
* Each tenant manages its own users and tasks
* Supports Host & Tenant admins

### Task Management

* Create, update, delete tasks (Manager only)
* Assign tasks to users
* Employees can update task status only
* Task statuses:

  * Pending
  * In Progress
  * Completed

### Reports (Stored Procedures)

* Task progress report per user
* Shows:

  * User name
  * Task name
  * Task status
* Optimized using SQL Stored Procedures (production-style reporting)

### Clean Architecture

* Domain-driven design
* Separation of concerns:

  * Core
  * Application
  * EntityFrameworkCore
  * Web API

---

## Tech Stack

| Technology            | Usage                        |
| --------------------- | ---------------------------- |
| ASP.NET Core          | Web API                      |
| ABP Framework         | Authorization, Multi-Tenancy |
| Entity Framework Core | ORM                          |
| SQL Server            | Database                     |
| Stored Procedures     | Reporting                    |
| Swagger               | API Testing                  |
| AutoMapper            | DTO Mapping                  |

---

## Project Structure

```
TskMngmntSys
│
├── TskMngmntSys.Core
│   ├── Authorization
│   ├── Entities
│
├── TskMngmntSys.Application
│   ├── Tasks
│   │   ├── Dtos
│   │   ├── TaskAppService.cs
│
├── TskMngmntSys.EntityFrameworkCore
│   ├── QueryModels
│   ├── DbContext
│   ├── StoredProcedures
│
├── TskMngmntSys.Web.Host
│   ├── Controllers
│   ├── Swagger
```

## Roles & Permissions

### Manager

* Create tasks
* Edit tasks
* Delete tasks
* Assign tasks to employees
* View reports

### Employee

* View assigned tasks
* Update task status only

## Permissions Used

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

* All APIs tested via **Swagger**
* Role-based access validated
* Multi-tenant behavior verified

## Project Characteristics 

✔ Clean architecture
✔ Uses Stored Procedures for reports
✔ Proper role separation
✔ Multi-tenant design
✔ Enterprise authorization model

## Author

**MB Malik**
BSIT Graduate | ASP.NET Core & ABP Developer
