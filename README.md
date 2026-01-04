# Maliev Lifecycle Service

[![Build Status](https://img.shields.io/badge/Build-Passing-success)](https://github.com/ORGANIZATION/Maliev.LifecycleService)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![Database](https://img.shields.io/badge/Database-PostgreSQL%2018-blue)](https://www.postgresql.org/)

Orchestration microservice for employee onboarding, offboarding, and transitional workflows.

**Role in MALIEV Architecture**: The central coordinator for employee stage transitions. It automates the complex journey of joining (onboarding) and leaving (offboarding) the organization, triggering tasks across IT, HR, and Facilities to ensure a consistent experience.

---

## 🏗️ Architecture & Tech Stack

- **Framework**: ASP.NET Core 10.0 (C# 13)
- **Database**: PostgreSQL 18 with Entity Framework Core 10.x
- **Distributed Cache**: Redis 7.x (Transition state tracking)
- **Messaging**: RabbitMQ via MassTransit
- **API Documentation**: OpenAPI 3.1 + Scalar UI
- **Observability**: OpenTelemetry (Metrics, Traces, Logging)

---

## ⚖️ Constitution Rules

This service strictly adheres to the platform development mandates:

### Banned Libraries
To maintain high performance and low complexity, the following are **NOT** used:
- ❌ **AutoMapper**: Explicit manual mapping only.
- ❌ **FluentValidation**: Standard Data Annotations (`[Required]`, `[EmailAddress]`) only.
- ❌ **FluentAssertions**: Standard xUnit `Assert` methods only.
- ❌ **In-memory Test DB**: All integration tests use **Testcontainers** with real PostgreSQL 18.

### Mandatory Practices
- ✅ **TreatWarningsAsErrors**: Enabled in all `.csproj` files.
- ✅ **XML Documentation**: Required on all public methods and properties.
- ✅ **No Secrets in Code**: All sensitive configuration injected via environment variables.
- ✅ **No Test Config in Program.cs**: Test configuration in test fixtures only.
- ✅ **IAM Integration**: Self-registers permissions with the IAM Service using GCP-style naming: `{service}.{resource}.{action}`.

---

## ✨ Key Features

- **Automated Onboarding**: Intelligent checklist generation and progress tracking triggered by new hire events.
- **Workflow Offboarding**: Coordinated departure procedures including equipment recovery, exit interviews, and access revocation.
- **Cross-Departmental Tasking**: Automated assignment and monitoring of tasks across IT, Facilities, and Finance.
- **Template Engine**: Dynamic template management for onboarding/offboarding per department or seniority level.
- **Real-time Status Boards**: High-visibility progress tracking for HR and managers to ensure friction-less transitions.

---

## 🚀 Quick Start

### Prerequisites
- .NET 10.0 SDK
- Docker Desktop (for infrastructure)
- PostgreSQL 18 (Alpine)

### Local Development Setup

1. **Clone the repository**
```bash
git clone https://github.com/ORGANIZATION/Maliev.LifecycleService.git
cd Maliev.LifecycleService
```

2. **Spin up Infrastructure**
```bash
docker run --name lifecycle-db -e POSTGRES_PASSWORD=YOUR_PASSWORD -p 5432:5432 -d postgres:18-alpine
docker run --name lifecycle-redis -p 6379:6379 -d redis:7-alpine
```

3. **Configure Environment**
```powershell
# Windows PowerShell
$env:ConnectionStrings__LifecycleDbContext="YOUR_POSTGRES_CONNECTION_STRING"
$env:ConnectionStrings__Cache="YOUR_REDIS_CONNECTION_STRING"
```

4. **Apply Migrations & Run**
```bash
dotnet ef database update --project Maliev.LifecycleService.Infrastructure --startup-project Maliev.LifecycleService.Api
dotnet run --project Maliev.LifecycleService.Api
```

The service will be available at `http://localhost:5000/lifecycle`. Access the interactive documentation at `http://localhost:5000/lifecycle/scalar`.

---

## 📡 API Endpoints

All endpoints are prefixed with `/lifecycle/v1/`.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/employees/{id}/onboarding/start` | Manually initiate onboarding |
| GET | `/employees/{id}/onboarding/status` | Current onboarding progress |
| PUT | `/onboarding-items/{id}/complete` | Mark a specific task as completed |
| GET | `/onboarding/pending` | List all active transitions |

---

## 🏥 Health & Monitoring

Standardized health probes for Kubernetes orchestration:
- **Liveness**: `GET /lifecycle/liveness`
- **Readiness**: `GET /lifecycle/readiness` (Checks DB and Redis connectivity)
- **Metrics**: `GET /lifecycle/metrics` (Prometheus format)

---

## 🧪 Testing

We prioritize reliable tests over mock-heavy unit tests.

```bash
# Run all tests using Testcontainers
dotnet test --verbosity normal
```

- **Integration Tests**: Use real PostgreSQL 18 containers.
- **Contract Tests**: Ensure API stability for consumers.

---

## 📦 Deployment

Infrastructure management is handled via GitOps patterns.

- **Docker Image**: `REGION-docker.pkg.dev/PROJECT_ID/REPOSITORY/maliev-lifecycle-service:{sha}`
- **Environments**: Development, Staging, Production

---

## 📄 License

Proprietary - © 2025 MALIEV Co., Ltd. All rights reserved.