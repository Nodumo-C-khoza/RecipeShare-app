# Azure App Service Deployment Guide

## Problem Solved

This guide addresses the HTTP 503 errors caused by SQL Server LocalDB not being supported on Linux containers in Azure App Service.

## Your Database Configuration

Based on your Azure SQL Database deployment, here are your database details:

- **Server**: `recipeshare-sql-server.database.windows.net`
- **Database**: `RecipeShareDb`
- **Username**: `sqladmin@recipeshare`
- **Password**: [The password you set during database creation]
- **Resource Group**: `recipeshare-rg`
- **Location**: `Central US`

## Prerequisites

1. **Azure CLI** installed and logged in
2. **Azure SQL Database** (already created)
3. **.NET 8.0** SDK installed locally

## Quick Deployment Steps

### 1. Update the Deployment Script

Edit `deploy-azure-with-db.sh` and replace `YOUR_PASSWORD_HERE` with the actual password you set during database creation.

### 2. Run the Deployment

```bash
chmod +x deploy-azure-with-db.sh
./deploy-azure-with-db.sh
```

## Manual Configuration (Alternative)

If you prefer to configure manually:

### 1. Set Environment Variables in Azure

```bash
az webapp config appsettings set \
    --name "recipeshare-app" \
    --resource-group "recipeshare-rg" \
    --settings \
    DB_SERVER="recipeshare-sql-server.database.windows.net" \
    DB_NAME="RecipeShareDb" \
    DB_USER="sqladmin@recipeshare" \
    DB_PASSWORD="your-actual-password" \
    ASPNETCORE_ENVIRONMENT="Production"
```

### 2. Deploy the Application

```bash
# Build and publish
dotnet publish RecipeShare/RecipeShare.csproj -c Release -o ./publish

# Create deployment package
cd publish
zip -r ../publish.zip .
cd ..

# Deploy to Azure
az webapp deployment source config-zip \
    --name "recipeshare-app" \
    --resource-group "recipeshare-rg" \
    --src publish.zip
```

## Cost Optimization

Your current database is using General Purpose tier (2 vCores) which costs ~$418/month. Consider:

### For Development/Testing:
- **Downgrade to Basic tier** (~$5/month)
- **Downgrade to Standard tier** (~$30/month)

### To Downgrade:
1. Go to Azure Portal → SQL Database → RecipeShareDb
2. Click "Configure" → "Pricing tier"
3. Select "Basic" or "Standard"
4. Click "Apply"

## Troubleshooting

### Common Issues

1. **Connection Timeout**
   - ✅ Your firewall is already configured correctly
   - ✅ Azure services are allowed to connect

2. **Authentication Failed**
   - Verify the password you set during database creation
   - Check if the username is correct: `sqladmin@recipeshare`

3. **Database Not Found**
   - Database name: `RecipeShareDb`
   - Server: `recipeshare-sql-server.database.windows.net`

### Check Logs

```bash
az webapp log tail --name "recipeshare-app" --resource-group "recipeshare-rg"
```

### Verify Configuration

```bash
az webapp config appsettings list --name "recipeshare-app" --resource-group "recipeshare-rg"
```

## Security Best Practices

1. **Use Azure Key Vault** for storing the database password
2. **Enable SSL/TLS** (already configured)
3. **Regular backups** (already enabled)
4. **Monitor access** through Azure Security Center

## Next Steps

1. ✅ **Database created** - Done
2. **Update deployment script** with your password
3. **Deploy application** using the script
4. **Test the application**
5. **Consider cost optimization** (downgrade tier if needed)

## Support

If you encounter issues:
1. Check the Azure App Service logs
2. Verify database connectivity
3. Review the application logs for specific error messages
4. Ensure the password in the deployment script matches your database password 