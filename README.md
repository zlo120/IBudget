# IBudgetter

Welcome to the documentation for IBudgetter. This document provides an overview of the API calls available for interacting with the backend of IBudgetter.

## API Calls

## Entity Framework Commands

- **Install EFCore in CLI:** `dotnet tool install --global dotnet-ef`
- **Create migration:** `dotnet-ef migrations add MyMigration --context Context --project Infrastructure --startup-project hayden-test`
- **Update db:** `dotnet-ef database update --context Context --project Infrastructure --startup-project hayden-test`
