$connString = "Server=localhost;Database=master;Integrated Security=True;Connection Timeout=10;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandTimeout = 20
$cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'KhoaHocTrucTuyenDB')
BEGIN
    CREATE DATABASE KhoaHocTrucTuyenDB;
END
"@
$cmd.ExecuteNonQuery() | Out-Null
Write-Host "Database KhoaHocTrucTuyenDB created successfully!"

$conn.Close()
