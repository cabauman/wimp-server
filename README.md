<p align="center" style="background: rgb(36,36,36);
background: radial-gradient(circle, rgba(6,6,6,1) 14%, rgba(11,11,11,1) 17%, rgba(255,255,255,0) 23%);">
  <a href="" rel="noopener">
 <img width=200px height=200px src="https://images.evetech.net/alliances/99010468/logo" alt="WOMP Logo"></a>
</p>

<h3 align="center">WIMP Server</h3>

<div align="center">

  [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=agelito_wimp-server&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=agelito_wimp-server)
  [![License](https://img.shields.io/github/license/agelito/wimp-server)](/LICENSE)

</div>

---

<p align="center"> Server for processing, storing, and providing EVE Online intel.
    <br> 
</p>

## 📝 Table of Contents
- [About](#about)
- [Getting Started](#getting_started)
- [Usage](#usage)
- [Deployment](#deployment)
- [Built Using](#built_using)
- [Authors](#authors)
- [Acknowledgments](#acknowledgement)

## 🧐 About <a name = "about"></a>
This server is part of a tool called WIMP (WOMP Intel Management Program). The purpose of this server is to process and store intel coming from in-game chat channels. The server provides an API for submitting and retrieving intel information.

## 🏁 Getting Started <a name = "getting_started"></a>
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
Install the following:
* [Git](https://git-scm.com/downloads). A distributed version control system.
* [Visual Studio Code](https://code.visualstudio.com/). Open source code editor.
* [NET Core SDK](https://dotnet.microsoft.com/download). The SDK also includes the Runtime.
* The [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) from the VS Code Marketplace.

After installing the prerequisites you can verify the installed dotnet version by typing this command in the terminal:
```sh
dotnet --version
```

### Installing
Follow these steps to get started working with the project:

Clone the repository:
```sh
git clone git@github.com:agelito/wimp-server.git
```

Open the cloned project with Visual Studio Code:

```sh
code wimp-server
```

Open the terminal in Visual Studio Code:

```
ctrl+shift+`
```

Install any missing NuGet packages:
```sh
dotnet restore
```

**Generating development database migrations**

Database migration files have to be generated the first time using a new database or with an existing database when the data model was changed.


Use the following commands to create database migrations for a fresh database:
```sh
cd WIMP-Server
dotnet ef migrations add InitialCreate --context WimpDbContextDev --output-dir Migrations/Development
cd ..
```

Use the following commands to create database migrations for an existing database:

- _Remember to change `<name>` to something descriptive of the changes made to the data model_

- _This only needs to be done if there was any changes to the data model_
```sh
cd WIMP-Server
dotnet ef migrations add <name> --context WimpDbContextDev --output-dir Migrations/Development
cd ..
```

Build and run the application, because there's two projects we'll have to specify the `.csproj` file as well:
```sh
dotnet run --project WIMP-Server/WIMP-Server.csproj
```

The application should now run with default configuration options. See [usage](#usage) section for information about how to configure the application. 

## 🎈 Usage <a name="usage"></a>

### Running the server <a name="running"></a>
Run the application in development environment using `dotnet run` command:
```sh
dotnet run --project WIMP-Server/WIMP-Server.csproj
```

The server will populate the database first time it's run, be patient since this could take some time.

### Configuring the server <a name="configuration"></a>
The server is using the `appsettings.json` file for configuration. It's also possible to have different configurations for different environments by naming the file `appsettings.Development.json` or `appsettings.Production.json`:
```sh
# Default configuration file
WIMP-Server/appsettings.json

# Developmment configuration file
WIMP-Server/appsettings.Development.json

# Production configuration file
WIMP-Server/appsettings.Production.json
```

The following configuration options is available:
Key | Description | Example
---|---|---
EsiService | The Eve ESI server endpoint to use. | https://esi.evetech.net
ConnectionStrings.WimpDatabase | The Wimp Database connection string. | Server=localhost,1433;Initial Catalog=wimpinteldb;User ID=sa;Password=yourStrong(!)Password;

***appsettings.Development.json Example***
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "EsiService": "https://esi.evetech.net"
}
```

## 🚀 Deployment <a name="deployment"></a>

This section describes how to set up and run WIMP-Server in a production-like environment.

### Docker

**Start SQL Server 2019 Instance**

- _Replace `yourStrong(!)Password` with your own secure password and do not share the password with anyone_
- _Take a note of the selected password stored in `SA_PASSWORD` because it will be used when configuring the server later on_
```sh
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_PID=Express" -e "SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
```

**Retrieve information about the running database container**

Use this command to check the status, name, and ID of the database container:
```sh
docker container ls
```
- _Take note of the `CONTAINER ID` or `Names` field since it will be needed in next command_

Output will look similar to this:

CONTAINER ID | IMAGE | COMMAND | CREATED | STATUS | PORTS | NAMES
---|---|---|---|---|---|---
63bc5205a067 | mcr.microsoft.com/mssql/server:2019-latest | "/opt/mssql/bin/perm…" | 14 hours ago | Up 25 seconds | 0.0.0.0:1433->1433/tcp | vigorous_fermi


Check which internal IP address the database docker container is assigned to, use the `CONTAINER ID` or `Names` field from previous command:
```sh
docker container inspect 63bc5205a067 | grep "IPAddress"
```
- _Take note of the `IPAddress` since it will be used in the next step to configure the database connection string_

The output will be similar to this:
```
            "SecondaryIPAddresses": null,
            "IPAddress": "172.17.0.2",
                    "IPAddress": "172.17.0.2",
```
**Running prebuilt WIMP-Server docker image**

Run the prebuilt WIMP-Server docker image using this command:

Replace the `{IPAddress}` with the `IPAddress` from previous step and `{SA_PASSWORD}` with `SA_PASSWORD`.

```sh
docker run \
  -e 'ConnectionStrings:WimpDatabase=Server={IPAddress},1433;Initial Catalog=wimpinteldb;User ID=sa;Password={SA_PASSWORD};' \
  -d -p 8080:80 ghcr.io/agelito/wimp-server:latest
```

**Building and running docker image from source**

Configuring the database connection string:

From the previous steps you should remember or have notes of the `SA_PASSWORD` and `IPAddress` values. Now we'll use those values to configure the database connection string.

Open the `appsettings.Production.json` file and modify the `ConnectionStrings.WimpDatabase` field. Replace the `{IPAddress}` with the `IPAddress` from previous step and `{SA_PASSWORD}` with `SA_PASSWORD`.
```json
"WimpDatabase": "Server={IPAddress},1433;Initial Catalog=wimpinteldb;User ID=sa;Password={SA_PASSWORD};"
```

Create production database migrations, this will generate files for setting up the required database tables and data model:
```sh
cd WIMP-Server
rm -rf Migrations/Production # Execute this line if using a fresh database to remove any old migrations
ASPNETCORE_ENVIRONMENT=Production dotnet ef migrations add InitialCreate --context WimpDbContext --output-dir Migrations/Production
```

Now we're ready to build the docker image for WIMP-Server, use the following command to create a docker image named `wimpserver`:

- _Make sure the current working directory is in the root of the repository before doing this step_
```sh
docker build -t wimpserver .
```

Run the built docker image using this command:
```sh
docker run -d -p 8080:80 wimpserver
```

Verify the server was started correctly by trying to call one of the functions:
- _The server may take a few minutes to populate the database with initial data on first start_
```sh
curl http://localhost:8080/universe/30001192/1
```

The previous command should return systems and edges within `1` jump of `30001192`:
```json
{"systems":[{"systemId":30001192,"systemName":"GJ0-OJ"},{"systemId":30001190,"systemName":"JWZ2-V"},{"systemId":30001193,"systemName":"A-803L"},{"systemId":30001196,"systemName":"Q-S7ZD"}],"edges":[{"sourceSystemId":30001192,"destinationSystemId":30001190},{"sourceSystemId":30001192,"destinationSystemId":30001193},{"sourceSystemId":30001192,"destinationSystemId":30001196}]}
```

## ⛏️ Built Using <a name = "built_using"></a>
- [.Net Core](https://docs.microsoft.com/en-us/dotnet/core/introduction) - Development Platform

## ✍️ Authors <a name = "authors"></a>
- [@agelito](https://github.com/agelito) - Idea & Initial work

See also the list of [contributors](https://github.com/agelito/wimp-server/contributors) who participated in this project.

## 🎉 Acknowledgements <a name = "acknowledgement"></a>
- [EVE Online](https://www.eveonline.com/) - The fantastic game this tool is used with.
- [WOMP](https://evewho.com/alliance/99010468) - The great corp I'm part of in EVE Online.