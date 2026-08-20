Quick start

1) Store database connection string securely (user-secrets recommended)

From the web project folder:

```powershell
cd "Disaster Relief Management System/Disaster Relief Management System"
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:GiftOfTheGivers" "Server=tcp:gift-of-the-givers-sql.database.windows.net,1433;Initial Catalog=GiftOfTheGiversDB;User ID=gift-of-the-givers-admin;Password=YOUR_PASSWORD;Encrypt=True;"
```

2) Run the application

```powershell
dotnet run
```

Open the browser to https://localhost:5001/ (or the URL printed by dotnet run). The app serves the Swagger UI at the root so visiting / will show the API explorer.

Notes

- EF Core models and GiftDbContext are in the Service class library (Service/).
- Do not commit secrets.json or any files containing credentials. Use user-secrets or environment variables in CI/production.
