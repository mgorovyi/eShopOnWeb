$groupname="vivetheremod2_rg"
$grouplocation="eastus"

$webapp_repourl="https://github.com/vive-there/eShopOnWeb/tree/main/src/Web"

$locationapi="eastus"
$plannameapi ="plannameapi"
$appserviceapiname = "vivetherepublicapi"

$sqlservername="vivetheresqlserver1129"
$sqladminname="vivethereadmin"
$sqladminpwd="Gt4r!Y456"
$catalogdbname="eShopOnWeb.CatalogDb"
$identitydbname="eShopOnWeb.Identity"
$catalogConnection="Server=tcp:"+$sqlservername+".database.windows.net,1433;Initial Catalog="+$catalogdbname+";Persist Security Info=False;User ID="+$sqladminname+";Password="+$sqladminpwd+";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

#az login
az group create --name $groupname --location $grouplocation

#create sql server
az sql server create -g $groupname -l $grouplocation -n $sqlservername --admin-user $sqladminname --admin-password $sqladminpwd
#Allow Azure Services
#https://docs.microsoft.com/en-us/azure/azure-sql/database/firewall-configure#manage-firewall-rules-using-azure-cli
az sql server firewall-rule create -g $groupname --server $sqlservername --name "AllowAzureServices" --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
az sql server firewall-rule create -g $groupname --server $sqlservername --name "VivethereIp" --start-ip-address 81.17.143.21 --end-ip-address 81.17.143.21
# get all info  az sql db list-editions -a -o table -l LOCATION.
#disabled zone redundancy
az sql db create --name $catalogdbname --server $sqlservername -g $groupname --service-objective Free -z false
#Free tier only allows one free database per location
#I have commented this
#az sql db create --name $identitydbname --server $sqlservername -g $groupname --service-objective Free -z false

#appservice plan and service app for API
az appservice plan create --name $plannameapi --sku "S1" --resource-group $groupname --location $locationapi
az webapp create --name $appserviceapiname --resource-group $groupname --plan $plannameapi --runtime "DOTNET:6.0"  --assign-identity "[system]"
az webapp config appsettings set --name $appserviceapiname -g $groupname --settings baseUrls:webBase=$webappendpoint
az webapp config appsettings set --name $appserviceapiname -g $groupname --settings baseUrls:apiBase=$publicapiendpoint
az webapp config connection-string set --name $appserviceapiname --connection-string-type SQLAzure -g $groupname --settings CatalogConnection=$catalogConnection
az webapp config connection-string set --name $appserviceapiname --connection-string-type SQLAzure -g $groupname --settings IdentityConnection=$catalogConnection
az webapp cors add -g $groupname -n $appserviceapiname --allowed-origins $webappendpoint_origin

