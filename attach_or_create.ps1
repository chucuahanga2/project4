$connString = "Server=localhost;Database=master;Integrated Security=True;Connection Timeout=15;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()

Write-Host "Creating fresh clean database trungtamtienganh..."
$cmd = $conn.CreateCommand()
$cmd.CommandTimeout = 30
$cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'trungtamtienganh')
BEGIN
    ALTER DATABASE trungtamtienganh SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE trungtamtienganh;
END

CREATE DATABASE trungtamtienganh 
ON PRIMARY (
    NAME = 'trungtamtienganh_data',
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER3\MSSQL\DATA\trungtamtienganh_v2.mdf'
)
LOG ON (
    NAME = 'trungtamtienganh_log',
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER3\MSSQL\DATA\trungtamtienganh_v2.ldf'
);
"@
try {
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Database trungtamtienganh created cleanly with fresh files!"
} catch {
    Write-Host "Error creating DB: " $_.Exception.Message
}

$conn.Close()
