# Stage 1. Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["HabitTracker.sln", "./"]
COPY ["src/UsersService/UsersService.Api/UsersService.Api.csproj", "src/UsersService/UsersService.Api/"]
COPY ["src/UsersService/UsersService.Application/UsersService.Application.csproj", "src/UsersService/UsersService.Application/"]
COPY ["src/UsersService/UsersService.Domain/UsersService.Domain.csproj", "src/UsersService/UsersService.Domain/"]
COPY ["src/UsersService/UsersService.Infrastructure/UsersService.Infrastructure.csproj", "src/UsersService/UsersService.Infrastructure/"]

# docker will run this command if we have changes in dependencies, else it will use cache
# in case of change it will download them 
RUN dotnet restore "src/UsersService/UsersService.Api/UsersService.Api.csproj"

COPY . .
# -c Release for build in production mode (optimized)
# /p:UseAppHost=false without OS specific executable
RUN dotnet publish "src/UsersService/UsersService.Api/UsersService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2. Take runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UsersService.Api.dll"]