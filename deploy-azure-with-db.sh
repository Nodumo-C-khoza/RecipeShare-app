#!/bin/bash

# RecipeShare Azure Deployment Script with Database Configuration
# This script deploys the application to Azure App Service and configures the database connection

set -e

echo "🚀 Starting RecipeShare deployment to Azure..."

# Configuration
RESOURCE_GROUP="recipeshare-rg"
APP_NAME="recipeshare-app"
LOCATION="Central US"
PLAN_NAME="recipeshare-plan"
SKU="B1"

# Database configuration - UPDATED WITH YOUR ACTUAL DATABASE DETAILS
# These values come from your Azure SQL Database deployment
DB_SERVER="recipeshare-app.database.windows.net"
DB_NAME="RecipeShareDb"
DB_USER="sqladmin@recipeshare"
DB_PASSWORD="Jane@1976"  # Your actual database password

echo "📋 Configuration:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  App Name: $APP_NAME"
echo "  Location: $LOCATION"
echo "  Database Server: $DB_SERVER"
echo "  Database Name: $DB_NAME"
echo "  Database User: $DB_USER"

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI is not installed. Please install it first: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Login to Azure
echo "🔐 Logging into Azure..."
az login

# Create resource group (if not exists)
echo "📦 Creating resource group..."
az group create --name $RESOURCE_GROUP --location "$LOCATION" 2>/dev/null || echo "Resource group already exists"

# Create App Service plan
echo "📋 Creating App Service plan..."
az appservice plan create \
    --name $PLAN_NAME \
    --resource-group $RESOURCE_GROUP \
    --location "$LOCATION" \
    --sku $SKU \
    --is-linux 2>/dev/null || echo "App Service plan already exists"

# Create web app
echo "🌐 Creating web app..."
az webapp create \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --plan $PLAN_NAME \
    --runtime "DOTNETCORE:8.0" 2>/dev/null || echo "Web app already exists"

# Configure environment variables for database connection
echo "🔧 Configuring database connection..."
az webapp config appsettings set \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --settings \
    DB_SERVER="$DB_SERVER" \
    DB_NAME="$DB_NAME" \
    DB_USER="$DB_USER" \
    DB_PASSWORD="$DB_PASSWORD" \
    ASPNETCORE_ENVIRONMENT="Production"

# Configure startup command
echo "⚙️ Configuring startup command..."
az webapp config set \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --startup-file "dotnet RecipeShare.dll"

# Build and publish the application
echo "🔨 Building and publishing application..."
dotnet publish RecipeShare/RecipeShare.csproj -c Release -o ./publish

# Create zip file for deployment
echo "📦 Creating deployment package..."
cd publish
zip -r ../publish.zip . -q
cd ..

# Deploy to Azure
echo "📤 Deploying to Azure..."
az webapp deployment source config-zip \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --src publish.zip

# Get the app URL
APP_URL=$(az webapp show --name $APP_NAME --resource-group $RESOURCE_GROUP --query defaultHostName --output tsv)

echo "✅ Deployment completed successfully!"
echo "🌐 Your app is available at: https://$APP_URL"
echo ""
echo "📝 Database Details:"
echo "  Server: $DB_SERVER"
echo "  Database: $DB_NAME"
echo "  User: $DB_USER"
echo ""
echo "🔧 To update database password manually:"
echo "az webapp config appsettings set --name $APP_NAME --resource-group $RESOURCE_GROUP --settings DB_PASSWORD='your-new-password'"
echo ""
echo "📊 To view application logs:"
echo "az webapp log tail --name $APP_NAME --resource-group $RESOURCE_GROUP"