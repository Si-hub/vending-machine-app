FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore dependencies
COPY ["2.Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj", "2.Vending-Machine-App/Vending-Machine-App/"]
RUN dotnet restore "2.Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj"

# Copy everything else and build
COPY . ./
WORKDIR /app/2.Vending-Machine-App/Vending-Machine-App
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/2.Vending-Machine-App/Vending-Machine-App/out ./
ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]
