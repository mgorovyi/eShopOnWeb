$alias="vadtaras1220"

#Resourcse group name
$groupname=$alias+"_rg"
$grouplocation="eastus"

$locationapi="eastus"
$plannameapi ="plannameapi"
$appserviceapiname = $alias+"publicapi"


#az login
az group create --name $groupname --location $grouplocation

#appservice plan and service app for Public API
az appservice plan create --name $plannameapi --sku "S1" --resource-group $groupname --location $locationapi
az webapp create --name $appserviceapiname --resource-group $groupname --plan $plannameapi --runtime "DOTNET:6.0"  --assign-identity "[system]"
az webapp config appsettings set --name $appserviceapiname -g $groupname --settings ASPNETCORE_ENVIRONMENT=Development


