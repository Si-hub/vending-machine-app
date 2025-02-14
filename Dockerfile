# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Add this after the WORKDIR /app command in your Dockerfile
RUN mkdir -p wwwroot

# 1. Copy and restore .csproj first
COPY ["Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj", "./"]
RUN dotnet restore

# 2. Copy everything else
COPY . ./

# 3. Clean existing publish output
RUN rm -rf out

# 4. Publish to absolute path
RUN dotnet publish -c Release -o /app/out --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]