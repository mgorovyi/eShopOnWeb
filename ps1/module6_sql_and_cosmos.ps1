$alias="vivethere"

$groupname=$alias+"_rg"
$grouplocation="eastus"

$traficmanager_profile_name="eShopWebTrafficManager"
$traficmanager_unique_dnsname = "eshopweb"+$alias

$planname1="appserviceplan1"
$webappname1="eshopweb"+$alias+"1"
$locationweb1="eastus"

$planname2="appserviceplan2"
$webappname2="eshopweb"+$alias+"2"
$locationweb2="westeurope"

$deployment_slot_name="staging"

$locationapi="eastus"
$plannameapi ="plannameapi"
$appserviceapiname = $alias+"publicapi"


$publicapiendpoint="https://"+$appserviceapiname+".azurewebsites.net/api/"
$webappendpoint="https://"+$traficmanager_unique_dnsname+".trafficmanager.net/"
$webappendpoint_origin="https://"+$traficmanager_unique_dnsname+".trafficmanager.net"

echo $publicapiendpoint
echo $webappendpoint



$sqlservername= $alias+"sqlserver1129"
$sqladminname= $alias+"admin"
$sqladminpwd="Gt4r!Y456"
$catalogdbname="eShopOnWeb.CatalogDb"
$identitydbname="eShopOnWeb.Identity"
$catalogConnection="Server=tcp:"+$sqlservername+".database.windows.net,1433;Initial Catalog="+$catalogdbname+";Persist Security Info=False;User ID="+$sqladminname+";Password="+$sqladminpwd+";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
#$identityConnection="Server=tcp:"+$sqlservername+".database.windows.net,1433;Initial Catalog="+$identitydbname+";Persist Security Info=False;User ID="+$sqladminname+";Password="+$sqladminpwd+";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
echo $catalogConnection

#az login
az group create --name $groupname --location $grouplocation

#create sql server
az sql server create -g $groupname -l $grouplocation -n $sqlservername --admin-user $sqladminname --admin-password $sqladminpwd
#Allow Azure Services
#https://docs.microsoft.com/en-us/azure/azure-sql/database/firewall-configure#manage-firewall-rules-using-azure-cli
az sql server firewall-rule create -g $groupname --server $sqlservername --name "AllowAzureServices" --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
az sql server firewall-rule create -g $groupname --server $sqlservername --name $alias+"Ip" --start-ip-address 81.17.143.21 --end-ip-address 81.17.143.21
# get all info  az sql db list-editions -a -o table -l LOCATION.
#disabled zone redundancy
az sql db create --name $catalogdbname --server $sqlservername -g $groupname --service-objective Free -z false
#Free tier only allows one free database per location
#I have commented this
#az sql db create --name $identitydbname --server $sqlservername -g $groupname --service-objective Free -z false



#Create appservice plan 1
az appservice plan create --name $planname1 --resource-group $groupname --location $locationweb1 --sku S1
#create webapp
az webapp create --name $webappname1 --resource-group $groupname --plan $planname1 --runtime "DOTNET:6.0" --assign-identity "[system]"
#get webapp ID
$webapp1_ID = $(az webapp show --name $webappname1 -g $groupname --query "id" -o json)

#create slot
az webapp deployment slot create --name $webappname1 -g $groupname --slot $deployment_slot_name

#create deployment
#az webapp deployment source config --name $webappname1 --resource-group $groupname --repo-url $webapp_repourl --branch main --manual-integration --slot $deployment_slot_name --repository-type github;
az webapp config appsettings set --name $webappname1 -g $groupname --settings baseUrls:webBase=$webappendpoint
az webapp config appsettings set --name $webappname1 -g $groupname --settings baseUrls:apiBase=$publicapiendpoint
az webapp config connection-string set --name $webappname1 --connection-string-type SQLAzure -g $groupname --settings CatalogConnection=$catalogConnection
az webapp config connection-string set --name $webappname1 --connection-string-type SQLAzure -g $groupname --settings IdentityConnection=$catalogConnection
az webapp config connection-string set --name $webappname1 --connection-string-type SQLAzure -g $groupname --settings CatalogConnection=$catalogConnection --slot $deployment_slot_name
az webapp config connection-string set --name $webappname1 --connection-string-type SQLAzure -g $groupname --settings IdentityConnection=$catalogConnection --slot $deployment_slot_name
az webapp config appsettings set --name $webappname1 -g $groupname --settings baseUrls:webBase=$webappendpoint --slot $deployment_slot_name
az webapp config appsettings set --name $webappname1 -g $groupname --settings baseUrls:apiBase=$publicapiendpoint --slot $deployment_slot_name

#Create appservice plan 2
az appservice plan create --name $planname2 --resource-group $groupname --location $locationweb2 --sku S1

#create webapp
az webapp create --name $webappname2 --resource-group $groupname --plan $planname2 --runtime "DOTNET:6.0" --assign-identity "[system]"
#get webapp ID
$webapp2_ID = $(az webapp show --name $webappname2 -g $groupname --query "id" -o json)

#create slot
az webapp deployment slot create --name $webappname2 -g $groupname --slot $deployment_slot_name

