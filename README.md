# Marketplace web app backend solution.
## Technology & features:
- .NET 8 Minimal API
- [WolverineFx](https://github.com/JasperFx/wolverine)
- Entity Framework Core 8
- AutoMapper
- Configuration Pattern
- Serilog
- [akeyless.io](https://akeyless.io)
- Alba Integration Testing
- xUnit Unit Testing
- Docker

### Description:
The Marketplace API provides CRUD operations for community based buy/sell/trade web application powered by Vue 3 for Triad employees only.

#### Milestones
**MVP 1** Basic listing, browsing, merchant operations (buy/sell/trade/swap), user login and basic profile management, 
listing operations (create listing, add cards, create products etc and same for edit and delete where applicable).

**MVP 2** Companion mobile application written in [flutter/dart](https://flutter.dev/) language cross platform mobile app development platform from Google.
- Smart search
- Newsletter subscription

**MVP 3** AI integration and analytics
- Reporting
- Dashboard(s)
- Metric(s)

**MVP 4** Improvements & more community features

### Source code:
Navigate to each individual project README.md for more details and where required README.md files have been provided in folders/sub-folders to describe intent for a particular module or component etc.

### Docker
There is a **docker-compose.yaml** file in this repo feel free to change the default password to your preference and use the command **docker compose up** (**-d** for silent mode) to start the container (make sure **Docker** for Windows is running).
The **MSSQL 2022 server** instance is used by the Marketplace.Api data project where migrations are stored and execute against this database instance (ensure any running instances of SQL server have been shutdown first).
To navigate to the server and use Sql Server Management Studio (SSMS) use the following server address: **127.0.0.1,1433** and set the authentication mode to SQL Server Authentication entering your password for the user **sa**.
Ensure you are able to navigate to the **Databases** root folder with the sa account.

You are now ready to use and develop against this database server it is recommened to create and manage your database through Visual Studio 2022's Package-Manager-Console (PMC) terminal see [EF Core Migrations guide](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli).

### Entity Framework Core Migrations
Run the following command in a **Package Manager Console** (PMC): ```Add-Migrations InitialCreate```
This should create some files in the **Migrations** folder in the Data project.
Run the following command in PMC again to create and **Configure** the database: ```Update-Database -Verbose```
This seeds two **IdentityUser** based users into the Identity AspNet tables.
- - - further steps to follow - - - 
