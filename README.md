# Lifecycle Service

Dedicated microservice for coordinating employee onboarding, offboarding, and transition processes for Maliev Co. Ltd.

## Overview

The Lifecycle Service manages the critical entry and exit phases of the employee journey:

- **Onboarding** - Automated checklist generation, task assignment, and progress tracking for new hires.
- **Offboarding** - Coordinating departure procedures, including equipment return and exit interviews.
- **Task Management** - Assigning and monitoring specific lifecycle tasks across different departments (IT, HR, Facilities).
- **Access Revocation** - Triggering system access removal workflows upon employee termination.

## Architecture

- **Framework**: ASP.NET Core 10.0
- **Database**: PostgreSQL 18 with Entity Framework Core
- **Messaging**: RabbitMQ via MassTransit
- **Templates**: Configurable onboarding templates per department

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- PostgreSQL 18
- Docker (optional, for Redis and RabbitMQ)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/MALIEV-Co-Ltd/Maliev.LifecycleService.git
   ```

2. **Run database migrations**
   ```bash
   dotnet ef database update --project Maliev.LifecycleService.Infrastructure --startup-project Maliev.LifecycleService.Api
   ```

3. **Run the service**
   ```bash
   dotnet run --project Maliev.LifecycleService.Api
   ```

   The service will be available at `https://localhost:7244` or `http://localhost:5087`.

## API Endpoints

### Onboarding

```
POST /lifecycle/v1/employees/{employeeId}/onboarding/start - Initiate onboarding
GET  /lifecycle/v1/employees/{employeeId}/onboarding/status - View progress
GET  /lifecycle/v1/onboarding/pending - List all active onboardings
PUT  /lifecycle/v1/onboarding-items/{itemId}/complete - Mark task as done
```

### Offboarding

```
POST /lifecycle/v1/employees/{employeeId}/offboarding/start - Initiate offboarding
GET  /lifecycle/v1/employees/{employeeId}/offboarding/status - View progress
PUT  /lifecycle/v1/offboarding-tasks/{taskId}/complete - Mark task as done
```

## Integration Events Published

- `OnboardingStartedEvent` - Triggered when a new onboarding begins.
- `OffboardingStartedEvent` - Triggered when a new offboarding begins.

## Integration Events Consumed

- `EmployeeCreatedIntegrationEvent` - Automatically starts onboarding using department templates.
- `EmployeeTerminatedIntegrationEvent` - Automatically starts offboarding procedures.

## License

Copyright © 2025 Maliev Co. Ltd. All rights reserved.