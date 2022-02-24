cd "C:\Users\Vadym_Tarasov\source\repos\eShopOnWeb\"

$groupname="vivethere0118rg2"
$acr_name="vivethereacr0118cls2"

#az group create --name $groupname --location=eastus

az acr create --name $acr_name --resource-group $groupname --sku Standard --admin-enabled true #use az acr update



az acr build --registry $acr_name --image vivethereeshopweb:v1 -f ./src/web/Dockerfile_elm8 .


az acr build --registry $acr_name --image vivetherepublicapi:v1 -f ./src/publicapi/Dockerfile_elm8 .

#verify image
az acr repository list --name $acr_name --output table

#Enable admin account
#az acr update --name $acr_name --admin-enabled true

#Get admin user name
$acradmin=$(az acr credential show --name $acr_name --query username -o tsv)
echo $acradmin

#Get admin password
$acradminpassword=$(az acr credential show --name $acr_name --query passwords[0].value -o tsv)
echo $acradminpassword

#get acr login server
$acrLoginServer=$(az acr show --name $acr_name --query loginServer -o tsv)
echo $acrLoginServer

#get acr location
$acrLocation=$(az acr show --name $acr_name --query location -o tsv)
echo $acrLocation


#get image name
#$acrImageName=$acr_name+".azurecr.io/helloacrtasks:v1"

#we must register container instance!

#Deploy a container
#az container create `
#    --name vivethereacrtask `
#    --resource-group $groupname `
#    --image $acrImageName `
#    --registry-login-server $acrLoginServer `
#    --registry-username $acradmin `
#    --registry-password $acradminpassword `
#    --location $acrLocation `
#    --ip-address Public


#$ip = $(az container show --name vivethereacrtask -g $groupname --query ipAddress.ip -o tsv)
#echo $ip




