
cls

$alias = "vivethere017"
$groupname = $alias+"rg"
$sb_namespace=$alias+"sb"
$queueName="ordercompleted"
$grouplocation="eastus"

az group create --name $groupname --location $grouplocation

az servicebus namespace create --name $sb_namespace --resource-group $groupname --location $grouplocation --sku Standard

#az servicebus namespace authorization-rule create --resource-group $groupname --namespace-name $sb_namespace --name RootManageSharedAccessKey
#az servicebus namespace authorization-rule create --resource-group $groupname --namespace-name $sb_namespace --name listenPolicy --rights Listen

#get custom listen policy. it must be created before
$serviceBusConnString = $( az servicebus namespace authorization-rule keys list --namespace-name $sb_namespace --name RootManageSharedAccessKey -g $groupname --query primaryConnectionString -o tsv)

az servicebus queue create --name $queueName --namespace-name $sb_namespace --resource-group $groupname --enable-partitioning true


$faStorageName=$alias+"fasacc"

az storage account create `
  --name $faStorageName `
  --location $grouplocation `
  --resource-group $groupname `
  --sku Standard_LRS

$orderStorageName=$alias+"ordersa"

az storage account create `
  --name $orderStorageName `
  --location $grouplocation `
  --resource-group $groupname `
  --sku Standard_LRS

az storage container create --name orders --account-name $orderStorageName -g $groupname

$warehouseFuncName=$alias+"whfa"

az functionapp create `
    --name $warehouseFuncName `
    --resource-group $groupname `
    --storage-account $faStorageName `
    --consumption-plan-location $grouplocation `
    --disable-app-insights true `
    --assign-identity "[system]"

az functionapp config appsettings set -n $warehouseFuncName -g $groupname --settings ServiceBusConnectionString=$serviceBusConnString
az functionapp config appsettings set -n $warehouseFuncName -g $groupname `
    --settings WarehouseStorageConnectionString=$(az storage account show-connection-string --name $orderStorageName -g $groupname -o tsv)

