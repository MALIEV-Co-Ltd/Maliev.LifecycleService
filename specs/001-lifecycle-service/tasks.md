# Tasks: Employee Lifecycle Management Service

**Input**: Design documents from `/specs/001-lifecycle-service/`
**Prerequisites**: plan.md (required), spec.md (required), data-model.md (available)

**Tests**: Implementation tasks only - tests not explicitly requested in specification

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

Project uses flat structure at repository root per constitution:
- **Domain**: `Maliev.LifecycleService.Domain/`
- **Application**: `Maliev.LifecycleService.Application/`
- **Infrastructure**: `Maliev.LifecycleService.Infrastructure/`
- **API**: `Maliev.LifecycleService.Api/`
- **Tests**: `Maliev.LifecycleService.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Create .gitignore with build artifacts, IDE files, specs/, .specify/ exclusions
- [x] T002 Create .dockerignore excluding build outputs, Test projects, specs/, .specify/, CI/CD files
- [x] T003 Create LICENSE file at repository root
- [x] T004 Create README.md with project description and setup instructions
- [x] T005 Create .github/CODEOWNERS with content: `* @MALIEV-Co-Ltd/core-developers`
- [x] T006 [P] Create nuget.config with GitHub Packages source and credential placeholders
- [x] T007 [P] Create global.json specifying .NET 10.0 SDK version
- [x] T008 Create solution file Maliev.LifecycleService.sln at repository root
- [x] T009 Create Maliev.LifecycleService.Domain project with TreatWarningsAsErrors enabled
- [x] T010 [P] Create Maliev.LifecycleService.Application project referencing Domain
- [x] T011 [P] Create Maliev.LifecycleService.Infrastructure project referencing Domain and Application
- [x] T012 [P] Create Maliev.LifecycleService.Api project referencing Application and Infrastructure
- [x] T013 [P] Create Maliev.LifecycleService.Tests project with xUnit and Testcontainers packages
- [x] T014 Add Maliev.Aspire.ServiceDefaults NuGet package to Api project
- [x] T015 [P] Add Npgsql.EntityFrameworkCore.PostgreSQL to Infrastructure project
- [x] T016 [P] Add MassTransit.RabbitMQ to Infrastructure project
- [x] T017 [P] Add Microsoft.Extensions.Caching.StackExchangeRedis to Infrastructure project
- [x] T018 [P] Create appsettings.json in Api project with LogLevel configuration per constitution
- [x] T019 [P] Create appsettings.Development.json in Api project with development overrides

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Domain Layer Foundation

- [x] T020 [P] Create OnboardingStatus enum in Maliev.LifecycleService.Domain/Enums/OnboardingStatus.cs
- [x] T021 [P] Create OffboardingStatus enum in Maliev.LifecycleService.Domain/Enums/OffboardingStatus.cs
- [x] T022 [P] Create ItemCategory enum in Maliev.LifecycleService.Domain/Enums/ItemCategory.cs
- [x] T023 [P] Create TaskCategory enum in Maliev.LifecycleService.Domain/Enums/TaskCategory.cs
- [x] T024 [P] Create LifecyclePermissions class in Maliev.LifecycleService.Domain/Authorization/LifecyclePermissions.cs
- [x] T025 [P] Create ChecklistAlreadyStartedException in Maliev.LifecycleService.Domain/Exceptions/ChecklistAlreadyStartedException.cs
- [x] T026 [P] Create ItemAlreadyCompletedException in Maliev.LifecycleService.Domain/Exceptions/ItemAlreadyCompletedException.cs
- [x] T027 [P] Create ChecklistNotFoundException in Maliev.LifecycleService.Domain/Exceptions/ChecklistNotFoundException.cs
- [x] T028 Create AuditLog entity in Maliev.LifecycleService.Domain/Entities/AuditLog.cs

### Infrastructure Foundation

- [x] T029 Create LifecycleDbContext in Maliev.LifecycleService.Infrastructure/Data/LifecycleDbContext.cs
- [x] T030 Create AuditLogConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/AuditLogConfiguration.cs
- [x] T031 Configure DbContext with all entity configurations
- [x] T032 Create initial migration for database schema
- [x] T033 [P] Create IAuditLogService interface in Maliev.LifecycleService.Application/Interfaces/IAuditLogService.cs
- [x] T034 [P] Create AuditLogService in Maliev.LifecycleService.Application/Services/AuditLogService.cs
- [x] T035 [P] Create AuditLogRepository in Maliev.LifecycleService.Infrastructure/Repositories/AuditLogRepository.cs

### Event Messaging Foundation

- [x] T036 [P] Create EmployeeCreatedEvent record in Maliev.LifecycleService.Domain/Events/EmployeeCreatedEvent.cs (per MessagingContracts) with properties: EmployeeId, EmployeeNumber, StartDate, DepartmentId, PositionId?, ManagerId?
- [x] T037 [P] Create EmployeeTerminatedEvent record in Maliev.LifecycleService.Domain/Events/EmployeeTerminatedEvent.cs (per MessagingContracts) with properties: EmployeeId, TerminationDate, TerminationReason?, EligibleForRehire
- [x] T038 [P] Create CandidateAcceptedEvent record in Maliev.LifecycleService.Domain/Events/CandidateAcceptedEvent.cs
- [x] T039 [P] Create OnboardingStartedEvent record in Maliev.LifecycleService.Domain/Events/OnboardingStartedEvent.cs
- [x] T040 [P] Create OnboardingCompletedEvent record in Maliev.LifecycleService.Domain/Events/OnboardingCompletedEvent.cs
- [x] T041 [P] Create OffboardingStartedEvent record in Maliev.LifecycleService.Domain/Events/OffboardingStartedEvent.cs
- [x] T042 [P] Create OffboardingCompletedEvent record in Maliev.LifecycleService.Domain/Events/OffboardingCompletedEvent.cs
- [x] T043 [P] Create AccessRevocationRequiredEvent record in Maliev.LifecycleService.Domain/Events/AccessRevocationRequiredEvent.cs
- [x] T044 [P] Create OnboardingItemOverdueEvent record in Maliev.LifecycleService.Domain/Events/OnboardingItemOverdueEvent.cs
- [x] T045 Create EventPublisher in Maliev.LifecycleService.Infrastructure/Messaging/EventPublisher.cs with retry/dead-letter logic

### API Foundation

- [x] T046 Create Program.cs in Maliev.LifecycleService.Api/ with ServiceDefaults, database, caching, messaging, and authentication
- [x] T047 Configure MassTransit with exponential backoff retry (3 attempts) and dead-letter queue
- [x] T048 Add permission authorization configuration in Program.cs
- [x] T049 [P] Create AuditLoggingMiddleware in Maliev.LifecycleService.Api/Middleware/AuditLoggingMiddleware.cs
- [x] T050 [P] Create MetricsExtensions in Maliev.LifecycleService.Api/Extensions/MetricsExtensions.cs with counters and gauges
- [x] T051 Create Dockerfile in Maliev.LifecycleService.Api/Dockerfile with multi-stage build, app user, BuildKit secrets
- [x] T052 [P] Create LifecycleIAMRegistrationService in Maliev.LifecycleService.Infrastructure/IAM/LifecycleIAMRegistrationService.cs

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Automated Employee Onboarding (Priority: P1) 🎯 MVP

**Goal**: Automatically create and track onboarding checklists when employees are hired, with department-specific templates, task assignments, progress tracking, and completion events

**Independent Test**: Create a new employee record and verify that an onboarding checklist is automatically generated with department-specific tasks, due dates calculated correctly, and the checklist can be viewed and updated independently

### Domain Entities for US1

- [x] T053 [P] [US1] Create OnboardingChecklist entity in Maliev.LifecycleService.Domain/Entities/OnboardingChecklist.cs
- [x] T054 [P] [US1] Create OnboardingItem entity in Maliev.LifecycleService.Domain/Entities/OnboardingItem.cs
- [x] T055 [P] [US1] Create OnboardingTemplate entity in Maliev.LifecycleService.Domain/Entities/OnboardingTemplate.cs
- [x] T056 [P] [US1] Create OnboardingTemplateItem entity in Maliev.LifecycleService.Domain/Entities/OnboardingTemplateItem.cs

### Infrastructure for US1

- [x] T057 [P] [US1] Create OnboardingChecklistConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OnboardingChecklistConfiguration.cs
- [x] T058 [P] [US1] Create OnboardingItemConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OnboardingItemConfiguration.cs
- [x] T059 [P] [US1] Create OnboardingTemplateConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OnboardingTemplateConfiguration.cs
- [x] T060 [P] [US1] Create OnboardingTemplateItemConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OnboardingTemplateItemConfiguration.cs
- [x] T061 [US1] Add OnboardingChecklist, OnboardingItem, OnboardingTemplate, OnboardingTemplateItem to DbContext
- [x] T062 [US1] Create migration for onboarding tables
- [x] T063 [P] [US1] Create IOnboardingRepository interface in Maliev.LifecycleService.Application/Interfaces/IOnboardingRepository.cs
- [x] T064 [P] [US1] Create ITemplateRepository interface in Maliev.LifecycleService.Application/Interfaces/ITemplateRepository.cs
- [x] T065 [US1] Create OnboardingRepository in Maliev.LifecycleService.Infrastructure/Repositories/OnboardingRepository.cs
- [x] T066 [US1] Create TemplateRepository in Maliev.LifecycleService.Infrastructure/Repositories/TemplateRepository.cs
- [x] T067 [US1] Create TemplateCacheService in Maliev.LifecycleService.Infrastructure/Caching/TemplateCacheService.cs with 1-hour TTL

### Application Layer for US1

- [x] T068 [P] [US1] Create OnboardingStatusDto in Maliev.LifecycleService.Application/DTOs/OnboardingStatusDto.cs
- [x] T069 [P] [US1] Create OnboardingItemDto in Maliev.LifecycleService.Application/DTOs/OnboardingItemDto.cs
- [x] T070 [US1] Create OnboardingMapper in Maliev.LifecycleService.Application/Mappers/OnboardingMapper.cs with explicit mapping
- [x] T071 [P] [US1] Create GetOnboardingStatusQuery in Maliev.LifecycleService.Application/Queries/Onboarding/GetOnboardingStatusQuery.cs
- [x] T072 [P] [US1] Create GetPendingOnboardingsQuery in Maliev.LifecycleService.Application/Queries/Onboarding/GetPendingOnboardingsQuery.cs
- [x] T073 [US1] Create GetOnboardingStatusQueryHandler in Maliev.LifecycleService.Application/Queries/Onboarding/Handlers/GetOnboardingStatusQueryHandler.cs
- [x] T074 [US1] Create GetPendingOnboardingsQueryHandler in Maliev.LifecycleService.Application/Queries/Onboarding/Handlers/GetPendingOnboardingsQueryHandler.cs
- [x] T075 [P] [US1] Create CompleteOnboardingItemCommand in Maliev.LifecycleService.Application/Commands/Onboarding/CompleteOnboardingItemCommand.cs
- [x] T076 [P] [US1] Create ReassignOnboardingItemCommand in Maliev.LifecycleService.Application/Commands/Onboarding/ReassignOnboardingItemCommand.cs
- [x] T077 [US1] Create CompleteOnboardingItemCommandHandler in Maliev.LifecycleService.Application/Commands/Onboarding/Handlers/CompleteOnboardingItemCommandHandler.cs with concurrent completion check
- [x] T078 [US1] Create ReassignOnboardingItemCommandHandler in Maliev.LifecycleService.Application/Commands/Onboarding/Handlers/ReassignOnboardingItemCommandHandler.cs

### Event Consumer for US1

- [x] T079 [US1] Create EmployeeCreatedEventConsumer in Maliev.LifecycleService.Infrastructure/Consumers/EmployeeCreatedEventConsumer.cs to auto-create onboarding checklists
- [x] T080 [US1] Register EmployeeCreatedEventConsumer in Program.cs MassTransit configuration

### API for US1

- [x] T081 [US1] Create OnboardingController in Maliev.LifecycleService.Api/Controllers/OnboardingController.cs
- [x] T082 [US1] Implement GET /lifecycle/v1/employees/{employeeId}/onboarding/status endpoint with lifecycle.manage permission
- [x] T083 [US1] Implement PUT /lifecycle/v1/onboarding-items/{itemId}/complete endpoint with lifecycle.manage permission
- [x] T084 [US1] Implement PUT /lifecycle/v1/onboarding-items/{itemId}/reassign endpoint with lifecycle.manage permission
- [x] T085 [US1] Implement GET /lifecycle/v1/onboarding/pending endpoint with lifecycle.manage permission and pagination
- [x] T086 [US1] Register all query/command handlers in Program.cs DI container
- [x] T087 [US1] Add onboarding metrics (lifecycle_onboarding_started_total, lifecycle_active_onboardings) in MetricsExtensions

**Checkpoint**: At this point, User Story 1 should be fully functional - automatic onboarding creation, status tracking, task completion, reassignment, and event publishing

---

## Phase 4: User Story 3 - Employee Offboarding Workflow (Priority: P1)

**Goal**: Track offboarding tasks with paycheck-blocking logic, access revocation triggering, and completion events

**Independent Test**: Create an offboarding checklist for a terminated employee, verify that paycheck-blocking tasks prevent completion, and confirm that access revocation is triggered when all required tasks are done

### Domain Entities for US3

- [x] T088 [P] [US3] Create OffboardingChecklist entity in Maliev.LifecycleService.Domain/Entities/OffboardingChecklist.cs
- [x] T089 [P] [US3] Create OffboardingTask entity in Maliev.LifecycleService.Domain/Entities/OffboardingTask.cs

### Infrastructure for US3

- [x] T090 [P] [US3] Create OffboardingChecklistConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OffboardingChecklistConfiguration.cs
- [x] T091 [P] [US3] Create OffboardingTaskConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OffboardingTaskConfiguration.cs
- [x] T092 [US3] Add OffboardingChecklist and OffboardingTask to DbContext
- [x] T093 [US3] Create migration for offboarding tables
- [x] T094 [P] [US3] Create IOffboardingRepository interface in Maliev.LifecycleService.Application/Interfaces/IOffboardingRepository.cs
- [x] T095 [US3] Create OffboardingRepository in Maliev.LifecycleService.Infrastructure/Repositories/OffboardingRepository.cs

### Application Layer for US3

- [x] T096 [P] [US3] Create OffboardingStatusDto in Maliev.LifecycleService.Application/DTOs/OffboardingStatusDto.cs
- [x] T097 [P] [US3] Create OffboardingTaskDto in Maliev.LifecycleService.Application/DTOs/OffboardingTaskDto.cs
- [x] T098 [US3] Create OffboardingMapper in Maliev.LifecycleService.Application/Mappers/OffboardingMapper.cs with explicit mapping
- [x] T099 [US3] Create PaycheckBlockingService in Maliev.LifecycleService.Application/Services/PaycheckBlockingService.cs to calculate PaycheckReleaseBlocked flag
- [x] T100 [P] [US3] Create StartOffboardingCommand in Maliev.LifecycleService.Application/Commands/Offboarding/StartOffboardingCommand.cs
- [x] T101 [P] [US3] Create CompleteOffboardingTaskCommand in Maliev.LifecycleService.Application/Commands/Offboarding/CompleteOffboardingTaskCommand.cs
- [x] T102 [P] [US3] Create ReassignOffboardingTaskCommand in Maliev.LifecycleService.Application/Commands/Offboarding/ReassignOffboardingTaskCommand.cs
- [x] T103 [US3] Create StartOffboardingCommandHandler in Maliev.LifecycleService.Application/Commands/Offboarding/Handlers/StartOffboardingCommandHandler.cs with validation
- [x] T104 [US3] Create StartOffboardingValidator in Maliev.LifecycleService.Application/Validators/StartOffboardingValidator.cs using DataAnnotations
- [x] T105 [US3] Create CompleteOffboardingTaskCommandHandler in Maliev.LifecycleService.Application/Commands/Offboarding/Handlers/CompleteOffboardingTaskCommandHandler.cs with paycheck blocking update
- [x] T106 [US3] Create ReassignOffboardingTaskCommandHandler in Maliev.LifecycleService.Application/Commands/Offboarding/Handlers/ReassignOffboardingTaskCommandHandler.cs
- [x] T107 [P] [US3] Create GetOffboardingStatusQuery in Maliev.LifecycleService.Application/Queries/Offboarding/GetOffboardingStatusQuery.cs
- [x] T108 [P] [US3] Create GetPendingOffboardingsQuery in Maliev.LifecycleService.Application/Queries/Offboarding/GetPendingOffboardingsQuery.cs
- [x] T109 [US3] Create GetOffboardingStatusQueryHandler in Maliev.LifecycleService.Application/Queries/Offboarding/Handlers/GetOffboardingStatusQueryHandler.cs
- [x] T110 [US3] Create GetPendingOffboardingsQueryHandler in Maliev.LifecycleService.Application/Queries/Offboarding/Handlers/GetPendingOffboardingsQueryHandler.cs

### Background Service for US3

- [x] T111 [US3] Create AccessRevocationBackgroundService in Maliev.LifecycleService.Infrastructure/BackgroundServices/AccessRevocationBackgroundService.cs running hourly
- [x] T112 [US3] Register AccessRevocationBackgroundService in Program.cs

### API for US3

- [x] T113 [US3] Create OffboardingController in Maliev.LifecycleService.Api/Controllers/OffboardingController.cs
- [x] T114 [US3] Implement POST /lifecycle/v1/employees/{employeeId}/offboarding/start endpoint with lifecycle.manage permission
- [x] T115 [US3] Implement GET /lifecycle/v1/employees/{employeeId}/offboarding/status endpoint with lifecycle.manage permission
- [x] T116 [US3] Implement PUT /lifecycle/v1/offboarding-tasks/{taskId}/complete endpoint with lifecycle.manage permission
- [x] T117 [US3] Implement PUT /lifecycle/v1/offboarding-tasks/{taskId}/reassign endpoint with lifecycle.manage permission
- [x] T118 [US3] Implement GET /lifecycle/v1/offboarding/pending endpoint with lifecycle.manage permission and pagination
- [x] T119 [US3] Register all offboarding query/command handlers in Program.cs DI container
- [x] T120 [US3] Add offboarding metrics (lifecycle_offboarding_started_total, lifecycle_paycheck_blocked_count) in MetricsExtensions

**Checkpoint**: At this point, User Story 3 should be fully functional - offboarding creation, paycheck blocking, task completion, access revocation triggering

---

## Phase 5: User Story 5 - Onboarding Template Management (Priority: P2)

**Goal**: Enable administrators to create, update, and manage onboarding templates with department-specific customization

**Independent Test**: Create, update, and activate templates, then verify that newly created onboarding checklists use the correct template based on department matching or default fallback

### Application Layer for US5

- [x] T121 [P] [US5] Create TemplateDto in Maliev.LifecycleService.Application/DTOs/TemplateDto.cs
- [x] T122 [US5] Create TemplateMapper in Maliev.LifecycleService.Application/Mappers/TemplateMapper.cs with explicit mapping
- [x] T123 [P] [US5] Create CreateTemplateCommand in Maliev.LifecycleService.Application/Commands/Templates/CreateTemplateCommand.cs
- [x] T124 [P] [US5] Create UpdateTemplateCommand in Maliev.LifecycleService.Application/Commands/Templates/UpdateTemplateCommand.cs
- [x] T125 [P] [US5] Create DeleteTemplateCommand in Maliev.LifecycleService.Application/Commands/Templates/DeleteTemplateCommand.cs
- [x] T126 [US5] Create CreateTemplateCommandHandler in Maliev.LifecycleService.Application/Commands/Templates/Handlers/CreateTemplateCommandHandler.cs with cache invalidation
- [x] T127 [US5] Create UpdateTemplateCommandHandler in Maliev.LifecycleService.Application/Commands/Templates/Handlers/UpdateTemplateCommandHandler.cs with cache invalidation
- [x] T128 [US5] Create DeleteTemplateCommandHandler in Maliev.LifecycleService.Application/Commands/Templates/Handlers/DeleteTemplateCommandHandler.cs with soft delete (is_active=false)
- [x] T129 [P] [US5] Create GetTemplateQuery in Maliev.LifecycleService.Application/Queries/Templates/GetTemplateQuery.cs
- [x] T130 [P] [US5] Create ListTemplatesQuery in Maliev.LifecycleService.Application/Queries/Templates/ListTemplatesQuery.cs
- [x] T131 [US5] Create GetTemplateQueryHandler in Maliev.LifecycleService.Application/Queries/Templates/Handlers/GetTemplateQueryHandler.cs
- [x] T132 [US5] Create ListTemplatesQueryHandler in Maliev.LifecycleService.Application/Queries/Templates/Handlers/ListTemplatesQueryHandler.cs
- [x] T133 [US5] Update OnboardingTemplateService to use TemplateCacheService for GetTemplateForDepartmentAsync method

### API for US5

- [x] T134 [US5] Create TemplatesController in Maliev.LifecycleService.Api/Controllers/TemplatesController.cs
- [x] T135 [US5] Implement GET /lifecycle/v1/templates endpoint with lifecycle.admin permission
- [x] T136 [US5] Implement POST /lifecycle/v1/templates endpoint with lifecycle.admin permission
- [x] T137 [US5] Implement PUT /lifecycle/v1/templates/{id} endpoint with lifecycle.admin permission
- [x] T138 [US5] Implement DELETE /lifecycle/v1/templates/{id} endpoint with lifecycle.admin permission
- [x] T139 [US5] Register all template query/command handlers in Program.cs DI container

**Checkpoint**: At this point, User Story 5 should be fully functional - template CRUD operations with caching and department-specific selection

---

## Phase 6: User Story 2 - Manual Onboarding Initiation (Priority: P2)

**Goal**: Allow HR managers to manually start onboarding workflows with template selection and custom start dates

**Independent Test**: Have an HR manager manually trigger onboarding for an existing employee ID, selecting a template and start date, and verify the checklist is created correctly

### Application Layer for US2

- [x] T140 [P] [US2] Create StartOnboardingCommand in Maliev.LifecycleService.Application/Commands/Onboarding/StartOnboardingCommand.cs
- [x] T141 [US2] Create StartOnboardingCommandHandler in Maliev.LifecycleService.Application/Commands/Onboarding/Handlers/StartOnboardingCommandHandler.cs with duplicate check
- [x] T142 [US2] Create StartOnboardingValidator in Maliev.LifecycleService.Application/Validators/StartOnboardingValidator.cs using DataAnnotations

### API for US2

- [x] T143 [US2] Implement POST /lifecycle/v1/employees/{employeeId}/onboarding/start endpoint in OnboardingController with lifecycle.manage permission
- [x] T144 [US2] Register StartOnboardingCommandHandler in Program.cs DI container

**Checkpoint**: At this point, User Story 2 should be fully functional - manual onboarding initiation with template selection

---

## Phase 7: User Story 4 - Exit Interview Recording (Priority: P3)

**Goal**: Record and retrieve structured exit interview data with restricted access

**Independent Test**: Record an exit interview for an employee with an active offboarding checklist, verify data persistence, and confirm retrieval with access restrictions

### Domain Entities for US4

- [x] T145 [P] [US4] Create ExitInterview entity in Maliev.LifecycleService.Domain/Entities/ExitInterview.cs

### Infrastructure for US4

- [x] T146 [P] [US4] Create ExitInterviewConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/ExitInterviewConfiguration.cs
- [x] T147 [US4] Add ExitInterview to DbContext
- [x] T148 [US4] Create migration for exit_interviews table
- [x] T149 [P] [US4] Create IExitInterviewRepository interface in Maliev.LifecycleService.Application/Interfaces/IExitInterviewRepository.cs
- [x] T150 [US4] Create ExitInterviewRepository in Maliev.LifecycleService.Infrastructure/Repositories/ExitInterviewRepository.cs with access control filter

### Application Layer for US4

- [x] T151 [P] [US4] Create ExitInterviewDto in Maliev.LifecycleService.Application/DTOs/ExitInterviewDto.cs
- [x] T152 [P] [US4] Create RecordExitInterviewCommand in Maliev.LifecycleService.Application/Commands/ExitInterview/RecordExitInterviewCommand.cs
- [x] T153 [US4] Create RecordExitInterviewCommandHandler in Maliev.LifecycleService.Application/Commands/ExitInterview/Handlers/RecordExitInterviewCommandHandler.cs
- [x] T154 [US4] Create RecordExitInterviewValidator in Maliev.LifecycleService.Application/Validators/RecordExitInterviewValidator.cs using DataAnnotations
- [x] T155 [P] [US4] Create GetExitInterviewQuery in Maliev.LifecycleService.Application/Queries/ExitInterview/GetExitInterviewQuery.cs
- [x] T156 [US4] Create GetExitInterviewQueryHandler in Maliev.LifecycleService.Application/Queries/ExitInterview/Handlers/GetExitInterviewQueryHandler.cs with access control

### API for US4

- [x] T157 [US4] Create ExitInterviewController in Maliev.LifecycleService.Api/Controllers/ExitInterviewController.cs
- [x] T158 [US4] Implement POST /lifecycle/v1/employees/{employeeId}/exit-interview endpoint with lifecycle.manage permission
- [x] T159 [US4] Implement GET /lifecycle/v1/employees/{employeeId}/exit-interview endpoint with lifecycle.manage permission and access control
- [x] T160 [US4] Register exit interview query/command handlers in Program.cs DI container
- [x] T161 [US4] Add exit interview metrics (lifecycle_exit_interviews_recorded_total) in MetricsExtensions

**Checkpoint**: At this point, User Story 4 should be fully functional - exit interview recording and retrieval with access restrictions

---

## Phase 8: User Story 6 - Overdue Task Notifications (Priority: P3)

**Goal**: Automatically identify and escalate overdue onboarding tasks via background service

**Independent Test**: Create onboarding items with past due dates, run the background reminder service, and verify that notifications and escalation events are published

### Background Service for US6

- [x] T162 [US6] Create OnboardingReminderBackgroundService in Maliev.LifecycleService.Infrastructure/BackgroundServices/OnboardingReminderBackgroundService.cs with cron schedule (0 8 * * *)
- [x] T163 [US6] Implement due-soon task identification (due within 1 day) and reminder event publishing
- [x] T164 [US6] Implement overdue task identification (3+ days overdue) and OnboardingItemOverdueEvent publishing
- [x] T165 [US6] Register OnboardingReminderBackgroundService in Program.cs
- [x] T166 [US6] Add overdue task metrics (lifecycle_overdue_tasks_count) in MetricsExtensions

**Checkpoint**: At this point, User Story 6 should be fully functional - automated reminders and escalations

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

### Testing Infrastructure

- [x] T167 [P] Create PostgresFixture in Maliev.LifecycleService.Tests/Integration/Fixtures/PostgresFixture.cs with Testcontainers
- [x] T168 [P] Create RabbitMqFixture in Maliev.LifecycleService.Tests/Integration/Fixtures/RabbitMqFixture.cs with Testcontainers
- [x] T169 [P] Create RedisFixture in Maliev.LifecycleService.Tests/Integration/Fixtures/RedisFixture.cs with Testcontainers
- [x] T170 Create base integration test class with fixture lifecycle management

### CI/CD & Deployment

- [x] T171 [P] Create .github/workflows/ci-develop.yml with build, test, and Docker build steps
- [x] T172 [P] Create .github/workflows/ci-staging.yml with deployment to staging environment
- [x] T173 [P] Create .github/workflows/ci-main.yml with deployment to production environment
- [x] T174 Configure GitHub Secrets for GITOPS_PAT (NuGet authentication) and GCP credentials

### Documentation

- [x] T175 [P] Update README.md with complete setup instructions, architecture overview, and API documentation link
- [x] T176 [P] Create CONTRIBUTING.md with development workflow and coding standards
- [x] T177 [P] Document environment variables and configuration in README.md

### Performance & Optimization

- [x] T178 Add compiled queries for GetOnboardingStatusQuery hot path
- [x] T179 Add database indexes per data-model.md specification (employee_id, status, checklist_id)
- [x] T180 Review and optimize N+1 query issues with Include() for related entities
- [x] T181 Add pagination defaults (50 items) and maximum (200 items) to pending checklist queries

### Security Hardening

- [x] T182 Validate all user inputs with DataAnnotations and manual validators
- [x] T183 Add rate limiting configuration per constitution
- [x] T184 Ensure all sensitive endpoints require authentication and authorization
- [x] T185 Review and test permission enforcement for lifecycle.manage and lifecycle.admin

### Monitoring & Observability

- [x] T186 Add histogram metrics for onboarding/offboarding duration (lifecycle_onboarding_duration_days, lifecycle_offboarding_duration_days)
- [x] T187 Add template cache hit ratio metric (lifecycle_template_cache_hit_ratio)
- [x] T188 Verify all metrics include required labels (service_name, version, region, environment)
- [x] T189 Test liveness and readiness endpoints (/lifecycle/liveness, /lifecycle/readiness)
- [x] T190 Verify structured logging outputs JSON with proper log levels per constitution

### Code Quality

- [x] T191 Run static code analysis and address any warnings (TreatWarningsAsErrors enforcement)
- [x] T192 Review explicit mapping implementations in OnboardingMapper, OffboardingMapper, TemplateMapper
- [x] T193 Ensure no AutoMapper, FluentValidation, or FluentAssertions dependencies exist
- [x] T194 Code review for constitution compliance (Docker, naming, structure, logging)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-8)**: All depend on Foundational phase completion
  - US1 (Phase 3 - P1): Can start after Foundational - No dependencies on other stories
  - US3 (Phase 4 - P1): Can start after Foundational - No dependencies on other stories
  - US5 (Phase 5 - P2): Can start after Foundational - Uses OnboardingTemplate from US1 but independently testable
  - US2 (Phase 6 - P2): Can start after Foundational - Uses OnboardingTemplate from US1 and US5
  - US4 (Phase 7 - P3): Can start after Foundational - Requires OffboardingChecklist from US3
  - US6 (Phase 8 - P3): Can start after Foundational - Requires OnboardingChecklist and OnboardingItem from US1
- **Polish (Phase 9)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 3 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 5 (P2)**: Can start after Foundational (Phase 2) - Uses OnboardingTemplate entities from US1
- **User Story 2 (P2)**: Recommended after US1 and US5 - Uses StartOnboardingCommand with template selection
- **User Story 4 (P3)**: Requires US3 complete - Links ExitInterview to OffboardingChecklist
- **User Story 6 (P3)**: Requires US1 complete - Processes OnboardingItem entities for overdue detection

### Within Each User Story

- Domain entities before EF Core configurations
- EF Core configurations before migrations
- Repositories before services
- Commands/queries before handlers
- Handlers before controllers
- Controllers before endpoint registration
- Core implementation before integration with other stories

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel (T003, T006, T007, T010-T013, T015-T019)
- All Foundational enum/event tasks can run in parallel (T020-T027, T036-T044, T049-T050)
- Once Foundational phase completes:
  - US1 (Phase 3) and US3 (Phase 4) can run in parallel (both P1 priority, no dependencies)
  - US5 (Phase 5) can start in parallel with US1/US3 (templates are foundational for US1 but independently testable)
- Within each story, tasks marked [P] can run in parallel:
  - Entity creation tasks (different files)
  - Configuration tasks (different files)
  - DTO creation tasks (different files)
  - Query/command creation tasks (different files)

---

## Parallel Example: User Story 1 (Automated Onboarding)

```bash
# Launch all domain entities for US1 together:
Task T053: "Create OnboardingChecklist entity in Maliev.LifecycleService.Domain/Entities/OnboardingChecklist.cs"
Task T054: "Create OnboardingItem entity in Maliev.LifecycleService.Domain/Entities/OnboardingItem.cs"
Task T055: "Create OnboardingTemplate entity in Maliev.LifecycleService.Domain/Entities/OnboardingTemplate.cs"
Task T056: "Create OnboardingTemplateItem entity in Maliev.LifecycleService.Domain/Entities/OnboardingTemplateItem.cs"

