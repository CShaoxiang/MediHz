variable "developer_public_ip" {
  type        = string
  description = "Your local machine's public IPv4 address"
}

variable "subscription_id" {
  type        = string
  description = "Azure subscription ID; use null to use the active Azure CLI subscription"
  default     = null
}

variable "resource_group_name" {
  type        = string
  description = "Azure resource group name"
}

variable "location" {
  type        = string
  description = "Azure region for deployment resources"
}

variable "acr_name" {
  type        = string
  description = "Globally unique Azure Container Registry name"
}

variable "acr_sku" {
  type        = string
  description = "Azure Container Registry SKU"
}

variable "postgres_server_name" {
  type        = string
  description = "Globally unique Azure PostgreSQL server name"
}

variable "postgres_admin_login" {
  type        = string
  description = "PostgreSQL administrator login"
}

variable "postgres_admin_password" {
  type        = string
  description = "PostgreSQL administrator password"
  sensitive   = true
}

variable "postgres_database_name" {
  type        = string
  description = "PostgreSQL database name"
}

variable "postgres_sku" {
  type        = string
  description = "Azure PostgreSQL compute SKU"
}

variable "postgres_storage_mb" {
  type        = number
  description = "Azure PostgreSQL storage capacity in megabytes"
}

variable "web_app_name" {
  type        = string
  description = "Globally unique Azure Web App name"
}

variable "app_service_plan_name" {
  type        = string
  description = "Azure App Service Plan name"
}

variable "app_service_sku" {
  type        = string
  description = "Azure App Service Plan SKU"
}

variable "container_image_name" {
  type        = string
  description = "Container image name and tag in Azure Container Registry"
}

variable "storage_account_name" {
  type        = string
  description = "Globally unique Azure Storage account name for uploaded files"
}

variable "web_app_settings" {
  type        = map(string)
  description = "Non-connection-string application settings for the Azure Web App"
  sensitive   = true
}

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  subscription_id = var.subscription_id
  features {}
}

# 1. Resource Group
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

# 2. Azure Container Registry (ACR)
resource "azurerm_container_registry" "acr" {
  name                = var.acr_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = var.acr_sku
  admin_enabled       = true
}

# 3. Managed PostgreSQL Server
resource "azurerm_postgresql_flexible_server" "postgres" {
  name                   = var.postgres_server_name
  resource_group_name    = azurerm_resource_group.rg.name
  location               = azurerm_resource_group.rg.location
  version                = "16"
  administrator_login    = var.postgres_admin_login
  administrator_password = var.postgres_admin_password
  storage_mb             = var.postgres_storage_mb
  sku_name               = var.postgres_sku
  lifecycle {
    ignore_changes = [
      zone,
      high_availability
    ]
  }
}


# 4. Production Database Definition
resource "azurerm_postgresql_flexible_server_database" "db" {
  name      = var.postgres_database_name
  server_id = azurerm_postgresql_flexible_server.postgres.id
  charset   = "UTF8"
  collation = "en_US.utf8"
}

# 5. Allow Azure Internal Services to connect
resource "azurerm_postgresql_flexible_server_firewall_rule" "allow_azure" {
  name             = "allow-azure-internal"
  server_id        = azurerm_postgresql_flexible_server.postgres.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# 6. Allow your local machine IP to connect (Uses the input variable)
resource "azurerm_postgresql_flexible_server_firewall_rule" "allow_local" {
  name             = "allow-local-developer-ip"
  server_id        = azurerm_postgresql_flexible_server.postgres.id
  start_ip_address = var.developer_public_ip
  end_ip_address   = var.developer_public_ip
}

# 7. App Service Plan
resource "azurerm_service_plan" "asp" {
  name                = var.app_service_plan_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = var.app_service_sku
}


# 8. Blob Storage for uploaded audio files
resource "azurerm_storage_account" "storage" {
  name                            = var.storage_account_name
  resource_group_name             = azurerm_resource_group.rg.name
  location                        = azurerm_resource_group.rg.location
  account_tier                    = "Standard"
  account_replication_type        = "LRS"
  min_tls_version                 = "TLS1_2"
  allow_nested_items_to_be_public = false
}

resource "azurerm_storage_container" "audio" {
  name                  = "audio"
  storage_account_id    = azurerm_storage_account.storage.id
  container_access_type = "private"
}

# 9. Web App for Containers (Frontend Host)
resource "azurerm_linux_web_app" "webapp" {
  name                = var.web_app_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.asp.id

  https_only = true

  site_config {
    always_on           = true
    minimum_tls_version = "1.2"
    ftps_state          = "Disabled"
    application_stack {
      docker_image_name = var.container_image_name
      # Natively handles Docker credentials in AzureRM v4.x:
      docker_registry_url      = "https://${azurerm_container_registry.acr.login_server}"
      docker_registry_username = azurerm_container_registry.acr.admin_username
      docker_registry_password = azurerm_container_registry.acr.admin_password
    }
  }

  app_settings = merge(var.web_app_settings, {
    "ConnectionStrings__MediHz"           = "Host=${azurerm_postgresql_flexible_server.postgres.fqdn};Database=${var.postgres_database_name};Username=${var.postgres_admin_login};Password=${var.postgres_admin_password};SSL Mode=Require;"
    "ASPNETCORE_FORWARDEDHEADERS_ENABLED" = "true"
    "ConnectionStrings__BlobStorage"      = azurerm_storage_account.storage.primary_connection_string
  })
}

output "acr_name" {
  value = azurerm_container_registry.acr.name
}

output "acr_login_server" {
  value = azurerm_container_registry.acr.login_server
}

output "postgres_fqdn" {
  value = azurerm_postgresql_flexible_server.postgres.fqdn
}

output "web_app_name" {
  value = azurerm_linux_web_app.webapp.name
}

output "web_app_url" {
  value = "https://${azurerm_linux_web_app.webapp.default_hostname}"
}

output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "google_client_id" {
  value = nonsensitive(var.web_app_settings["Authentication__Google__ClientId"])
}
