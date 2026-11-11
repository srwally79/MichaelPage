# MichaelPage Front

Angular 19 standalone application with reactive forms and Bootstrap 5.3.

## Run locally

1. Start the API at `http://localhost:5000`.
2. From `Front`, install packages and run the dev server:

```bash
npm install
npm run start
```

The Angular proxy forwards `/api` requests to the API. Open `http://localhost:4200` and sign in with the local demo account `demo` / `Demo123**`. The API stores the JWT in an HttpOnly cookie; the frontend never reads the token.

## Implemented

- JWT login with an HttpOnly secure cookie, session guard and error interceptors.
- Task listing and filters by status and user.
- Reactive task creation with required assignee and JSON object validation.
- Task status updates, with the Pending-to-Done transition disabled in the interface.
- Visible success and error messages.
