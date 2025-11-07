   Nay Oo Kyaw
   nayookyaw.nok@gmail.com

# Temperature Report 
# Blazor Web Assembly + ASP.NET (.NET Framework Core 9) + MS SQL Starter

# Backend

* Migration EF Core Migration
`dotnet ef migrations add InitialCreate`
`dotnet ef database update`

* Run the backend
`dotnet build`
`dotnet run`

* Testing 
`dotnet new xunit -n BackendAspNetCore.Tests -o backend-aspnetcore/Tests`

# Dependencies
- Testing - Moq + xUnit
- Fluent API for migration order
- Validation - FluentValidation [used], Optional - Data Annotations

# Frontend


## Notes

- LINQ sorting/paging is implemented in `UsersController`
