# Use the .NET 10 ASP.NET runtime for the final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET 10 SDK to compile the code
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
# Notice how we also copy the Shared.Data project!
COPY ["Order.API/Order.API.csproj", "Order.API/"]
COPY ["Shared.Data/Shared.Data.csproj", "Shared.Data/"]
RUN dotnet restore "Order.API/Order.API.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/Order.API"
RUN dotnet build "Order.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Order.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.API.dll"]