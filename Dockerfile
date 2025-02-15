# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy .csproj file using absolute path (Corrected and CRUCIAL)
COPY ["Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj", "/app/Vending-Machine-App/Vending-Machine-App/"]

# Change directory to the folder containing the .csproj file
WORKDIR /app/Vending-Machine-App/Vending-Machine-App

RUN dotnet restore

# Change directory back to /app and copy all project files
WORKDIR /app
COPY . .

# Clean existing publish output (Important!)
RUN rm -rf out

# Publish to absolute path
RUN dotnet publish Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj -c Release -o /app/out --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Create wwwroot (important!)
RUN mkdir -p wwwroot

# Copy published output and Angular files
COPY --from=build-env /app/out .
COPY --from=build-env /app/VendingMachineApp.Client/dist/vending-machine-app.client/* /app/wwwroot/

ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]