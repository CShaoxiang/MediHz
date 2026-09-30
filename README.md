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
- `MediHz.Web` — React Frontend.
- `MediHz.Tests` — .NET unit tests.

## Quickstart

### Backend

Prerequisites:
- .NET 10 SDK.
```bash
cd MediHz
dotnet restore
dotnet run --project src/MediHz.Api -lp development
```
Runs the project in development mode on port 5001. API reference available at <http://localhost:5001/scalar/v1>.

```bash
dotnet tool restore
```
Installs necessary dotnet tools for development. These currently include:
- [CSharpier](https://csharpier.com/) for opinionated formatting.

### Database

Start PostgreSQL:
```bash
docker compose up -d
```
Apply the latest EF Core migration:
```bash
dotnet ef database update --project src/MediHz.Infrastructure --startup-project src/MediHz.Api
```
To update entities and create a migration:
```bash
dotnet ef migrations add <Placerholder-migration-name> --project src/MediHz.Infrastructure --startup-project src/MediHz.Api
dotnet ef database update --project src/MediHz.Infrastructure --startup-project src/MediHz.Api
```

### Frontend

Prerequisites:
- Node 24.
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

There is a pre-commit hooks to automate the required formatting and linting processes in `.githooks/pre-commit`. It can be enabled to automatically run on commit and save with `git config core.hooksPath .githooks`.

### Review

Each Merge Request (MR) must be reviewed by one contributor that did not contribute code to the MR. Reviews are expected to be raise valid concerns around functionality, code quality, maintainability, and clarify concepts in code.

MRs should follow the MR template in the repo. Which is automatically used when creating one through the web-ui or can be referenced from `./.gitlab/merge-request-templates/default.md` by agents wishing to raise MRs on behalf of the user.

The MR title should align with [Commit Names](#commits).

#### CI/CD

All MRs must pass the following checks before they can be merged:
1. Testing - all dotnet tests in `MediHz.Tests` should pass.
1. Backend formatting - CSharpier must be ran on backend files.
1. Frontend formatting, linting, and typechecking - Prettier, eslint, and tsc checks must pass on the frontend.
```bash
dotnet tool restore
dotnet csharpier check .
dotnet test

cd src/MediHz.Web
npm run format:check
npm run lint
npm run typecheck
npm run build
```

### Deployment
The deployment is a single Docker image containing the React SPA and .NET API. Terraform manages Azure Container Registry, PostgreSQL Flexible Server, App Service Plan, and Linux Web App; `deploy.sh` applies infrastructure, migrates the database, builds and pushes the image, and restarts the web app.

Copy `terraform.tfvars.example` to the ignored `terraform.tfvars` file and define the Azure subscription, resource names, capacity, database credentials, and Web App settings for the target environment. This keeps `main.tf` reusable across machines and Azure subscriptions. Never commit `.terraform/`, `terraform.tfstate*`, a real `terraform.tfvars`, Azure credentials, or production secrets.

#### Prerequisites

- Azure CLI authenticated to the deployment subscription: `az login`.
- Terraform, Docker, .NET 10 SDK, and the EF Core tool installed.
- Docker Desktop running.
- Access to the shared Terraform state backend and the Azure resource group.

#### Deploy an update

1. Copy and configure the local deployment values:

   ```bash
   cp terraform.tfvars.example terraform.tfvars
   ```
2. Initialise Terraform against the team-managed remote backend. The platform owner must provide the backend configuration; do not create or share production state through Git.
   ```bash
   terraform init
   terraform plan
   ```

   When running `terraform plan` manually, Terraform prompts for the current
   `developer_public_ip`. `deploy.sh` detects and passes that value automatically.
3. Review the plan, then run the deployment from a Bash-compatible shell:

   ```bash
   ./deploy.sh
   ```
4. Verify the deployed app and health endpoint, and review Azure App Service logs:

   ```bash
   az webapp show --name "$(terraform output -raw web_app_name)" --resource-group "$(terraform output -raw resource_group_name)" --query defaultHostName -o tsv
   az webapp log tail --name "$(terraform output -raw web_app_name)" --resource-group "$(terraform output -raw resource_group_name)"
   ```


