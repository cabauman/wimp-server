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
- [Deployment](#deployment)
- [Usage](#usage)
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
```
dotnet --version
```

### Installing
Follow these steps to get started working with the project:

Clone the repository:
```
git clone git@github.com:agelito/wimp-server.git
```

Open the cloned project with Visual Studio Code:

```
code wimp-server
```

Open the terminal in Visual Studio Code:

```
ctrl+shift+`
```

Install any missing NuGet packages:
```
dotnet restore
```

Build and run the application, because there's two projects we'll have to specify the `.csproj` file as well:
```
dotnet run --project WIMP-Server/WIMP-Server.csproj
```

The application should now run with default configuration options. See [usage](#usage) section for information about how to configure the application. 

## 🎈 Usage <a name="usage"></a>

### Running the server <a name="running"></a>
Run the application in development environment using `dotnet run` command:
```
dotnet run --project WIMP-Server/WIMP-Server.csproj
```

The server will populate the database first time it's run, be patient since this could take some time.

### Configuring the server <a name="configuration"></a>
The server is using the `appsettings.json` file for configuration. It's also possible to have different configurations for different environments by naming the file `appsettings.Development.json` or `appsettings.Production.json`:
```
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

***appsettings.Development.json Example***
```
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

## ⛏️ Built Using <a name = "built_using"></a>
- [.Net Core](https://docs.microsoft.com/en-us/dotnet/core/introduction) - Development Platform

## ✍️ Authors <a name = "authors"></a>
- [@agelito](https://github.com/agelito) - Idea & Initial work

See also the list of [contributors](https://github.com/agelito/wimp-intellog/contributors) who participated in this project.

## 🎉 Acknowledgements <a name = "acknowledgement"></a>
- [EVE Online](https://www.eveonline.com/) - The fantastic game this tool is used with.
- [WOMP](https://evewho.com/alliance/99010468) - The great corp I'm part of in EVE Online.