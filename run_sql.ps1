param(
    [string]$FilePath = "d:\.idea\webkhoahoc\01_SetupDatabase_KhoaHocTrucTuyen.sql"
)

$connStr = "Server=localhost;Database=KhoaHocTrucTuyenDB;Integrated Security=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()
Write-Host "Connected to KhoaHocTrucTuyenDB."

$content = Get-Content $FilePath -Raw -Encoding UTF8
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
            Write-Host "Notice/Error in batch: " $_.Exception.Message
        }
    }
}

$conn.Close()
Write-Host "Finished executing: $FilePath"
