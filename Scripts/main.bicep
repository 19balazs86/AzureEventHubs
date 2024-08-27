param hubNamespaceName string
param hubName string

var rgLocation = resourceGroup().location

var uniqueSuffix = uniqueString(resourceGroup().id, 'just-text')

var _hubNamespaceName = '${hubNamespaceName}-${uniqueSuffix}'
var _hubName = '${hubName}-${uniqueSuffix}'

// --> EventHub / Namespace
// https://learn.microsoft.com/en-us/azure/templates/microsoft.eventhub/namespaces

resource eventHubNamespace 'Microsoft.EventHub/namespaces@2024-01-01' = {
  name: _hubNamespaceName
  location: rgLocation
  sku: {
    name: 'Basic'
  }
}

// --> EventHub / Namespace / Eventhub
// https://learn.microsoft.com/en-us/azure/templates/microsoft.eventhub/namespaces/eventhubs

resource eventHub 'Microsoft.EventHub/namespaces/eventhubs@2024-01-01' = {
  name: _hubName
  parent: eventHubNamespace
  properties: {
    partitionCount: 4
    retentionDescription: {
      cleanupPolicy: 'Delete'
      retentionTimeInHours: 1
    }
  }
}

// --> EventHub / Namespace / Eventhub / AuthorizationRule
// https://learn.microsoft.com/en-us/azure/templates/microsoft.eventhub/namespaces/eventhubs/authorizationrules


resource authorizationRule 'Microsoft.EventHub/namespaces/eventhubs/authorizationRules@2024-01-01' = {
  name: 'SendListen'
  parent: eventHub
  properties: {
    rights: ['Send', 'Listen']
  }
}

#disable-next-line outputs-should-not-contain-secrets
output url string = authorizationRule.listKeys().primaryConnectionString