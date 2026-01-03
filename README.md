# TaskManager Backend API

ASP.NET Web API backend for TaskManager with PostgreSQL and JWT authentication.

## Requirements

- Docker & Docker Compose

## Quick Start

```bash
# Start the API and PostgreSQL
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop all services
docker-compose down

# Stop and remove data
docker-compose down -v
```

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and get JWT token |
| GET | `/api/auth/me` | Get current user (requires auth) |

### Goals (requires auth)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/goals` | Get all goals |
| POST | `/api/goals` | Create goal |
| GET | `/api/goals/{id}` | Get goal by ID |
| DELETE | `/api/goals/{id}` | Delete goal |
| POST | `/api/goals/{id}/months` | Add month to goal |
| DELETE | `/api/goals/{id}/months/{monthId}` | Delete month |
| POST | `/api/goals/{id}/months/{monthId}/tasks` | Add task |
| PATCH | `/api/goals/{id}/tasks/{taskId}/toggle` | Toggle task |
| DELETE | `/api/goals/{id}/months/{monthId}/tasks/{taskId}` | Delete task |
| POST | `/api/goals/{id}/subgoals` | Add subgoal |
| PATCH | `/api/goals/{id}/subgoals/{subGoalId}/toggle` | Toggle subgoal |
| DELETE | `/api/goals/{id}/subgoals/{subGoalId}` | Delete subgoal |

### Analytics (requires auth)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/analytics/activity` | Get user analytics |

## Development

The API will be available at `http://localhost:5000`.

Swagger UI: `http://localhost:5000/swagger` (only in Development mode)

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | - |
| `Jwt__Key` | JWT signing key (min 32 chars) | - |
| `Jwt__Issuer` | JWT issuer | TaskManagerAPI |
| `Jwt__Audience` | JWT audience | TaskManagerClient |
| `Jwt__ExpirationInDays` | Token expiration in days | 7 |

