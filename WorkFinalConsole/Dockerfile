FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WorkFinalConsole/WorkFinalConsole.csproj", "WorkFinalConsole/"]
RUN dotnet restore "WorkFinalConsole/WorkFinalConsole.csproj"
COPY . .
WORKDIR "/src/WorkFinalConsole"
RUN dotnet build "WorkFinalConsole.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkFinalConsole.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkFinalConsole.dll"]
