# Build stage
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy .csproj and restore dependencies
COPY ["Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj", "./"]
RUN dotnet restore "Vending-Machine-App.csproj"

# Copy everything else (including wwwroot)
COPY . ./

# Build and publish
RUN dotnet publish -c Release -o out

# Copy Angular assets (wwwroot) to the published output
RUN cp -r Vending-Machine-App/Vending-Machine-App/wwwroot out/wwwroot/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]