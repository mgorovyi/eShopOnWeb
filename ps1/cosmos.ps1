$groupname="vivethere_rg122700"
$grouplocation="eastus"

#az login
az group create --name $groupname --location $grouplocation

#create cosmosdb account 
$cosmosdbAccount="vivetherecosmosaccount0"
az cosmosdb create --name $cosmosdbAccount --resource-group $groupname `
             --capabilities EnableServerless `
             --default-consistency-level Session

#create sql database
$databaseName="DeliveryService"
$containerName="Orders"

az cosmosdb sql database create --name $databaseName `
    --resource-group $groupname `
    --account-name $cosmosdbAccount

#create container
$partitionKey='/id'


az cosmosdb sql container create --account-name $cosmosdbAccount --resource-group $groupname `
    --database-name $databaseName `
    --name $containerName `
    --partition-key-path $partitionKey

$cosmosdbConnectionsString = $(az cosmosdb keys list -g $groupname -n $cosmosdbAccount --type connection-strings -o tsv --query "connectionStrings[?description=='Primary SQL Connection String'].connectionString")
echo $cosmosdbConnectionsString