az webapp config appsettings set --name $webappname2 -g $groupname --settings baseUrls:webBase=$webappendpoint
az webapp config appsettings set --name $webappname2 -g $groupname --settings baseUrls:apiBase=$publicapiendpoint
az webapp config appsettings set --name $webappname2 -g $groupname --settings baseUrls:webBase=$webappendpoint --slot $deployment_slot_name
az webapp config appsettings set --name $webappname2 -g $groupname --settings baseUrls:apiBase=$publicapiendpoint --slot $deployment_slot_name
az webapp config connection-string set --name $webappname2 --connection-string-type SQLAzure -g $groupname --settings CatalogConnection=$catalogConnection
az webapp config connection-string set --name $webappname2 --connection-string-type SQLAzure -g $groupname --settings IdentityConnection=$catalogConnection
az webapp config connection-string set --name $webappname2 --connection-string-type SQLAzure -g $groupname --settings CatalogConnection=$catalogConnection --slot $deployment_slot_name
az webapp config connection-string set --name $webappname2 --connection-string-type SQLAzure -g $groupname --settings IdentityConnection=$catalogConnection --slot $deployment_slot_name

#Traffic manager profile
az network traffic-manager profile create --name $traficmanager_profile_name --resource-group $groupname --routing-method Geographic --unique-dns-name $traficmanager_unique_dnsname

#add traffic manager endpoints
az network traffic-manager endpoint create --name "eshopeastusendpoint" --profile $traficmanager_profile_name -g $groupname --type azureEndpoints --geo-mapping GEO-NA --target-resource-id $webapp1_ID --endpoint-status enabled
az network traffic-manager endpoint create --name "eshopwesteuropeendpoint" --profile $traficmanager_profile_name -g $groupname --type azureEndpoints --geo-mapping GEO-EU --target-resource-id $webapp2_ID --endpoint-status enabled

#appservice plan and service app for API
az appservice plan create --name $plannameapi --sku "S1" --resource-group $groupname --location $locationapi
az webapp create --name $appserviceapiname --resource-group $groupname --plan $plannameapi --runtime "DOTNET:6.0"  --assign-identity "[system]"
az webapp config appsettings set --name $appserviceapiname -g $groupname --settings baseUrls:webBase=$webappendpoint
az webapp config appsettings set --name $appserviceapiname -g $groupname --settings baseUrls:apiBase=$publicapiendpoint
az webapp config connection-string set --name $appserviceapiname --connection-string-type SQLAzure -g $groupname --settings CatalogConnection=$catalogConnection
az webapp config connection-string set --name $appserviceapiname --connection-string-type SQLAzure -g $groupname --settings IdentityConnection=$catalogConnection
az webapp cors add -g $groupname -n $appserviceapiname --allowed-origins $webappendpoint_origin

#swap slots demo
#az webapp deployment slot swap  -g $groupname -n $webappname1 --slot $deployment_slot_name --target-slot production

#create function


#create cosmosdb account 
$cosmosdbAccount= $alias+"cosmosaccount"
az cosmosdb create --name $cosmosdbAccount --resource-group $groupname `
             --capabilities EnableServerless `
             --default-consistency-level Session

#create sql database
$databaseName="eShop"
$containerName="DeliveryOrders"

az cosmosdb sql database create --name $databaseName `
    --resource-group $groupname `
    --account-name $cosmosdbAccount

#create container
$partitionKey='/id'


az cosmosdb sql container create --account-name $cosmosdbAccount --resource-group $groupname `
    --database-name $databaseName `
    --name $containerName `
    --partition-key-path $partitionKey

#get connection string and add to function configuration
$cosmosdbConnectionsString = $(az cosmosdb keys list -g $groupname -n $cosmosdbAccount --type connection-strings -o tsv --query "connectionStrings[?description=='Primary SQL Connection String'].connectionString")
echo $cosmosdbConnectionsString

$storageName=$alias+"storageacc"

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
#set cosmosdb connection string
az functionapp config appsettings set -n $functionappname -g $groupname --settings CosmosDBConnection=$cosmosdbConnectionsString

$defaultHost=$(az functionapp show -n $functionappname -g $groupname --query defaultHostName -o tsv)
$funcUrl = "https://"+$defaultHost+"/api/"
echo $funcUrl
$funcMasteer=$(az functionapp keys list -g $groupname -n $functionappname --query masterKey -o tsv)


az webapp config appsettings set --name $webappname1 -g $groupname --settings DeliverySettings:DeliveryBaseApiUrl=$funcUrl 
az webapp config appsettings set --name $webappname1 -g $groupname --settings DeliverySettings:DeliveryBaseApiUrl=$funcUrl --slot $deployment_slot_name
az webapp config appsettings set --name $webappname1 -g $groupname --settings DeliverySettings:AuthKey=$funcMasteer 
az webapp config appsettings set --name $webappname1 -g $groupname --settings DeliverySettings:AuthKey=$funcMasteer --slot $deployment_slot_name

az webapp config appsettings set --name $webappname2 -g $groupname --settings DeliverySettings:DeliveryBaseApiUrl=$funcUrl 
az webapp config appsettings set --name $webappname2 -g $groupname --settings DeliverySettings:DeliveryBaseApiUrl=$funcUrl --slot $deployment_slot_name
az webapp config appsettings set --name $webappname2 -g $groupname --settings DeliverySettings:AuthKey=$funcMasteer 
az webapp config appsettings set --name $webappname2 -g $groupname --settings DeliverySettings:AuthKey=$funcMasteer --slot $deployment_slot_name




