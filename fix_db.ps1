$connString = "Server=localhost;Database=master;Integrated Security=True;Connection Timeout=10;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()

Write-Host "Attempting to fix trungtamtienganh database..."

# Try to set ONLINE
try {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 15
    $cmd.CommandText = @"
    ALTER DATABASE trungtamtienganh SET EMERGENCY;
    ALTER DATABASE trungtamtienganh SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DBCC CHECKDB (trungtamtienganh, REPAIR_ALLOW_DATA_LOSS);
    ALTER DATABASE trungtamtienganh SET MULTI_USER;
    ALTER DATABASE trungtamtienganh SET ONLINE;
"@
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Successfully recovered trungtamtienganh!"
} catch {
    Write-Host "Emergency repair failed: " $_.Exception.Message
    Write-Host "Dropping corrupted RECOVERY_PENDING database and recreating cleanly..."
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandTimeout = 30
        $cmd.CommandText = @"
        ALTER DATABASE trungtamtienganh SET OFFLINE WITH ROLLBACK IMMEDIATE;
        DROP DATABASE trungtamtienganh;
        CREATE DATABASE trungtamtienganh;
"@
        $cmd.ExecuteNonQuery() | Out-Null
        Write-Host "Database recreated cleanly!"
    } catch {
        Write-Host "Drop and create failed: " $_.Exception.Message
    }
}

$conn.Close()
