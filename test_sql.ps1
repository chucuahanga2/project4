$connString = "Server=localhost;Database=master;Integrated Security=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
try {
    $conn.Open()
    Write-Host "Connected to SQL Server master successfully!"

    # 1. Kiểm tra / Tạo database trungtamtienganh
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'trungtamtienganh')
BEGIN
    CREATE DATABASE trungtamtienganh;
    PRINT 'Database created';
END
"@
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Database trungtamtienganh verified/created!"

    # 2. Cấp quyền sở hữu database cho tài khoản hiện tại
    $user = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
    Write-Host "Current Windows User: $user"
    $cmd.CommandText = "ALTER AUTHORIZATION ON DATABASE::trungtamtienganh TO [$user];"
    try {
        $cmd.ExecuteNonQuery() | Out-Null
        Write-Host "Granted DB ownership to $user!"
    } catch {
        Write-Host "Notice on ALTER AUTHORIZATION: " $_.Exception.Message
    }

    $conn.Close()
    Write-Host "All master setup tasks completed successfully."
} catch {
    Write-Host "Fatal SQL Error: " $_.Exception.Message
}
