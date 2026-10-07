# ServiceFlow

A full-stack service desk application where companies can manage customer support tickets. Built with **ASP.NET Core 10 Web API** on the backend and **React 19 + TypeScript** on the frontend, with **PostgreSQL** as the database.

The whole system runs with a single `docker compose up` command.

![ServiceFlow Dashboard](docs/dashboard.png)

## What Does It Do?

ServiceFlow is basically a ticket management system for field service / IT support companies. You can:

- Create support tickets with title, description, priority and estimated cost
- Assign tickets to employees (technicians)
- Track ticket status: **Open → In Progress → Resolved → Closed**
- Filter tickets by status on the dashboard
- View all customers and employees
- Add new customers and employees from the dashboard
- Log in with JWT authentication (creating and changing data needs a login)

The idea is that a customer calls with a problem (broken screen, network issue, etc.), and the company creates a ticket, assigns a technician, and tracks it until it's done.

## Architecture

The backend follows a **Clean Architecture** (layered) pattern with 3 projects:

```
ServiceFlow.Api              →  Controllers, DTOs, Middleware (HTTP layer)
    ↓
ServiceFlow.Infrastructure   →  EF Core DbContext, PostgreSQL services
    ↓
ServiceFlow.Core             →  Entities, Interfaces, Enums, Value Objects (no dependencies)
```

- **Core** has zero external dependencies. It only contains the domain model (entities, enums, interfaces).
- **Infrastructure** implements the interfaces from Core using Entity Framework Core + PostgreSQL.
- **API** is the entry point. It registers everything with Dependency Injection and exposes REST endpoints.

This way, the business logic doesn't know anything about the database or HTTP — it only knows its own rules.

## Tech Stack

**Backend:**
- ASP.NET Core 10 (.NET 10)
- Entity Framework Core 10 (Code-First)
- PostgreSQL
- Swagger / OpenAPI
- JWT Bearer authentication

**Frontend:**
- React 19
- TypeScript
- Vite
- Tailwind CSS 4

**Other:**
- Docker & Docker Compose
- xUnit (unit and integration tests)
- GitHub Actions (CI)

## How to Run

### Option 1: Docker Compose (easiest)

Just run this in the project root:

```bash
docker compose up --build
```

This starts 3 containers:
| Service | URL |
|---------|-----|
| PostgreSQL Database | `localhost:5432` |
| ASP.NET Core API | `http://localhost:5021` |
| React Frontend | `http://localhost:5173` |

The database gets created and seeded with demo data automatically on first run.

### Option 2: Run Manually

If you want to run without Docker:

1. Make sure you have **PostgreSQL** running on `localhost:5432` with a database called `serviceflow`
2. Start the backend:
   ```bash
   cd src/ServiceFlow.Api
   dotnet run
   ```
3. Start the frontend:
   ```bash
   cd src/ServiceFlow.Web
   npm install
   npm run dev
   ```

## Demo Login

To create or change data in the app you need to log in:

| Username | Password |
|----------|----------|
| `admin` | `admin123` |

These are demo credentials from `appsettings.json` (`Auth:DemoUsername` / `Auth:DemoPassword`). They are only for local development and should be changed (together with the `Jwt:Key` secret) in a real deployment.

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/login` | No | Log in and get a JWT token |
| GET | `/api/tickets` | No | Get all tickets |
| GET | `/api/tickets/{id}` | No | Get ticket by ID |
| GET | `/api/tickets/customer/{customerId}` | No | Get tickets for a customer |
| GET | `/api/tickets/employee/{employeeId}` | No | Get tickets for an employee |
| POST | `/api/tickets` | Yes | Create a new ticket |
| PUT | `/api/tickets/{id}/assign` | Yes | Assign ticket to employee |
| PUT | `/api/tickets/{id}/resolve` | Yes | Mark ticket as resolved |
| PUT | `/api/tickets/{id}/close` | Yes | Close a ticket |
| DELETE | `/api/tickets/{id}` | Yes | Delete a ticket |
| GET | `/api/customers` | No | Get all customers |
| GET | `/api/customers/{id}` | No | Get customer by ID |
| POST | `/api/customers` | Yes | Create a new customer |
| GET | `/api/employees` | No | Get all employees |
| GET | `/api/employees/{id}` | No | Get employee by ID |
| POST | `/api/employees` | Yes | Create a new employee |

Endpoints marked "Yes" need an `Authorization: Bearer <token>` header.

Swagger UI is available at `http://localhost:5021/swagger` when running in Development mode.

## Project Structure

```
ServiceFlow/
├── src/
│   ├── ServiceFlow.Api/            # Web API (controllers, DTOs, middleware)
│   ├── ServiceFlow.Core/           # Domain layer (entities, interfaces, enums)
│   ├── ServiceFlow.Infrastructure/ # Data layer (EF Core, PostgreSQL, migrations)
│   └── ServiceFlow.Web/            # React frontend
├── tests/
│   ├── ServiceFlow.Core.Tests/     # Unit tests (xUnit)
│   └── ServiceFlow.Api.Tests/      # Integration tests for the API
├── .github/workflows/              # CI (build + tests)
├── docker-compose.yml
└── ServiceFlow.slnx
```

## Some Design Decisions

- **Money as a Value Object**: Estimated cost is stored as a `record struct` with Amount + Currency, not just a plain decimal. This is a DDD (Domain-Driven Design) pattern that makes the model more expressive.

- **State transitions on the entity**: Instead of setting `ticket.Status = Resolved` from the controller, the entity has methods like `ticket.Resolve()` and `ticket.Close()`. This keeps the business rules inside the domain model, not scattered across controllers.

- **Global exception handling**: There is a middleware (`ExceptionHandlingMiddleware`) that catches all unhandled exceptions and returns a clean JSON error response using the RFC 7807 ProblemDetails format. So the API never returns raw stack traces to the client.

- **Auto-migration on startup**: When the API starts, it automatically applies any pending EF Core migrations and seeds demo data. This makes it easy to just start the app and have everything ready.

- **JWT authentication**: Anyone can read data (GET endpoints), but creating, assigning, resolving, closing and deleting need a login. The user logs in at `POST /api/auth/login` and gets a JWT token, which the frontend sends with every write request.

- **Request validation**: The request DTOs use Data Annotations, so wrong input (empty title, bad email, etc.) is rejected with a 400 before it reaches the domain.

## Running Tests

```bash
dotnet test
```

There are two test projects:

- `tests/ServiceFlow.Core.Tests` – xUnit unit tests for the domain logic (ticket state transitions, preventing changes to closed tickets, Money value object arithmetic).
- `tests/ServiceFlow.Api.Tests` – integration tests that call the real API endpoints (auth, customers, employees, tickets) against a PostgreSQL database.

The API tests need PostgreSQL running on `localhost:5432`. The GitHub Actions workflow starts a Postgres container for this automatically.

## CI

A GitHub Actions workflow (`.github/workflows`) runs on every push and pull request to `main`. It builds the solution and runs all tests, and it also builds the React frontend.

## Known Limitations & Roadmap

This project focuses primarily on clean backend architecture, domain modeling, and database integration. Areas planned for future iterations include:

- **Roles**: Right now any logged-in user can do all write actions. The plan is to add roles so technicians only see assigned tickets and administrators have full dispatch control.
