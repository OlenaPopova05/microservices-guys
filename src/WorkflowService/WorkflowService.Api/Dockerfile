FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["HabitTracker.sln", "./"]
COPY ["src/WorkflowService/WorkflowService.Api/WorkflowService.Api.csproj", "src/WorkflowService/WorkflowService.Api/"]
COPY ["src/WorkflowService/WorkflowService.Application/WorkflowService.Application.csproj", "src/WorkflowService/WorkflowService.Application/"]
COPY ["src/WorkflowService/WorkflowService.Domain/WorkflowService.Domain.csproj", "src/WorkflowService/WorkflowService.Domain/"]
COPY ["src/WorkflowService/WorkflowService.Infrastructure/WorkflowService.Infrastructure.csproj", "src/WorkflowService/WorkflowService.Infrastructure/"]

RUN dotnet restore "src/WorkflowService/WorkflowService.Api/WorkflowService.Api.csproj"

COPY . .
RUN dotnet publish "src/WorkflowService/WorkflowService.Api/WorkflowService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WorkflowService.Api.dll"]
