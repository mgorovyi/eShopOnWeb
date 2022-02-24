$groupname="vivethere_rg4"
$grouplocation="eastus"

$traficmanager_profile_name="eShopWebTrafficManager"
$traficmanager_unique_dnsname = "eshopwebvivethere"

$webapp_repourl="https://github.com/vive-there/eShopOnWeb/tree/main/src/Web"

$planname1="appserviceplan1"
$webappname1="eshopwebvivethere1"
$locationweb1="eastus"

$planname2="appserviceplan2"
$webappname2="eshopwebvivethere2"
$locationweb2="westeurope"

$deployment_slot_name="staging"

$locationapi="eastus"
$plannameapi ="plannameapi"
$appserviceapiname = "vivetherepublicapi"
$d = Get-Date
$apimanagementname="vivethereapimnginstance"+$d.Year+$d.Month+$d.Day+$d.Hour+$d.Minute+$d.Second+$d.Millisecond
$publicapiendpoint="https://"+$apimanagementname+".azure-api.net/publicapi/api/"
$webappendpoint="http://"+$traficmanager_unique_dnsname+".trafficmanager.net/"
echo $publicapiendpoint
echo $webappendpoint

$apimngpath="publicapi"

#az login
az group create --name $groupname --location $grouplocation

#Create appservice plan 1
az appservice plan create --name $planname1 --resource-group $groupname --location $locationweb1 --sku S1

#create webapp
az webapp create --name $webappname1 --resource-group $groupname --plan $planname1 --runtime "DOTNET:6.0" 
#get webapp ID
$webapp1_ID = $(az webapp show --name $webappname1 -g $groupname --query "id" -o json)

#create slot
az webapp deployment slot create --name $webappname1 -g $groupname --slot $deployment_slot_name

#create deployment
#az webapp deployment source config --name $webappname1 --resource-group $groupname --repo-url $webapp_repourl --branch main --manual-integration --slot $deployment_slot_name --repository-type github;
az webapp config appsettings set --name $webappname1 -g $groupname --settings baseUrls:webBase=$webappendpoint
az webapp config appsettings set --name $webappname1 -g $groupname --settings baseUrls:apiBase=$publicapiendpoint
az webapp config appsettings set --name $webappname1 -g $groupname --settings baseUrls:webBase=$webappendpoint --slot $deployment_slot_name
az webapp config appsettings set --name $webappname1 -g $groupname --settings baseUrls:apiBase=$publicapiendpoint --slot $deployment_slot_name

#Create appservice plan 2
az appservice plan create --name $planname2 --resource-group $groupname --location $locationweb2 --sku S1

#create webapp
az webapp create --name $webappname2 --resource-group $groupname --plan $planname2 --runtime "DOTNET:6.0" 
#get webapp ID
$webapp2_ID = $(az webapp show --name $webappname2 -g $groupname --query "id" -o json)

#create slot
az webapp deployment slot create --name $webappname2 -g $groupname --slot $deployment_slot_name

az webapp config appsettings set --name $webappname2 -g $groupname --settings baseUrls:webBase=$webappendpoint
az webapp config appsettings set --name $webappname2 -g $groupname --settings baseUrls:apiBase=$publicapiendpoint
az webapp config appsettings set --name $webappname2 -g $groupname --settings baseUrls:webBase=$webappendpoint --slot $deployment_slot_name
az webapp config appsettings set --name $webappname2 -g $groupname --settings baseUrls:apiBase=$publicapiendpoint --slot $deployment_slot_name

#Traffic manager profile
az network traffic-manager profile create --name $traficmanager_profile_name --resource-group $groupname --routing-method Geographic --unique-dns-name $traficmanager_unique_dnsname

#add traffic manager endpoints
az network traffic-manager endpoint create --name "eshopeastusendpoint" --profile $traficmanager_profile_name -g $groupname --type azureEndpoints --geo-mapping GEO-NA --target-resource-id $webapp1_ID --endpoint-status enabled
az network traffic-manager endpoint create --name "eshopwesteuropeendpoint" --profile $traficmanager_profile_name -g $groupname --type azureEndpoints --geo-mapping GEO-EU --target-resource-id $webapp2_ID --endpoint-status enabled

#appservice plan and service app for API
az appservice plan create --name $plannameapi --sku "S1" --resource-group $groupname --location $locationapi
az webapp create --name $appserviceapiname --resource-group $groupname --plan $plannameapi --runtime "DOTNET:6.0"
az webapp config appsettings set --name $appserviceapiname -g $groupname --settings baseUrls:webBase=$webappendpoint
az webapp config appsettings set --name $appserviceapiname -g $groupname --settings baseUrls:apiBase=$publicapiendpoint
az webapp cors add -g $groupname -n $appserviceapiname --allowed-origins $webappendpoint

#create api management instance
az apim create --name $apimanagementname --resource-group $groupname -l $locationapi --publisher-email "vadym.tarasov@gmail.com" --publisher-name  "Pupkin Inc" --sku-name "Consumption"
az apim api create --service-name $apimanagementname -g $groupname --api-id "vivetherepublicapi" --display-name "vivetherepublicapi" --path $apimngpath


