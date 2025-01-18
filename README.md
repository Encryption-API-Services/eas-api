# eas-api

[![image](https://img.shields.io/badge/Discord-5865F2?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/7bXXCQj45q)

# Environment Description
This is a .NET 8.0 Web API. Authentication is hand written using JWT Tokens which are signed with an individual ECC 521 key pair and the storage mechanism is a MongoDB database to allow for easy schema changes during development. 
This project contains the front-end routes for our [SPA interface](https://github.com/Encryption-API-Services/AngularSPA). The website allows user's to make an account and verify it to obtain access to a API key for development or production.
The API key is pluggable into the C# SDK in the following way, when consuming the SDK benchmarks are sent to the main website and users can view their benchmarks in a chart.
```csharp
using CasDotnetSdk;

CASConfiguration.IsDevelopment = true;
CASConfiguration.ApiKey = "FV0mg+D/1JXh9oX07nQGJld+1jLQ6vkve4ZTpzhg2cRGuSt4JgTXfcLznHcdqAN+jp8YBz/0xgR1xJ3mQhtuBQ==";
```

We use [Infisical](https://infisical.com/) for secret management, project isn't going to work correctly without them. Get ahold of me to figure something out regarding this.
## SDKs Currently In Development
[C#](https://github.com/Cryptographic-API-Services/cas-dotnet-sdk)
[JavaScript](https://github.com/Cryptographic-API-Services/cas-typescript-sdk)
[Python](https://github.com/Cryptographic-API-Services/cas-python-sdk)
