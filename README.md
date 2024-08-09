# Playing with Azure Event Hubs

This repository contains a project for sending and processing events.

## Prerequisites

- You can find a [CreateEventHubs.ps1](Scripts/CreateEventHubs.ps1) script file, that will manage all creation process on Azure
- Create an *Event Hubs Namespace* and an *Event Hub*
- In the newly created Hub instance, open the Shared access policies and create a new policy with Send and Listen
- Copy the PrimaryKey ConnectionString to configure appsettings.json
- Run locally the Azurite storage emulator (Scripts folder: [AzuriteStart.cmd](Scripts/AzuriteStart.cmd))

```json
"ConnectionStrings": {
    "EventHubs": "Use the PrimaryKey ConnectionString from the policy you created",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true"
}
```

## Resources

- [Documentation](https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-about) 📚*MS-Learn*| [Pricing](https://azure.microsoft.com/en-us/pricing/details/event-hubs)
- [Client library](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.eventhubs-readme) 📚*MS-Learn* | [EventHubs-samples](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/eventhub/Azure.Messaging.EventHubs) 👤*Azure*
- [Processor-client library](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.eventhubs.processor-readme) 📚*MS-Learn* | [EventHubs.Processor-samples](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/eventhub/Azure.Messaging.EventHubs.Processor) 👤*Azure*
- [Azure CLI commands](https://learn.microsoft.com/en-us/cli/azure/eventhubs?view=azure-cli-latest) 📚*MS-Learn*
