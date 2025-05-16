# Marketplace API project
dotnet 8 Minimal API project demonstrating (attempting to..) a lean coding pattern that has emphasis on good readible code semantic names for 
namespace and objects minimal commenting heavy on integration tests leveraging WolverineFx a Mediator library (better easier to work with 
version of .net MediatR).

## Docker
This project uses a Docker MSSQL 2022 Server image as the backing store for Entity Framework related tasks and operations.  The default port will be 
127.0.0.1,1433 - ensure any intances of MSSQL Server and SqlExpress have been shut down first.
The default user is **sa** and the password will be defined in the **docker-compose.yaml** file be sure to change this to your own secure password.

The compose file will spin up a containers:
- MSSQL Server 2022

## Wolverine as a Mediator
This project makes use of WolverineFx as a mediator tool.  This completely disables all node persistence including the inbox and outbox.
