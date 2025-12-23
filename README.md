# Image Management System (IMS) - Java Prototype

This is a **Java (Spring Boot)** prototype for the CI7250 coursework brief (Image Management System).

Check the implementation against the coursework requirements.

## Tech

- Java 21
- Spring Boot (REST API)
- In-memory storage (no database required for demo)
- Basic Auth + role-based access

## Build & Run

```bash
mvn -q test
mvn spring-boot:run
```

App runs on `http://localhost:8080`.

Health check:

```bash
curl -s http://localhost:8080/actuator/health
```

## Authentication

Basic auth users:

- `radiologist` / `password` (role: STAFF)
- `doctor` / `password` (role: STAFF)
- `admin` / `password` (roles: ADMIN, STAFF)

## Demo API (examples)

### Create a patient (STAFF)

```bash
curl -u radiologist:password -H 'Content-Type: application/json' \
  -d '{"name":"John Doe","address":"Kingston","conditions":"lung","diagnosis":"pending"}' \
  http://localhost:8080/api/staff/patients
```

### List patients (STAFF)

```bash
curl -u radiologist:password http://localhost:8080/api/staff/patients
```

### Add a workflow task (STAFF)

```bash
curl -u doctor:password -H 'Content-Type: application/json' \
  -d '{"type":"UPLOAD_IMAGE","cost":20.00,"performedBy":"doctor"}' \
  http://localhost:8080/api/staff/workflow/patients/1001/tasks
```

### List workflow tasks for a patient (STAFF)

```bash
curl -u doctor:password http://localhost:8080/api/staff/workflow/patients/1001/tasks
```

### Upload image metadata (STAFF)

```bash
curl -u radiologist:password -H 'Content-Type: application/json' \
  -d '{"modality":"MRI","diseaseTag":"brain_cancer"}' \
  http://localhost:8080/api/staff/images/patients/1001
```

### List images for a patient (STAFF)

```bash
curl -u radiologist:password http://localhost:8080/api/staff/images/patients/1001
```

### Create staff record (ADMIN)

```bash
curl -u admin:password -H 'Content-Type: application/json' \
  -d '{"name":"Dr Smith","role":"Radiologist"}' \
  http://localhost:8080/api/admin/staff
```

## Notes for the coursework

- The code is structured in components:
  - `domain` (entities)
  - `repo` (in-memory repositories)
  - `service` (business logic)
  - `web` (REST controllers)
  - `config` (security)

This provides a clear **service-oriented/modular** design and can be extended with a database later (e.g. PostgreSQL) or split into microservices.