# Launch all EF Core configurations for US1 together:
Task T057: "Create OnboardingChecklistConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OnboardingChecklistConfiguration.cs"
Task T058: "Create OnboardingItemConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OnboardingItemConfiguration.cs"
Task T059: "Create OnboardingTemplateConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OnboardingTemplateConfiguration.cs"
Task T060: "Create OnboardingTemplateItemConfiguration in Maliev.LifecycleService.Infrastructure/Data/Configurations/OnboardingTemplateItemConfiguration.cs"

# Launch all DTOs for US1 together:
Task T068: "Create OnboardingStatusDto in Maliev.LifecycleService.Application/DTOs/OnboardingStatusDto.cs"
Task T069: "Create OnboardingItemDto in Maliev.LifecycleService.Application/DTOs/OnboardingItemDto.cs"
```

---

## Parallel Example: Multiple User Stories

```bash
# After Foundational phase completes, launch P1 stories in parallel:

# Developer A works on US1 (Automated Onboarding):
Tasks T053-T087

# Developer B works on US3 (Offboarding Workflow):
Tasks T088-T120

# Developer C works on US5 (Template Management):
Tasks T121-T139 (can start once US1 entities are done, or work independently and integrate later)
```

---

## Implementation Strategy

### MVP First (User Stories 1 & 3 Only - Both P1)

1. Complete Phase 1: Setup (T001-T019)
2. Complete Phase 2: Foundational (T020-T052) - CRITICAL, blocks all stories
3. Complete Phase 3: User Story 1 - Automated Onboarding (T053-T087)
4. Complete Phase 4: User Story 3 - Offboarding Workflow (T088-T120)
5. **STOP and VALIDATE**: Test US1 and US3 independently
6. Deploy/demo MVP with core onboarding and offboarding workflows

### Incremental Delivery (P1 → P2 → P3)

1. Complete Setup + Foundational → Foundation ready
2. Add US1 (Automated Onboarding) → Test independently → Deploy (MVP core)
3. Add US3 (Offboarding Workflow) → Test independently → Deploy (MVP complete)
4. Add US5 (Template Management) → Test independently → Deploy (P2 enhancement)
5. Add US2 (Manual Onboarding) → Test independently → Deploy (P2 complete)
6. Add US4 (Exit Interviews) → Test independently → Deploy (P3 enhancement)
7. Add US6 (Overdue Notifications) → Test independently → Deploy (P3 complete)
8. Complete Phase 9 (Polish) → Production-ready

### Parallel Team Strategy (3 Developers)

With three developers after Foundational phase:

1. **Team completes Setup + Foundational together** (T001-T052)
2. Once Foundational is done:
   - **Developer A**: User Story 1 - Automated Onboarding (T053-T087)
   - **Developer B**: User Story 3 - Offboarding Workflow (T088-T120)
   - **Developer C**: User Story 5 - Template Management (T121-T139, integrates with US1)
3. After P1 stories complete:
   - **Developer A**: User Story 2 - Manual Onboarding (T140-T144)
   - **Developer B**: User Story 4 - Exit Interviews (T145-T161)
   - **Developer C**: User Story 6 - Overdue Notifications (T162-T166)
4. Team collaborates on Phase 9 Polish (T167-T194)

---

## Task Summary

**Total Tasks**: 194

**Tasks by Phase**:
- Phase 1 (Setup): 19 tasks
- Phase 2 (Foundational): 33 tasks (BLOCKING)
- Phase 3 (US1 - P1): 35 tasks
- Phase 4 (US3 - P1): 33 tasks
- Phase 5 (US5 - P2): 19 tasks
- Phase 6 (US2 - P2): 5 tasks
- Phase 7 (US4 - P3): 17 tasks
- Phase 8 (US6 - P3): 5 tasks
- Phase 9 (Polish): 28 tasks

**Tasks by User Story**:
- US1 (Automated Onboarding - P1): 35 tasks
- US3 (Offboarding Workflow - P1): 33 tasks
- US5 (Template Management - P2): 19 tasks
- US2 (Manual Onboarding - P2): 5 tasks
- US4 (Exit Interviews - P3): 17 tasks
- US6 (Overdue Notifications - P3): 5 tasks

**Parallel Opportunities**: 67 tasks marked [P] can run in parallel within their phase

**MVP Scope**: Phase 1 (Setup) + Phase 2 (Foundational) + Phase 3 (US1) + Phase 4 (US3) = 120 tasks for core functionality

**Independent Test Validation**: Each user story phase includes checkpoint with independent test criteria from spec.md

---

## Notes

- **[P] tasks**: Different files, no dependencies - can run in parallel
- **[Story] label**: Maps task to specific user story for traceability (US1-US6)
- **Each user story**: Independently completable and testable per spec.md acceptance criteria
- **Constitution compliance**: All tasks follow constitution requirements (flat structure, no AutoMapper/FluentValidation/FluentAssertions, Testcontainers, explicit mapping)
- **Test-First Development**: Although test tasks not included (not explicitly requested), implement with Red-Green-Refactor cycle
- **Commit frequency**: Commit after each task or logical group
- **Checkpoint validation**: Stop at each user story checkpoint to validate independent functionality
- **File path clarity**: All tasks include exact file paths per plan.md structure
