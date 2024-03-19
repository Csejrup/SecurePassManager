FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SecurePassManager/SecurePassManager.csproj", "SecurePassManager/"]
RUN dotnet restore "SecurePassManager/SecurePassManager.csproj"
COPY . .
WORKDIR "/src/SecurePassManager"
RUN dotnet build "SecurePassManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SecurePassManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecurePassManager.dll"]
