FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /

COPY Directory.Build.props /
COPY Directory.Build.targets /
COPY Host/Host.csproj Host/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Core/Domain/Domain.csproj Core/Domain/
COPY Core/Application/Application.csproj Core/Application/
COPY Core/Shared/Shared.csproj Core/Shared/
COPY Migrators/Migrators.MsSql/Migrators.MsSql.csproj Migrators/Migrators.MsSql/

RUN dotnet restore Host/Host.csproj --disable-parallel

COPY . .
WORKDIR /Host
RUN dotnet build "Host.csproj" -c Relase -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /app/publish .

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENV ASPNETCORE_URLS=https://+:5005;http://+:5006
EXPOSE 5005
EXPOSE 5006
ENTRYPOINT ["dotnet", "CodeMatrix.Mepd.Host.dll"]