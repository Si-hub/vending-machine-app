# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy and restore .csproj first (Correct path - Nested structure)
COPY ["Vending-Machine-App/Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj", "Vending-Machine-App/Vending-Machine-App/"] # Correct Path

# Change directory to the inner folder for dotnet restore
WORKDIR /app/Vending-Machine-App/Vending-Machine-App

RUN dotnet restore

# Change directory back to app and Copy everything else (Correct Path)
WORKDIR /app
COPY . ./

# Clean existing publish output
RUN rm -rf out

# Publish to absolute path (Correct Path)
RUN dotnet publish Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj -c Release -o /app/out --no-restore # Corrected path

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Create wwwroot (important!)
RUN mkdir -p wwwroot

# Copy published output and Angular files
COPY --from=build-env /app/out .
COPY --from=build-env /app/VendingMachineApp.Client/dist/vending-machine-app.client/* /app/wwwroot/

ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]