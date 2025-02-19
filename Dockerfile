# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy all project files
COPY . .

# Install Entity Framework Core Tools (without comment in same line)
RUN dotnet tool install --global dotnet-ef
ENV PATH="/root/.dotnet/tools:${PATH}"

# Restore .NET dependencies
RUN dotnet restore "/app/Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj"

# Build and publish .NET application
RUN dotnet publish "/app/Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj" -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]