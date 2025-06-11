#!/bin/bash

# Deploy RecipeShare with Angular Frontend
# This script builds both Angular and .NET, then deploys to Azure

set -e

# Configuration
APP_NAME="recipeshare-app-1749580253"
RESOURCE_GROUP="recipeshare-rg"

echo "🚀 Deploying RecipeShare with Angular frontend..."

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed. Please install it first."
    exit 1
fi

# Check if npm is installed
if ! command -v npm &> /dev/null; then
    echo "❌ npm is not installed. Please install it first."
    exit 1
fi

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed. Please install it first."
    exit 1
fi

echo "✅ All required tools are installed"

# Build the Angular frontend
echo "🔨 Building Angular frontend..."
cd RecipeShareAngularApp
npm install
npm run build -- --configuration production
cd ..

# Check if Angular build was successful
if [ ! -d "RecipeShareAngularApp/dist/recipe-share" ]; then
    echo "❌ Angular build failed - dist/recipe-share directory not found"
    exit 1
fi

echo "✅ Angular frontend built successfully"

# Build the .NET application
echo "🔨 Building .NET application..."
cd RecipeShare
dotnet publish -c Release -o ../deploy-package

# Check if .NET build was successful
if [ ! -f "../deploy-package/RecipeShare.dll" ]; then
    echo "❌ .NET build failed - RecipeShare.dll not found"
    exit 1
fi

cd ..

# Copy Angular build files to wwwroot
echo "📁 Copying Angular build files to wwwroot..."
mkdir -p deploy-package/wwwroot
cp -r RecipeShareAngularApp/dist/recipe-share/* deploy-package/wwwroot/

# Verify the frontend files are in place
if [ ! -f "deploy-package/wwwroot/index.html" ]; then
    echo "❌ Angular index.html not found in wwwroot"
    exit 1
fi

echo "✅ Frontend files copied successfully"

# Create deployment package
echo "📦 Creating deployment package..."
cd deploy-package
zip -r ../deploy-package.zip .
cd ..

# Deploy to Azure
echo "🚀 Deploying to Azure..."
az webapp deployment source config-zip \
    --name $APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --src deploy-package.zip

# Get the app URL
APP_URL=$(az webapp show --name $APP_NAME --resource-group $RESOURCE_GROUP --query defaultHostName --output tsv)

echo "✅ Deployment completed successfully!"
echo "🌍 Your app is available at: https://$APP_URL"
echo "📊 Monitor your app at: https://portal.azure.com/#@/resource/subscriptions/*/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$APP_NAME/overview"

# Clean up
echo "🧹 Cleaning up deployment files..."
rm -rf deploy-package
rm -f deploy-package.zip

echo "🎉 RecipeShare App deployed with Angular frontend!"
echo ""
echo "📝 What was deployed:"
echo "1. ✅ .NET 8.0 Web API backend"
echo "2. ✅ Angular frontend (built for production)"
echo "3. ✅ Database connection (already configured)"
echo "4. ✅ All files properly organized in wwwroot/"

# Optional: Open the app in browser
if command -v xdg-open &> /dev/null; then
    xdg-open "https://$APP_URL"
elif command -v open &> /dev/null; then
    open "https://$APP_URL"
fi 