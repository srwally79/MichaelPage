# MichaelPage API

REST API in .NET 10, EF Core, and SQL Server. The solution follows the four layers in `Agent.md`; C# identifiers use English names and EF maps them to the existing Spanish SQL identifiers.

## Configuration

The local SQL Server connection and a random 25-character JWT secret are set in `Api/appsettings.json`. The API derives a 256-bit signing key from that secret before using HS256.

The local database already contains the demo account: username `demo`, password `Demo123**`. Its password is stored as a BCrypt hash and its email is `demo@local.test`. The API does not create users automatically at startup. Login accepts `{ "username": "demo", "password": "Demo123**" }`.

## Run

Install the .NET 10 SDK, then from `Back` run:

```bash
dotnet restore MichaelPage.Back.sln
dotnet run --project Api/Api.csproj
```

The SQL schema must already be created from `../ScriptBBDD.sql`. The API does not create or migrate tables on startup. The Angular development origin `http://localhost:4200` is allowed by CORS.

Swagger UI is available at `/swagger`. Sign in there first; the browser then sends the HttpOnly cookie automatically to protected routes.

## Endpoints

| Method | Route | Authorization | Purpose |
| --- | --- | --- | --- |
| POST | `/api/auth/login` | Anonymous | Sets the HttpOnly JWT cookie for username and password |
| GET | `/api/auth/session` | Auth cookie | Checks the current session |
| POST | `/api/auth/logout` | Anonymous | Clears the auth cookie |
| POST | `/api/users` | Auth cookie | Creates a user; body: `name`, `email`, `password` |
| GET | `/api/users` | Auth cookie | Lists users |
| POST | `/api/tasks` | Auth cookie | Creates a task; body: `title`, `userId`, optional JSON object `additionalInfo` |
| GET | `/api/tasks?status=Pending&userId=<guid>` | Auth cookie | Lists tasks, filtered by optional status and user, newest first |
| PUT | `/api/tasks/{id}/status` | Auth cookie | Changes status; body: `status` |
| GET | `/api/statuses` | Auth cookie | Lists the seeded statuses |
| GET | `/api/logs` | Auth cookie | Lists task creation and status change logs |

`additionalInfo` defaults to `{}` and must be a JSON object, for example `{ "comment": "este es additionalinfo", "priority": "High" }`. A scalar string is rejected with `400 Bad Request` before it reaches SQL Server. Errors use `{ "message": "...", "details": [] }`. Pending cannot transition directly to Done. Creation and status changes are recorded in `Log`.

Invalid login credentials return `400 Bad Request`; missing or invalid JWTs return `401 Unauthorized`. Unexpected server errors return a generic `500` response without internal exception details.

The API stores the JWT in a `Secure`, `HttpOnly`, `SameSite=Strict` cookie scoped to `/api`. The frontend never reads or stores the token in JavaScript storage.

