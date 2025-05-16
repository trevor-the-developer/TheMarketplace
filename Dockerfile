FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=P@ssw0rd!
ENV MSSQL_PID=Developer

# Expose SQL Server port
EXPOSE 1433
