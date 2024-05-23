# IBudget 💸

Welcome to the documentation for IBudget. This document provides an overview of the API calls available for interacting with the backend of IBudget.

## 📞 API Calls

## 📖 SQLite db 
The console application will create a `IBudget.db` file wherever the data string specifies. An example data string is `"Data Source=C:\\Users\\Zac.Lo\\AppData\\Local\\IBudget\\IBudgetDB\\IBudget.db"` and therefore if that directory and the `IBudget.db` file doesn't exist, both directory and file will be created at `C:\Users\Zac.Lo\AppData\Local\IBudget\IBudgetDB\IBudget.db`.

## 🧑‍💻 Entity Framework Commands 
- **Install EFCore in CLI:** `dotnet tool install --global dotnet-ef`
- **Create migration:** `dotnet-ef migrations add MyMigration --context Context --project IBudget.Infrastructure --startup-project IBudget.ConsoleUI`
- **Update db:** `dotnet-ef database update --context Context --project IBudget.Infrastructure  --startup-project IBudget.ConsoleUI`

## 📄 MongoDB 
This application uses MongoDB. For your local environment, you must install MongoDB Community Server, Mongo Shell (or MongoSh) and it is recommended you to install MongoDB compass too.

- **MongoDB Community Server:** https://www.mongodb.com/try/download/community
- **MongoSH & MongoDB compass:** https://www.mongodb.com/try/download/compass 

### **Recommended DB name and collection name**
I have created my local db with the name `IBudget` with the collection name `userExpenseDictionaries`.

### **Helpful MongoDB commands**
#### To find your DB connection string:
1. Open mongo shell, but using the command `> mongosh` in terminal
2. Use your db, `> use db-name`. For example, I would run `> use IBudget`
3. `> db.getMongo()`

## ⚙️ AppSettings 
Both `appsettings.json` for the `IBudget.ConsoleUI` project and `IBudget.API` project are untracked. Here are templates to create your own `appsettings.json` in the root directory of both projects. 

### IBudget.ConsoleUI\appsettings.json
```
{
  "Logging": {
    "LogLevel": {
      "Default" : "None"
    }
  },
  "DBtype": "SQLite",
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
