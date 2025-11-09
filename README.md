# IBudget
> Note: IBudget is the development name, the application name is Stacks

A personal finance management application built with .NET 8 and Avalonia UI. Track expenses, manage income, set financial goals, and get insights into your spending habits.

## Prerequisites

Before you start, make sure you have these installed:

- .NET 8 SDK or later
- MongoDB Community Server (for database storage)
- Git (to clone the repository)

Optional but recommended:
- MongoDB Compass (GUI for viewing your data)
- MongoSH (MongoDB shell for command-line operations)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/zlo120/IBudget.git
cd IBudget
```

### Database Setup

The application supports both LiteDB and MongoDB. For most users, MongoDB is recommended.

#### MongoDB Setup

1. Install MongoDB Community Server from https://www.mongodb.com/try/download/community
2. Install MongoDB Compass and MongoSH from https://www.mongodb.com/try/download/compass

Once MongoDB is running, the application will automatically create a database named `IBudget` with the necessary collections on first run. The main collection used is `userDictionaries`.

If you need to find your MongoDB connection string:
```bash
mongosh
use IBudget
db.getMongo()
```

### Building the Application

The simplest way to build the entire solution:

```bash
dotnet build IBudget.sln
```

This compiles all three projects in the solution:
- `IBudget.Core` - Core business logic and models
- `IBudget.Infrastructure` - Database context and repositories
- `IBudget.GUI` - Avalonia-based user interface

For a release build:

```bash
dotnet build IBudget.sln -c Release
```

### Running the Application

#### Development Mode

To run the GUI application in development mode:

```bash
dotnet run --project IBudget.GUI/IBudget.GUI.csproj
```

Or if you're using Visual Studio, just set `IBudget.GUI` as the startup project and hit F5.

#### Using VS Code Tasks

If you're working in VS Code, there are predefined tasks available:

- `build` - Builds the solution
- `publish` - Publishes the solution for distribution
- `watch` - Runs the app with hot reload enabled

You can run these from the Command Palette (Ctrl+Shift+P) by selecting "Tasks: Run Task".

### Project Structure

The solution follows a clean architecture approach:

- **IBudget.Core** - Domain models, interfaces, services, and business logic. No dependencies on external frameworks.
- **IBudget.Infrastructure** - Implementation of data access using MongoDB and LiteDB, repository patterns.
- **IBudget.GUI** - Avalonia UI application with MVVM pattern, views, and view models.

### Configuration Notes

The application uses LiteDB by default for local storage. If you prefer MongoDB or need to customize database settings, you can modify the connection strings in the application's settings after first launch.

Database files are typically stored in:
```
C:\Users\{YourUsername}\AppData\Local\IBudget\IBudgetDB\
```

### Common Issues

**Build fails with missing dependencies**
```bash
dotnet restore IBudget.sln
dotnet build IBudget.sln
```

**MongoDB connection issues**

Make sure MongoDB service is running. On Windows, check Services for "MongoDB Server". On macOS/Linux, run:
```bash
sudo systemctl status mongod
```

**Application won't start**

Check that you have .NET 8 runtime installed:
```bash
dotnet --list-runtimes
```

## Development

The application uses:
- Avalonia 11.2 for cross-platform UI
- CommunityToolkit.Mvvm for MVVM implementation
- MongoDB.EntityFrameworkCore for database operations
- ClosedXML for spreadsheet generation
- CsvHelper for CSV import/export

When adding new features or fixing bugs, run the build task regularly to catch any compilation issues early.

## License

See the LICENSE file for details.
