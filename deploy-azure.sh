#!/bin/bash

# Azure Deployment Script for RecipeShare App
# This script deploys the application to Azure App Service

set -e

# Configuration
RESOURCE_GROUP="recipeshare-rg"
APP_NAME="recipeshare-app"
LOCATION="East US"
PLAN_NAME="recipeshare-plan"
SKU="B1"

echo "🚀 Starting Azure deployment for RecipeShare App..."

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI is not installed. Please install it first."
    exit 1
fi

# Login to Azure
echo "🔐 Logging into Azure..."
az login

# Create resource group
echo "📦 Creating resource group..."
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create App Service plan
echo "📋 Creating App Service plan..."
az appservice plan create \
    --name $PLAN_NAME \
    --resource-group $RESOURCE_GROUP \
    --sku $SKU \
    --is-linux

# Create web app
echo "🌐 Creating web app..."
az webapp create \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --plan $PLAN_NAME \
    --runtime "DOTNETCORE:8.0"

# Configure app settings
echo "⚙️ Configuring app settings..."
az webapp config appsettings set \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    WEBSITES_ENABLE_APP_SERVICE_STORAGE=false

# Enable continuous deployment from GitHub
echo "🔗 Setting up GitHub integration..."
az webapp deployment source config \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --repo-url "https://github.com/Nodumo-C-khoza/RecipeShare-app.git" \
    --branch main \
    --manual-integration

# Get the app URL
APP_URL=$(az webapp show --name $APP_NAME --resource-group $RESOURCE_GROUP --query defaultHostName --output tsv)

echo "✅ Deployment completed successfully!"
echo "🌍 Your app is available at: https://$APP_URL"
echo "📊 Monitor your app at: https://portal.azure.com/#@/resource/subscriptions/*/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$APP_NAME/overview"

# Optional: Open the app in browser
if command -v xdg-open &> /dev/null; then
    xdg-open "https://$APP_URL"
elif command -v open &> /dev/null; then
    open "https://$APP_URL"
fi

echo "🎉 RecipeShare App is now live on Azure!" 