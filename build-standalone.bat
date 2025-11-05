@echo off
echo Building PPacker standalone executable for Windows...

if exist dist\win-x64 rmdir /s /q dist\win-x64
mkdir dist\win-x64 2>nul

echo Building Windows x64 standalone executable...
dotnet publish src\PPacker\PPacker.csproj -c Release -r win-x64 --self-contained true -o dist\win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

if %errorlevel% equ 0 (
    echo.
    echo ✓ Build successful!
    echo Standalone executable created at: dist\win-x64\PPacker.exe
    echo.
    echo To test it, run: dist\win-x64\PPacker.exe --help
) else (
    echo.
    echo ✗ Build failed!
    exit /b 1
)