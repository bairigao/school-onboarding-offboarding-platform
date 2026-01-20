# school-onboarding-offboarding-platform
A comprehensive web platform for managing the onboarding and offboarding of students and staff in educational institutions. Built with ASP.NET Core 10.0 backend and Next.js 16 frontend, integrated with Snipe-IT for device management.

## Features

### 🎯 Core Functionality
- **People Management**: Create and manage student and staff records
- **Lifecycle Workflows**: Automated onboarding and offboarding processes
- **Device Management**: Track device assignments and check-in/check-out
- **Task Management**: Create and complete workflow tasks
- **Search & Filters**: Search people by name, filter by type and status

### 👥 Role-Based Access Control
- **Enrollment Officer (EO)**: Create people, manage their own lifecycle requests
- **HR Staff (HR)**: Create people, manage their own lifecycle requests  
- **IT Staff (IT)**: View all requests, complete device-related tasks, check in/out devices

### 🔗 Integrations
- **Snipe-IT API**: Device management system integration
- **OSTicket**: (Planned) Auto-create tickets for lifecycle events

## Tech Stack

### Backend
- **Framework**: ASP.NET Core 10.0
- **Database**: SQL Server (Docker)
- **ORM**: Entity Framework Core 10.0.1
- **API Documentation**: Swagger/Swashbuckle
- **Logging**: Serilog

### Frontend
- **Framework**: Next.js 16.1.1
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **HTTP Client**: Axios

## Getting Started

### Backend Setup
\`\`\`bash
cd SchoolOnboardingAPI
dotnet ef database update
dotnet run
\`\`\`
API runs on `http://localhost:5007`

### Frontend Setup
\`\`\`bash
cd SchoolOnboardingFrontend
npm install
npm run dev
\`\`\`
Frontend runs on `http://localhost:3000`

## API Endpoints

### People
- `GET /api/people` - List people (search, filters, pagination)
- `GET /api/people/{id}` - Get specific person
- `POST /api/people` - Create person
- `PUT /api/people/{id}` - Update person

### Lifecycle Requests
- `GET /api/lifecycle` - List requests
- `POST /api/lifecycle` - Create request
- `PUT /api/lifecycle/{id}` - Update request

### Lifecycle Tasks
- `GET /api/lifecycle-tasks` - List tasks
- `PUT /api/lifecycle-tasks/{id}` - Mark complete

### Assets
- `GET /api/assets/person/{personId}` - Get person's devices
- `POST /api/assets/{id}/checkin` - Check in device

## Database Models

- **Person**: Student and staff records
- **LifecycleRequest**: Onboarding/offboarding workflows
- **LifecycleTask**: Individual workflow tasks
- **AssetAssignment**: Device assignments
- **TicketLink**: OSTicket integration (planned)
- **AuditLog**: System audit trail

## Future Enhancements

- [ ] JWT Authentication
- [ ] OSTicket integration
- [ ] Email notifications
- [ ] Bulk CSV import
- [ ] Advanced reporting