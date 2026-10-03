#!/usr/bin/env bash
set -euo pipefail

echo "Starting Local Deployment Pipeline for MediHz..."

DEVELOPER_IP=$(curl -s https://ifconfig.me)
echo "Current Local IP: $DEVELOPER_IP"

# 2. Sync infrastructure changes just in case
echo "Syncing Terraform Infrastructure..."
terraform apply -var="developer_public_ip=$DEVELOPER_IP"

ACR_NAME=$(terraform output -raw acr_name)
ACR_LOGIN_SERVER=$(terraform output -raw acr_login_server)
POSTGRES_FQDN=$(terraform output -raw postgres_fqdn)
WEB_APP_NAME=$(terraform output -raw web_app_name)
GOOGLE_CLIENT_ID=$(terraform output -raw google_client_id)

# 3. Run Database Migrations
echo " Running Entity Framework Core Database Migrations..."
export ConnectionStrings__MediHz="Host=${POSTGRES_FQDN};Database=medihz;Username=medihzadmin;Password=MediHz-Test-Only-Password-2026!;SSL Mode=Require;"
dotnet ef database update --project src/MediHz.Infrastructure --startup-project src/MediHz.Api

# 4. Authenticate, Build and Push Container
echo "Authenticating with Azure Container Registry..."
az acr login --name "$ACR_NAME"

echo " Building Production Docker Image..."
docker build --build-arg "VITE_GOOGLE_CLIENT_ID=${GOOGLE_CLIENT_ID}" -t "${ACR_LOGIN_SERVER}/medihz-app:latest" .

echo " Pushing Docker Image to Azure Container Registry..."
docker push "${ACR_LOGIN_SERVER}/medihz-app:latest"

# 5. Restart Web App to pick up modifications
echo "Refreshing Azure Web App Instance..."
az webapp restart --name "$WEB_APP_NAME" --resource-group "$(terraform output -raw resource_group_name)"

echo "✅ MediHz App successfully deployed to Production at $(terraform output -raw web_app_url)"
