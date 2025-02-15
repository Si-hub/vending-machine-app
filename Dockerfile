FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy all project files
COPY . .

# Create wwwroot (important!)
RUN mkdir -p wwwroot

# Restore, build, and publish
RUN dotnet restore
RUN dotnet publish Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj -c Release -o /app/out --no-restore

# Copy Angular files to wwwroot
COPY VendingMachineApp.Client/dist/vending-machine-app.client/* wwwroot/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app

# Copy published output and Angular files
COPY --from=build-env /app/out .
COPY --from=build-env /app/wwwroot .

ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]