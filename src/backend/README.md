# MEPD

## Project Structure

This project consists of the following projects:

```bash
├── src
│   ├── Host.csproj
|   ├── Infrastructure.csproj
│   ├── Core
│   |   ├── Application.csproj
│   |   └── Domain.csproj
|   ├── Migrators
│   |   ├── Migrators.MSSQL.csproj
│   └── Shared
│       └── Shared.DTOs.csproj
```

### Host

Contains the API controllers and startup logic including ASP.net container setup.

### Application

This project contains abstract classes and interfaces of application elements

### Domain

This project includes application domain classes

### Infrastructure 

This project impliemnts application features and services

## Configurations

Within the Host project there is a folder named "Configurations". Where all the application configuration files stored, file per feature.

```bash
|-- Host.csproj
|  |-- Configurations
|  |    |-- cache.json
|  |    |-- cors.json
|  |    |-- database.json
|  |    |-- hangfire.json
|  |    |-- logger.json
|  |    |-- mail.json
|  |    |-- middleware.json
|  |    |-- monitoring.json
|  |    |-- security.json
|  |    |-- securityheaders.json
|  |    |-- signalr.json
|  |    |-- swagger.json
|  |-- appsettings.json

```

> The default `appsettings.json` file is avialble to manage other custom parameters.

## CORS

The application has support for providing APIs for different clients, all the configurations realted to this feature can be found under `Host/Configurations/cors.json`.

The configuration provide dynamic scenarios how Cross-Origin request by clients can be served. 

```json
{
  "CorsSettings": {
    "Url": "http://localhost:4500",
    "Clients": [
      {
        "Policy": "angular-client",
        "Url": "http://localhost:4501"
      },
      {
        "Policy": "react-client",
        "Url": "http://localhost:4502"
      }
    ]
  }
}
```

## Logging

The application has support for logging at several distations By using Serilog. All loging configuration can be found under `Host/Configurations/logger.json`.

### Elastic Search / Kiabana
```bash
docker-compose -f ./deployment/elk/docker-compose.elk.yml up
```

* Kiabana runs on port 5601 - http://localhost:5601
* Elastic Search runs on port 9200 - http://localhost:9200

You can see that we are pointing to port 9200 from our configuration. This instructs Serilog to write the logs to ElasticSearch DB.

To view the logs on Dashboard, navigate to http://localhost:5601

Firstly, navigate to Kibana Spaces - http://localhost:5601/app/management/kibana/spaces

From the sidebar, select Index Patterns and create a new one.

In the Name field - put in codematrix.mepd-logs* or whatever you have set in your Serilog Configuration under indexFormat. Select the Timestamp field as @timestamp. That’s it.

Now, go to http://localhost:5601/app/discover

### Console

Console logging is enabled by default. Its configuration is done in code under `Program.cs` using `Serilog.Sinks.Console`

### File

Structured JSON formatted logs stored on the local storage using package - Serilog.Sinks.File

```json
{
        "Args": {
          "path": "Logs/logs.json",
          "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog",
          "rollingInterval": "Day",
          "restrictedToMinimumLevel": "Information",
          "retainedFileCountLimit": 5
        },
        "Name": "File"
}
```

### SEQ

docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest


## Datbase context

To update Application Context you need to run following command from within Host project folder

```
dotnet ef migrations add feature-name --project ../Migrators/Migrators.MsSql/ --context ApplicationDbContext -o Migrations/Application
```

To update Root context you need to run below command from within Host project folder

```
dotnet ef migrations add feature-name --project ../Migrators/Migrators.MsSql/ --context TenantManagementDbContext -o Migrations/Root
```

## API Default Credentials

Request must be submited over to 

```json
{
    "email":"admin@root.com",
    "password":"123Pa$$word!"
}
```

## Getting Started with Docker

There are some prerequisites for using the included Dockerfile and docker-compose.yml files:

1. Make sure you have docker installed ([https://docs.docker.com](https://docs.docker.com/))
2. Setup SSL
   * Create and install an https certificate:

      ```shell
      dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\CodeMatrix.Mepd.Host.pfx -p SuperSecurePassword123!
      ```

   * It's possible that the above step gives you an A valid HTTPS certificate is already present. error. In that case you will have to run this first:

      ```shell
      dotnet dev-certs https --clean
      ```

   * Trust the certificate

    ```shell
    dotnet dev-certs https --trust
    ```

    **Note** - Make sure that you use the same password that has been configured in the docker-compose.yml file. By default, SuperSecurePassword123! is configured.

3. 5005 & 5006 are the ports setup to run webapi on Docker, so make sure that these ports are free. You could also change the ports in the docker-compose.yml and Host\Dockerfile files.

4. Now navigate back to the root of the Project on your local machine and run the following via terminal `docker-compose -f 'docker-compose.yml' up --build`
5. This will start pulling MSSQL Server Image from Docker Hub if you don't already have this image. It's around 500+ Mbs of download.
6. Once that is done, dotnet SDKs and runtimes are downloaded, if not present already. That's almost 200+ more Mbs of download.
7. PS If you find any issues while Docker installs the nuget packages, it is most likely that your ssl certificates are not installed properly. Apart from that I also added the `--disable-parallel` in the `Host\Dockerfile` to ensure network issues don't pop-up. You can remove this option to speed up the build process.
8. That's almost everything. Once the containers are available, migrations are updated in the MSSQL DB, default data is seeded.
9. The Api should be available at `https://localhost:5005/swagger`

## Data Encryption

### How to use

To use the data encryption, you will need to decoreate string properties on the entities with `[Encrypt]` attribute and enable encryption.

The encryption uses the default **encryption provider** that supports 128bits, 192bit or 256bits key, other provides can be created by implimenting `IEncryptionProvider` interface.

#### Example usage

```c#
public class PersonalEntity{
    public int Id { get; set; }

    [Encrypted]
    public string PersonalInfo { get; set; }

    [Encrypted]
    public string Passkey { get; set;}

    public string PublicInfo { get; set; }
}

```