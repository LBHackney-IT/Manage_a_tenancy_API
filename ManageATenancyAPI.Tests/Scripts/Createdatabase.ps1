[reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.Smo")

#############
#Params
$sqlinstance =".\SQLEXPRESS"
$databasename="ManageATenancyIntegrationTestDatabase"

$ManageAtenancySchemaPath ="manage-a-tenancy-schema.sql"
$UhWarehouseSchemaPath="uh-warehouse-schema.sql"
$ManageAtenancyDataPath ="manage-a-tenancy-data.sql"
$UhWarehouseSchemaDataPath="uh-warehouse-data.sql"

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

  Write-Host "Run :"$ManageAtenancySchemaPath
  invoke-sqlcmd -inputfile $PSScriptRoot"\"$ManageAtenancySchemaPath -serverinstance $sqlinstance -database $databasename 
  
  Write-Host "Run :"$UhWarehouseSchemaPath
  invoke-sqlcmd -inputfile $PSScriptRoot"\"$UhWarehouseSchemaPath -serverinstance $sqlinstance -database $databasename 
  
  Write-Host "Run :"$ManageAtenancyDataPath
  invoke-sqlcmd -inputfile $PSScriptRoot"\"$ManageAtenancyDataPath -serverinstance $sqlinstance -database $databasename 
  
  Write-Host "Run :"$UhWarehouseSchemaDataPath
  invoke-sqlcmd -inputfile $PSScriptRoot"\"$UhWarehouseSchemaDataPath -serverinstance $sqlinstance -database $databasename 
