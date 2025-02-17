# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy all project files
COPY . .

# Restore .NET dependencies with the correct path
RUN dotnet restore "/app/Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj"

# Install Node.js and npm
RUN apt-get update && apt-get install -y nodejs npm

# Build Angular application
WORKDIR /app/VendingMachineApp.Client
RUN npm install
RUN npm run build:prod

# Publish .NET application with correct path
WORKDIR /app
RUN dotnet publish "/app/Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj" -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Create wwwroot directory
RUN mkdir -p wwwroot

# Copy published output
COPY --from=build-env /app/out .

# Copy Angular files
COPY --from=build-env /app/VendingMachineApp.Client/dist/vending-machine-app.client/* /app/wwwroot/

ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]