$alias="vivethere"

$groupname=$alias+"_rg112920"
$grouplocation="eastus"


#az login
az group create --name $groupname --location $grouplocation

$storageName=$alias+"storagesa11"

az storage account create `
  --name $storageName `
  --location $grouplocation `
  --resource-group $groupname `
  --sku Standard_LRS

$functionappname=$alias+"deliveryfunctionapp"

az functionapp create `
    --name $functionappname `
    --resource-group $groupname `
    --storage-account $storageName `
    --consumption-plan-location $grouplocation `
    --disable-app-insights true
    #--runtime "dotnet-isolated" --runtime-version "6.0"

$defaultHost=$(az functionapp show -n $functionappname -g $groupname --query defaultHostName -o tsv)
$funcUrl = "https://"+$defaultHost+"/api/"
