$connString = "Server=localhost;Database=master;Integrated Security=True;Connection Timeout=30;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
Write-Host "Connected to SQL Server (master)!"

# Switch database to KhoaHocTrucTuyenDB
$cmd = $conn.CreateCommand()
$cmd.CommandText = "USE KhoaHocTrucTuyenDB;"
$cmd.ExecuteNonQuery() | Out-Null
Write-Host "Switched to KhoaHocTrucTuyenDB successfully!"

function Execute-SqlScript($filePath) {
    Write-Host "Running script: $filePath"
    $content = Get-Content $filePath -Raw -Encoding UTF8
    # Loại bỏ lệnh CREATE DATABASE trungtamtienganh hoặc USE trungtamtienganh
    $content = $content -replace "(?i)CREATE DATABASE trungtamtienganh;", "-- SKIP CREATE DB"
    $content = $content -replace "(?i)USE trungtamtienganh;", "USE KhoaHocTrucTuyenDB;"
    
    # Chia thành các batch GO
    $batches = $content -split "(?m)^\s*GO\s*$"
    foreach ($batch in $batches) {
        $trimmed = $batch.Trim()
        if (![string]::IsNullOrWhiteSpace($trimmed)) {
            $cmd = $conn.CreateCommand()
            $cmd.CommandTimeout = 60
            $cmd.CommandText = $trimmed
            try {
                $cmd.ExecuteNonQuery() | Out-Null
            } catch {
                Write-Host "Notice in batch: " $_.Exception.Message
            }
        }
    }
    Write-Host "Finished script: $filePath"
}

# 1. Base Tables
Execute-SqlScript "d:\.idea\webkhoahoc\DatabaseKLTN.sql"

# 2. Base Data
Execute-SqlScript "d:\.idea\webkhoahoc\Insert.sql"

# 3. Online Course Platform Extensions
Execute-SqlScript "d:\.idea\webkhoahoc\UpdateDatabase_KhoaHocTrucTuyen.sql"

$conn.Close()
Write-Host "Database KhoaHocTrucTuyenDB fully populated and configured!"
