# MediHz

MediHz is a light-weight audio annotation tool for use by medical professionals in the healthcare industry. It aims to provide a simple and intuitive interface for creating annotations on audio files collaboratively at scale.

## Technology Stack

- Backend: .NET Core.
- Frontend: React, TypeScript.
- Database: PostgreSQL.

## Project Structure

- `MediHz.Domain` — core domain types with no external dependencies.
- `MediHz.Application` — service interfaces and DTOs. Depends on `Domain`.
- `MediHz.Infrastructure` — service implementations (i.e. persistence, storage, external services).
- `MediHz.Api` — HTTP controllers, SignalR hubs, and app startup (`Program.cs`).
- `MediHz.Web` — React Frontend

## Quickstart

### Backend

Prerequisites:
- .NET 10 SDK.
```bash
git clone <repo-url>
cd MediHz
dotnet restore
dotnet run --project src/MediHz.Api -lp development
```
Runs the project in development mode on port 5001. API reference available at <http://localhost:5001/scalar/v1>.

### Database Setup:

Start PostgreSQL:
```bash
docker compose up -d
```
Apply the latest EF Core migration:
```bash
dotnet ef database update --project src/MediHz.Infrastructure --startup-project src/MediHz.Api
```
To update the latest table and apply the migration :
```bash
dotnet ef migrations add <Placerholder-migration-name> --project src/MediHz.Infrastructure --startup-project src/MediHz.Api

dotnet ef database update --project src/MediHz.Infrastructure --startup-project src/MediHz.Api
```

### Frontend

```bash
git clone <repo-url>
cd MediHz/src/MediHz.web
npm run dev
```
Runs and hosts the frontend. Click on local host link provided in console to appear on web.

## Development Process

1. Create a branch according to the [Branching Strategy](#branching-strategy).
1. Create atomic commits on the branch with appropriate [Commit Messages](#commits).
1. Push your branch to the origin and create a [Merge Request](#review), ensuring the code is tested thoroughly and passes [CI/CD Checks](#cd/cd).
1. Squash merge the branch onto main when ready.
1. Once merged begin the [Deployment Process](#deployment).

### Branching Strategy

This project makes use of [Trunk Based Development](https://trunkbaseddevelopment.com/). Developers are expected to develop features on a branch off of `main`.

Branches should align with [Commit Names](#commits); `<type>/<short-description>`. For example:
- `feature/audio-annotation-editor`
- `fix/audio-upload-failure`

### Commits

Commits should be atomic, this means each commit should do just one thing. Commit messages concisely describe the work done using the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/#summary) specification.

### Review

Each Merge Request (MR) must be reviewed by one contributor that did not contribute code to the MR. Reviews are expected to be raise valid concerns around functionality, code quality, maintainability, and clarify concepts in code.

MRs should follow the MR template in the repo. Which is automatically used when creating one through the web-ui or can be referenced from `./.gitlab/merge-request-templates/default.md` by agents wishing to raise MRs on behalf of the user.

The MR title should align with [Commit Names](#commits).

#### CI/CD

TODO.

### Deployment
TODO.

