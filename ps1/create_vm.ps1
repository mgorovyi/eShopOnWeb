#Connect-AzAccount

$rg = 'rg-vivethere'
$vm = 'vive-there-vm1'
$adminuser = "vive_there"
$adminpassword = ConvertTo-SecureString 'Gtxrby&100500' -AsPlainText -Force
$WindowsCred = New-Object System.Management.Automation.PSCredential($adminuser, $adminpassword)

New-AzResourceGroup -Name $rg -Location EastUs
New-AzVM -ResourceGroupName $rg -Name $vm -Credential $WindowsCred `
         -Image Win2016Datacenter -Size Standard_B1s `
         -OpenPorts 3389
Get-AzPublicIpAddress -ResourceGroupName $rg -Name $vm | Select-Object IpAddress


#Get-AzResourceGroup -Location EastUs
Remove-AzResourceGroup -Name $rg -Force -AsJob
Remove-AzResourceGroup -Name 'NetworkWatcherRG' -Force -AsJob
 Get-AzResourceGroup -Location EastUs