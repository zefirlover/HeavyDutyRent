# HeavyDutyRent

**HeavyDutyRent** is a project built with C# ASP.NET EF Core Repository with Postgre SQL usage.

## Getting Started

These instructions will help you to download, install and run the project on your local machine for development and testing purposes.

### Prerequisites

Before you can run the project, you'll need to have the following installed on your machine:

- .NET Core SDK 3.1 or later
- PostgreSQL 12 or later

### Installing

1. Clone the repository to your local machine using **Command prompt**:

```
git clone https://github.com/zefirlover/HeavyDutyRent.git
```

2. Navigate to the project directory (manually or by using command below):

```
cd HeavyDutyRent
```

3. Create a new database in **PostgreSQL** and update the connection string in the `appsettings.json` file (if needed). The database can be created using a migration ("MigrationName" can be changed):

```
dotnet ef migrations add MigrationName --project "Persistence" --startup-project "WebApi"
```

4. Run the following command to restore the packages:

```
dotnet restore
```

5. Run the following command to build the project:

```
dotnet build
```

6. Run the following command to run the project:

```
dotnet run
```

Also, you can run the project using **Visual Studio** or **JetBrains Rider**

## Built With

- [.NET Core](https://dotnet.microsoft.com/) - The web framework used
- [PostgreSQL](https://www.postgresql.org/) - The database management system