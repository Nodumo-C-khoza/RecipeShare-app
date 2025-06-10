# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Set environment variables for performance
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:80

# Expose port 80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "RecipeShare.dll"]