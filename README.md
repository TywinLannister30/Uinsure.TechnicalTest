# Uinsure Technical Test

## Assumptions

It is assumed that the person running the application has docker desktop installed and running.

## Run the Docker dependencies

Before running the acceptance tests, start the database and API with Docker:

```bash
docker compose up --build
```

This brings up SQL Server, the API, and the Aspire dashboard. Keep the containers running while you execute the acceptance tests.

## View logs and traces (Aspire)

Open the Aspire dashboard at http://localhost:18888 to view logs and traces for the running services.

## View the database

You can access the sql database spun up in docker with the following credentials:

username: sa
password: YourStrong!Passw0rd
