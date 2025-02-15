# Build stage (using a Node.js image)
FROM node:18-alpine AS build-env
WORKDIR /app

# Copy all project files
COPY . .

# Change directory to the folder containing the .csproj file
WORKDIR /app/Vending-Machine-App

# Restore .NET dependencies
RUN dotnet restore

# Change directory back to the root of the app
WORKDIR /app

# Build Angular application
WORKDIR /app/VendingMachineApp.Client
RUN npm install
RUN npm run build:prod

# Change directory back to the root of the app
WORKDIR /app

# Clean existing publish output (Important!)
RUN rm -rf out

# Publish .NET application
RUN dotnet publish Vending-Machine-App/Vending-Machine-App.csproj -c Release -o /app/out --no-restore

# Runtime stage (using .NET runtime image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Create wwwroot (important!)
RUN mkdir -p wwwroot

# Copy published output and Angular files (Corrected)
COPY --from=build-env /app/out .
COPY --from=build-env /app/VendingMachineApp.Client/dist/vending-machine-app.client/* /app/wwwroot/

ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]