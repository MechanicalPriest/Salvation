## API for Mechanical Priest Toolkit

The API project for MPT is written in C# as individual Azure Functions. These functions take in HTTP triggers with an optional payload and return and HTTP result. 

To run with Visual Studio Community 2019:

1. Load the entire solution in Visual Studio
2. Create a `local.settings.json` file in the project root with the following. This uses the local development storage and disables CORS for testing purposes.

```json
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "Host": {
    "CORS": "*"
  }
}
```

3. Right click the Salvation.Api project and choose **Debug > Start New Instance**. You should see output similar to below:

```
Azure Functions Core Tools (3.0.2912 Commit hash: bfcbbe48ed6fdacdf9b309261ecc8093df3b83f2)
Function Runtime Version: 3.0.14287.0
Hosting environment: Production
Content root path: F:\Projects\salvation\Application\Salvation.Api\bin\Debug\netcoreapp3.1
Now listening on: http://0.0.0.0:7071
Application started. Press Ctrl+C to shut down.

Functions:

        DefaultProfile: [GET] http://localhost:7071/api/DefaultProfile

        ProcessModel: [POST] http://localhost:7071/api/ProcessModel
```

Note: This can also be run outside of Visual Studio using the Azure Functions Tools `func.exe` against the location of the built Salvation.Api folder.

```
func start
```

### External References
- [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview)
- [Azure Static Web Apps](https://docs.microsoft.com/en-us/azure/static-web-apps/overview)