# Stage 1: Build the React frontend
FROM node:24-bookworm-slim AS frontend
WORKDIR /web
COPY src/MediHz.Web/package.json src/MediHz.Web/package-lock.json ./
RUN npm ci
COPY src/MediHz.Web/ ./
ARG VITE_GOOGLE_CLIENT_ID
ENV VITE_GOOGLE_CLIENT_ID=$VITE_GOOGLE_CLIENT_ID
RUN npm run build

# Stage 2: Restore and publish the .NET API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/MediHz.Domain/MediHz.Domain.csproj", "src/MediHz.Domain/"]
COPY ["src/MediHz.Application/MediHz.Application.csproj", "src/MediHz.Application/"]
COPY ["src/MediHz.Infrastructure/MediHz.Infrastructure.csproj", "src/MediHz.Infrastructure/"]
COPY ["src/MediHz.Api/MediHz.Api.csproj", "src/MediHz.Api/"]
RUN dotnet restore "src/MediHz.Api/MediHz.Api.csproj"

COPY . .
WORKDIR /src/src/MediHz.Api
RUN dotnet publish "MediHz.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Stage 3: Runtime image with API + SPA
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
COPY --from=frontend /web/dist ./wwwroot
ENTRYPOINT ["dotnet", "MediHz.Api.dll"]
