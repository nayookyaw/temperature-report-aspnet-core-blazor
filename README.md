      Nay Oo Kyaw
      nayookyaw.nok@gmail.com

# Temperature Report 
# Blazor Web Assembly + ASP.NET (.NET Framework Core 9) + MS SQL Server

# Dependencies Installation
`dotnet restore`

# For publish/build WASM
`dotnet workload install wasm-tools`

# Backend
* Migration EF Core Migration
`dotnet tool install --global dotnet-ef`

* Sync migrations for the first time
 `dotnet ef migrations add SyncModel_20251108`

`dotnet ef migrations add InitialCreate` (run this comment only there is no files in Migration folder)
`dotnet ef database update`

* Run the backend
`dotnet build`
`dotnet run`

# Dependencies
- Testing - Moq + xUnit
- Fluent API for migration order
- Validation - FluentValidation [used], Optional - Data Annotations

# Frontend


## Notes
