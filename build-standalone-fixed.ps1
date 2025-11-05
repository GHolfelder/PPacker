# Build standalone executables for different platforms

Write-Host "Building PPacker standalone executables..." -ForegroundColor Green

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
    Write-Host "✓ Windows x64 build successful" -ForegroundColor Green
} else {
    Write-Host "✗ Windows x64 build failed" -ForegroundColor Red
}

# Windows x86
Write-Host "Building Windows x86..." -ForegroundColor Yellow
dotnet publish src\PPacker\PPacker.csproj -c Release -r win-x86 --self-contained true -o "$outputDir\win-x86" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Windows x86 build successful" -ForegroundColor Green
} else {
    Write-Host "✗ Windows x86 build failed" -ForegroundColor Red
}

# Linux x64
Write-Host "Building Linux x64..." -ForegroundColor Yellow
dotnet publish src\PPacker\PPacker.csproj -c Release -r linux-x64 --self-contained true -o "$outputDir\linux-x64" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Linux x64 build successful" -ForegroundColor Green
} else {
    Write-Host "✗ Linux x64 build failed" -ForegroundColor Red
}

# macOS x64
Write-Host "Building macOS x64..." -ForegroundColor Yellow
dotnet publish src\PPacker\PPacker.csproj -c Release -r osx-x64 --self-contained true -o "$outputDir\osx-x64" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ macOS x64 build successful" -ForegroundColor Green
} else {
    Write-Host "✗ macOS x64 build failed" -ForegroundColor Red
}

# macOS ARM64 (Apple Silicon)
Write-Host "Building macOS ARM64..." -ForegroundColor Yellow
dotnet publish src\PPacker\PPacker.csproj -c Release -r osx-arm64 --self-contained true -o "$outputDir\osx-arm64" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ macOS ARM64 build successful" -ForegroundColor Green
} else {
    Write-Host "✗ macOS ARM64 build failed" -ForegroundColor Red
}

Write-Host ""
Write-Host "Build complete! Standalone executables are in the 'dist' folder:" -ForegroundColor Green
Get-ChildItem $outputDir -Directory | ForEach-Object {
    $exePath = Join-Path $_.FullName "PPacker*"
    $exe = Get-ChildItem $exePath -File | Select-Object -First 1
    if ($exe) {
        $size = [math]::Round($exe.Length / 1MB, 2)
        Write-Host "  $($_.Name): $($exe.Name) ($size MB)" -ForegroundColor Cyan
    }
}

Write-Host ""
Write-Host "To use the standalone executable:" -ForegroundColor Yellow
Write-Host "  Windows: .\dist\win-x64\PPacker.exe --help" -ForegroundColor Gray
Write-Host "  Linux:   ./dist/linux-x64/PPacker --help" -ForegroundColor Gray
Write-Host "  macOS:   ./dist/osx-x64/PPacker --help" -ForegroundColor Gray