# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy ALL the project files to retain the folder structure
COPY ["Api/TouRest.Api.csproj", "Api/"]
COPY ["Application/TouRest.Application.csproj", "Application/"]
COPY ["Domain/TouRest.Domain.csproj", "Domain/"]
COPY ["Infrastructure/TouRest.Infrastructure.csproj", "Infrastructure/"]

# 2. Restore the dependencies for the main Api project 
# (This automatically restores the others too)
RUN dotnet restore "Api/TouRest.Api.csproj"

# 3. Copy the rest of the source code for all folders
COPY . .

# 4. Move into the Api folder and build the release
WORKDIR "/src/Api"
RUN dotnet publish "TouRest.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Cloud Run port configuration
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy the final built files from Stage 1
COPY --from=build /app/publish .

# Tell the container to run the Api
ENTRYPOINT ["dotnet", "Api.dll"]
