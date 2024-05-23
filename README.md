# IBudget

Welcome to the documentation for IBudget. This document provides an overview of the API calls available for interacting with the backend of IBudget.

## API Calls

## SQLite db
The console application will create a `IBudget.db` file wherever the data string specifies. An example data string is `"Data Source=C:\\Users\\Zac.Lo\\AppData\\Local\\IBudget\\IBudgetDB\\IBudget.db"` and therefore if that directory and the `IBudget.db` file doesn't exist, both directory and file will be created at `C:\Users\Zac.Lo\AppData\Local\IBudget\IBudgetDB\IBudget.db`.

## Entity Framework Commands
- **Install EFCore in CLI:** `dotnet tool install --global dotnet-ef`
- **Create migration:** `dotnet-ef migrations add MyMigration --context Context --project IBudget.Infrastructure --startup-project IBudget.ConsoleUI`
- **Update db:** `dotnet-ef database update --context Context --project IBudget.Infrastructure  --startup-project IBudget.ConsoleUI`

## AppSettings
Both appsettings.json for the IBudget.ConsoleUI project and IBudget.API project are untracked. Here are templates to create your own appsettings.json in the root directory of both projects. 
### IBudget.ConsoleUI\appsettings.json
```
{
  "Logging": {
    "LogLevel": {
      "Default" : "None"
    }
  },
  "DBtype": "sqlite",
  "DBpath": "x",
  "ConnectionStrings": {
    "SQLite": "",
    "MongoDB": ""
  }
}
```
### IBudget.API\appsettings.json
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```