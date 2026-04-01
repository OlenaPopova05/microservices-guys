FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["HabitTracker.sln", "./"]
COPY ["src/NotificationService/NotificationService.Api/NotificationService.Api.csproj", "src/NotificationService/NotificationService.Api/"]
COPY ["src/NotificationService/NotificationService.Infrastructure/NotificationService.Infrastructure.csproj", "src/NotificationService/NotificationService.Infrastructure/"]
COPY ["src/CoreService/CoreService.Contracts/CoreService.Contracts.csproj", "src/CoreService/CoreService.Contracts/"]

RUN dotnet restore "src/NotificationService/NotificationService.Api/NotificationService.Api.csproj"

COPY . .
WORKDIR "/src/src/NotificationService/NotificationService.Api"
RUN dotnet publish "NotificationService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]