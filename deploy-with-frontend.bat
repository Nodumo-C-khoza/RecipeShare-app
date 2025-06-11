@echo off
echo 🚀 Deploying RecipeShare with Angular frontend...

REM Check if Node.js is installed
node --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Node.js is not installed. Please install it first.
    exit /b 1
)

REM Check if npm is installed
npm --version >nul 2>&1
if errorlevel 1 (
    echo ❌ npm is not installed. Please install it first.
    exit /b 1
)

REM Check if .NET is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET SDK is not installed. Please install it first.
    exit /b 1
)

echo ✅ All required tools are installed

REM Build the Angular frontend
echo 🔨 Building Angular frontend...
cd RecipeShareAngularApp
call npm install
call npm run build -- --configuration production
cd ..

REM Check if Angular build was successful
if not exist "RecipeShareAngularApp\dist\recipe-share" (
    echo ❌ Angular build failed - dist/recipe-share directory not found
    exit /b 1
)

echo ✅ Angular frontend built successfully

REM Build the .NET application
echo 🔨 Building .NET application...
cd RecipeShare
call dotnet publish -c Release -o ..\deploy-package
cd ..

REM Check if .NET build was successful
if not exist "deploy-package\RecipeShare.dll" (
    echo ❌ .NET build failed - RecipeShare.dll not found
    exit /b 1
)

REM Copy Angular build files to wwwroot
echo 📁 Copying Angular build files to wwwroot...
if not exist "deploy-package\wwwroot" mkdir deploy-package\wwwroot
xcopy "RecipeShareAngularApp\dist\recipe-share\*" "deploy-package\wwwroot\" /E /Y

REM Verify the frontend files are in place
if not exist "deploy-package\wwwroot\index.html" (
    echo ❌ Angular index.html not found in wwwroot
    exit /b 1
)

echo ✅ Frontend files copied successfully

REM Create deployment package
echo 📦 Creating deployment package...
cd deploy-package
powershell -Command "Compress-Archive -Path * -DestinationPath ..\deploy-package.zip -Force"
cd ..

REM Deploy to Azure
echo 🚀 Deploying to Azure...
az webapp deployment source config-zip --name "recipeshare-app-1749580253" --resource-group "recipeshare-rg" --src deploy-package.zip

REM Get the app URL
for /f "tokens=*" %%i in ('az webapp show --name "recipeshare-app-1749580253" --resource-group "recipeshare-rg" --query defaultHostName --output tsv') do set APP_URL=%%i

echo ✅ Deployment completed successfully!
echo 🌍 Your app is available at: https://%APP_URL%
echo 📊 Monitor your app at: https://portal.azure.com/#@/resource/subscriptions/*/resourceGroups/recipeshare-rg/providers/Microsoft.Web/sites/recipeshare-app-1749580253/overview

REM Clean up
echo 🧹 Cleaning up deployment files...
rmdir /s /q deploy-package
del deploy-package.zip

echo 🎉 RecipeShare App deployed with Angular frontend!
echo.
echo 📝 What was deployed:
echo 1. ✅ .NET 8.0 Web API backend
echo 2. ✅ Angular frontend (built for production)
echo 3. ✅ Database connection (already configured)
echo 4. ✅ All files properly organized in wwwroot/

REM Open the app in browser
start https://%APP_URL%

pause 