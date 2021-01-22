# Redox.DataModel.Builder

## What is Redox?
From their documentation, [Redox](https://www.redoxengine.com/product/) is an EHR Integration API that connects custom products to healthcare organizations while eliminating infrastructure requirements, data transformation tasks, and security overhead of bespoke integration methods. 
It’s healthcare integration as a service presented as a single, standard, secure API endpoint. One connection to [Redox](https://www.redoxengine.com/product/) speeds customer acquisition and increases developer productivity.

## Let’s start
For a testing purpose we need to create Data models which are available only as JSON Schema files and can be downloaded [here](https://developer.redoxengine.com/data-models/schemas.zip).

This simple console application downloads ZIP archive, decompress it in memory and create .NET Data models in a same folder structure as it is in ZIP archive.

All you have to do is set **DataModelFolder** location in **appsettings.json** file :
```json
{
  "AppConfig": {
    "RedoxDataModelJsonSchemaFileUri": "https://developer.redoxengine.com/data-models/schemas.zip",
    "DataModelFolder": "C:\\Temp\\DataModels"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```
## Prerequisites
- [Visual Studio](https://www.visualstudio.com/vs/community) 2019 16.8.1 or greater 

## Tags & Technologies
- [Redox Engine](https://www.redoxengine.com/product/)
- [Redox Blog](https://www.redoxengine.com/blog/)
- [Redox Engine Developer Guide](https://developer.redoxengine.com/)
- [NJsonSchema for .NET](https://github.com/RicoSuter/NJsonSchema) - JSON Schema reader, generator and validator for .NET

For any questions/suggestions contact me on [LinkedIn](https://si.linkedin.com/in/matjazbravc).
