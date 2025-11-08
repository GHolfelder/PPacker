# Build standalone executable for Windows x64

Write-Host "Building PPacker standalone executable..." -ForegroundColor Green

# Create output directory
$outputDir = ".\dist"
if (Test-Path $outputDir) {
    Remove-Item $outputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $outputDir | Out-Null

# Windows x64
Write-Host "Building Windows x64..." -ForegroundColor Yellow
dotnet publish src\PPacker\PPacker.csproj -c Release -r win-x64 --self-contained true -o "$outputDir\win-x64" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful!" -ForegroundColor Green
    $exe = Get-Item "$outputDir\win-x64\PPacker.exe"
    $size = [math]::Round($exe.Length / 1MB, 2)
    Write-Host "Created: PPacker.exe ($size MB)" -ForegroundColor Cyan
} else {
    Write-Host "Build failed!" -ForegroundColor Red
}
