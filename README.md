# eas-api

![GitHub issues](https://img.shields.io/github/issues/Encryption-Api-Services/eas-api)
![GitHub](https://img.shields.io/github/license/Encryption-Api-Services/eas-api)

# Environment Description
This is a .NET 8.0 Web API. Authentication is hand written using JWT Tokens which are signed with an individual ECC 521 key pair and the storage mechanism is a MongoDB database to allow for easy schema changes during development. This project contains the front-end routes for our [SPA interface](https://github.com/Encryption-API-Services/AngularSPA). Once you are registered for the site, you gain access to the API routes for certain encryption methods. 

# SDKs Currently In Development
[C#](https://github.com/Encryption-API-Services/eas-dotnet)
[Deno](https://github.com/Encryption-API-Services/eas-deno)
[JavaScript](https://github.com/Encryption-API-Services/eas-javascript)

# Environment Setup
If for some reason your installation of Visual Studio didn't install the correct SDK or you are using VSCode, you will need to have the .NET Core [SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) installed.

There are 9 environment variables that need to be define on the machine you are developing on for this project to work properly. 
  - Connection
  - Database
  - UserCollectionName
  - Email (for SMTP)
  - TWILIO_ACCOUNT_SID
  - TWILIO_AUTH_TOKEN
  - IpInfoToken
  - StripApiKey
  - Domain
