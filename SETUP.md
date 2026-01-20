# School Onboarding/Offboarding Platform

A comprehensive ASP.NET Core Web API for managing student and staff onboarding/offboarding with Snipe-IT asset management integration.

## 🛠 Technology Stack

- **Backend**: ASP.NET Core 10.0
- **Database**: SQL Server (via Docker)
- **ORM**: Entity Framework Core
- **Logging**: Serilog
- **Mapping**: AutoMapper
- **API Docs**: Swagger/OpenAPI

## 📋 Prerequisites

- .NET 10.0 SDK
- Docker & Docker Compose
- Git

## 🚀 Getting Started

### 1. Start the Database

```powershell
# From project root directory
docker-compose up -d

# Verify the container is running
docker ps
```

The SQL Server will be available at:
- **Host**: `localhost:1433`
- **Username**: `sa`
- **Password**: `SchoolDB@2024!`
- **Database**: `SchoolOnboardingDb` (created on first migration)

### 2. Create Database Schema

```powershell
cd SchoolOnboardingAPI

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

### 3. Run the API

```powershell
# From SchoolOnboardingAPI directory
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000/openapi/v1.json`

## 📁 Project Structure

```
SchoolOnboardingAPI/
├── Models/              # Database models
│   ├── Person.cs
│   ├── LifecycleRequest.cs
│   ├── LifecycleTask.cs
│   ├── AssetAssignment.cs
│   ├── TicketLink.cs
│   └── AuditLog.cs
├── Data/                # Entity Framework
│   └── ApplicationDbContext.cs
├── Services/            # Business logic (coming soon)
├── Controllers/         # API endpoints (coming soon)
├── Program.cs           # Application startup
├── appsettings.json     # Configuration
└── SchoolOnboardingAPI.csproj
```

## 🗄️ Database

### Models & Relationships

- **Person** - Central entity for students/staff
- **LifecycleRequest** - Onboarding/offboarding initiation
- **LifecycleTask** - Individual workflow tasks
- **AssetAssignment** - Device tracking (Snipe-IT integration)
- **TicketLink** - Support ticket linking (OSTicket)
- **AuditLog** - Complete change audit trail

### Database Tools

Connect with any SQL client using:
```
Server: localhost,1433
Username: sa
Password: SchoolDB@2024!
Database: SchoolOnboardingDb
```

## 🔌 Docker Commands

```powershell
# Start containers
docker-compose up -d

# Stop containers
docker-compose down

# View logs
docker-compose logs -f sqlserver

# Access SQL Server CLI
docker exec -it school-onboarding-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SchoolDB@2024!

# Reset database (delete all data)
docker-compose down -v
docker-compose up -d
dotnet ef database update
```

## 📚 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SchoolOnboardingDb;User Id=sa;Password=SchoolDB@2024!;..."
  },
  "SnipeIt": {
    "BaseUrl": "https://your-snipeit-instance.com",
    "ApiKey": "your-api-key-here"
  }
}
```

Update credentials for:
- Snipe-IT API integration
- Database connection (if not using Docker defaults)

## 🔐 Security Notes

- Never commit passwords to Git (use .gitignore)
- Use environment variables in production
- Update default SA password in production
- Enable SQL Server encryption for production

## 📖 Entity Framework Migrations

```powershell
# Create new migration after model changes
dotnet ef migrations add DescriptiveNameHere

# Apply migrations
dotnet ef database update

# Revert last migration
dotnet ef migrations remove

# See migration history
dotnet ef migrations list
```

## 🧪 Testing Database Connection

```powershell
# From SchoolOnboardingAPI directory
dotnet user-secrets set "TestDbConnection" "true"
dotnet run
```

Check logs for database connection status.

## 📝 Development Workflow

1. **Make model changes** → Update Models/*.cs
2. **Create migration** → `dotnet ef migrations add NameOfChange`
3. **Apply to DB** → `dotnet ef database update`
4. **Run API** → `dotnet run`
5. **Test endpoints** → Swagger UI or Postman

## 🚢 Deployment (Future)

For school server deployment:
1. Update connection string to point to school SQL Server
2. Configure environment variables
3. Set up CI/CD pipeline
4. Deploy Docker containers or native installation

## 📞 Support

For issues or questions, check:
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core/)
- Docker logs: `docker-compose logs`

## 📄 License

Internal use only - School Onboarding Platform
