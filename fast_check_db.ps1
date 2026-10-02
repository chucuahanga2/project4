$connString = "Server=localhost;Database=master;Integrated Security=True;Connection Timeout=5;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandTimeout = 5
$cmd.CommandText = "SELECT name, state_desc FROM sys.databases;"
$da = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
$dt = New-Object System.Data.DataTable
$da.Fill($dt) | Out-Null
foreach ($row in $dt.Rows) {
    Write-Host "DB: " $row["name"] "(" $row["state_desc"] ")"
}
$conn.Close()
