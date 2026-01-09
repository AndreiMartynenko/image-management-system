# HealthcareIMS

## Overview
HealthcareIMS is a demo Image Management System (IMS) for ABC Healthcare Group, built with ASP.NET Core Razor Pages, Entity Framework Core, and ASP.NET Core Identity (role-based access).

It supports:
- Patient accounts and staff accounts (Reception, Doctor, Radiologist, Accountant, Admin)
- Visit workflow with time-stamped services/tasks
- Radiology image upload with classification (MRI/CT/XRay + disease type)
- Billing (invoice + payments)
- Reports (e.g., patient history)

## Tech Stack
- .NET 8 (ASP.NET Core Razor Pages)
- EF Core
- ASP.NET Core Identity + Roles
- SQL Server (DefaultConnection)

## Quick Start
1. Ensure you have:
   - .NET SDK 8.x installed
   - SQL Server / LocalDB available (see `appsettings.json`)

## Running in Rider (recommended)
1. Open Rider
2. `File` -> `Open...`
3. Select `HealthcareIMS.sln`
4. Choose the `HealthcareIMS` run configuration and click `Run`

If Rider prompts to install a .NET SDK, install **.NET 8 SDK**.

2. Configure database connection:
   - Edit `appsettings.json` `ConnectionStrings:DefaultConnection` as needed.

3. Create database / apply migrations:
   - Use Visual Studio migrations tooling or `dotnet ef`.

4. Run:
   - Run the solution from Visual Studio or `dotnet run`.

## Seeded Roles and Admin
On startup, the app seeds roles and a default admin user.

### Roles
- Admin
- Reception
- Doctor
- Radiologist
- Accountant
- Patient

### Default admin (demo)
- Email: `admin@abc.com`
- Password: `123456aA@`

## Demo Walkthrough (Suggested)
1. Login as Admin
   - Confirm roles exist
   - Create/assign users to roles (Reception/Doctor/Radiologist/Accountant)
   - Create ServiceDefinitions (name/category/cost)

2. Reception
   - Create a patient account
   - Open Manage Visit/Services
   - Search/select patient -> open visit
   - Add services to the visit (assign doctor optional)

3. Radiologist
   - Open assigned Radiology service
   - Upload an image + select Imaging Type and Disease Type
   - Add diagnosis/comments
   - Optionally refer to another department

4. Doctor
   - Open DoctorVisit service
   - Add diagnosis/comments
   - Finish visit or refer

5. Accountant
   - Open invoices
   - Add payment(s) and check payment status changes

## Security Note
CSRF (anti-forgery) protection is enabled. All POST forms should include anti-forgery tokens.

## Submission Cleanup Checklist (recommended)
Before submitting (zip/upload), remove build artifacts and unused folders:

- Delete folders:
  - `bin/`
  - `obj/`
  - `.vs/`
- Delete files:
  - `*.csproj.user`
  - `.DS_Store`

### Remove unused legacy app folder
This repo contains an older duplicate app under `HealthcareIMS/HealthcareIMS` which is not referenced by `HealthcareIMS.sln`.

Safe approach:
1. Rename `HealthcareIMS/HealthcareIMS` -> `HealthcareIMS/_OLD_HealthcareIMS_BACKUP`
2. Run the solution in Rider
3. If everything still runs, delete `_OLD_HealthcareIMS_BACKUP` before submission
