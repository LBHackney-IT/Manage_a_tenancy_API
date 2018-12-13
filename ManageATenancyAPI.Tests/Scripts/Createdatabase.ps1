Start-Transcript -Path  $PSScriptRoot"\createdatabase.log"
[reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.Smo")

#############
#Params
$sqlinstance =".\SQLEXPRESS"
$databasename="ManageATenancyIntegrationTestDatabase"

$ManageAtenancySchemaPath ="manage-a-tenancy-schema.sql"
$UhWarehouseSchemaPath="uh-warehouse-schema.sql"

$ManageAtenancyDataPath ="manage-a-tenancy-data.sql"
$UhWarehouseDataPath="uh-warehouse-data.sql"

#############

$server = new-object ("Microsoft.SqlServer.Management.Smo.Server") $sqlinstance

$dbExists = $FALSE
foreach ($db in $server.databases) {
  if ($db.name -eq $databasename) {
    Write-Host "$databasename already exists."
    $dbExists = $TRUE
  }
}
if ($dbExists -eq $TRUE) {

   Write-Host "Drop database" $databasename
   $server.KillAllProcesses($databasename)
   $server.databases[$databasename].Drop()  
   
}
  Write-Host "Create database" $databasename

 $db = New-Object -TypeName Microsoft.SqlServer.Management.Smo.Database -argumentlist $server, $databasename
 $db.Create()

 $user = "NT AUTHORITY\NETWORK SERVICE"
 $usr = New-Object -TypeName Microsoft.SqlServer.Management.Smo.User -argumentlist $db, $user
 $usr.Login = $user
 $usr.Create()

 $role = $db.Roles["db_datareader"]
 $role.AddMember($user)

  Write-Host "Install Schema and test data"

Write-Host "Run :" + $ManageAtenancySchemaPath + "."
& cmd.exe /c sqlcmd -S $sqlinstance -d $databasename  -i $PSScriptRoot"\"$ManageAtenancySchemaPath 
  
Write-Host "Run :" + $UhWarehouseSchemaPath + "."
& cmd.exe /c sqlcmd -S $sqlinstance -d $databasename  -i $PSScriptRoot"\"$UhWarehouseSchemaPath 
  
Write-Host "Run :" + $UhWarehouseDataPath + "."
& cmd.exe /c sqlcmd -S $sqlinstance -d $databasename  -i $PSScriptRoot"\"$UhWarehouseDataPath
  
Write-Host "Run :" + $ManageAtenancyDataPath + "."
& cmd.exe /c sqlcmd -S $sqlinstance -d $databasename -i $PSScriptRoot"\"$ManageAtenancyDataPath
  


  Stop-Transcript