$connString = "Server=localhost;Database=KhoaHocTrucTuyenDB;Integrated Security=True;Connection Timeout=5;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandTimeout = 5
$cmd.CommandText = "SELECT name FROM sys.tables ORDER BY name;"
$da = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
$dt = New-Object System.Data.DataTable
$da.Fill($dt) | Out-Null
Write-Host "Tables in KhoaHocTrucTuyenDB:"
foreach ($row in $dt.Rows) {
    Write-Host "- " $row["name"]
}
$conn.Close()
