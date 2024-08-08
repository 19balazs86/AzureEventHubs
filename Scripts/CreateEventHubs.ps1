param(
  [string]
  # [Parameter(Mandatory=$true)]
  $resGroupName = "rg-sand-box",

  [string] $location = "North Europe",

  [string] $hubNamespaceName = "eh-balazs-namespace-test",

  [string] $hubName = "test-balazs-hub",

  [string] $sku = "Basic"
)

$response = az group exists --name $resGroupName

if ("false".Equals($response))
{
    Write-Host "Creating Resource-Group: " $resGroupName

    az group create --name $resGroupName --location $location
}

$jsonResponse = az eventhubs namespace exists --name $hubNamespaceName | ConvertFrom-Json

if ($jsonResponse.nameAvailable)
{
    Write-Host "Creating Namespace: " $hubNamespaceName

    az eventhubs namespace create --name $hubNamespaceName --resource-group $resGroupName --sku $sku --location $location
}

$hubList = az eventhubs eventhub list --namespace-name $hubNamespaceName --resource-group $resGroupName | ConvertFrom-Json

if (!($hubList -contains $hubName))
{
    Write-Host "Creating EventHub: " $hubName

    az eventhubs eventhub create `
        --name $hubName `
        --namespace-name $hubNamespaceName `
        --resource-group $resGroupName `
        --partition-count 4 `
        --cleanup-policy Delete `
        --retention-time-in-hours 1

    az eventhubs eventhub authorization-rule create  `
        --resource-group $resGroupName `
        --namespace-name $hubNamespaceName `
        --eventhub-name $hubName `
        --name SendListen `
        --rights Send Listen

    $response = az eventhubs eventhub authorization-rule keys list `
        --authorization-rule-name SendListen `
        --eventhub-name $hubName `
        --namespace-name $hubNamespaceName `
        --resource-group $resGroupName `
        --query "primaryConnectionString"

    Write-Host "Connection string: " $response
}