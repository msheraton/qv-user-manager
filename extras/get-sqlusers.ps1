$SqlConnection = New-Object System.Data.SqlClient.SqlConnection
$SqlConnection.ConnectionString = "Server=localhost;Database=mydb;Integrated Security=True"
$SqlConnection.Open()
$SqlCmd = New-Object System.Data.SqlClient.SqlCommand
$SqlCmd.Connection = $cn
$SqlCmd.CommandText = $args[0]
$dr = $SqlCmd.ExecuteReader()
while ($dr.Read())
{
    write-host $dr.GetString(0)
}
$SqlConnection.Close()