# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy JUST the .csproj file from your PROJECT FOLDER
COPY ["2.Vending-Machine-App/Vending-Machine-App/Vending-Machine-App.csproj", "./"]
RUN dotnet restore "Vending-Machine-App.csproj"

# Copy the rest of the project files
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "Vending-Machine-App.dll"]