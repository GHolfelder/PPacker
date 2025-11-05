# PPacker Standalone Executable Troubleshooting

## Common Issues and Solutions

### Issue: "PPacker.dll not found" Error

If you get an error about missing PPacker.dll when running the standalone executable, try these solutions:

#### Solution 1: Rebuild with explicit parameters
```bash
dotnet clean src\PPacker\PPacker.csproj
dotnet publish src\PPacker\PPacker.csproj -c Release -r win-x64 --self-contained true -o dist\win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

#### Solution 2: Use the batch file
```bash
.\build-standalone.bat
```

#### Solution 3: Check antivirus software
Some antivirus programs interfere with self-extracting executables. Try:
- Temporarily disabling antivirus
- Adding the executable to antivirus whitelist
- Running from a different location

#### Solution 4: Use framework-dependent version
If the self-contained version continues to have issues, use the framework-dependent version (requires .NET 8.0 to be installed):

```bash
dotnet publish src\PPacker\PPacker.csproj -c Release --self-contained false -o dist\framework-dependent
```

Then run with:
```bash
dotnet .\dist\framework-dependent\PPacker.dll --help
```

### Issue: Large File Size

The standalone executable is ~80MB because it includes the entire .NET runtime. This is normal for self-contained applications.

To reduce size:
- Use framework-dependent deployment (requires .NET installation)
- Enable trimming (may cause compatibility issues):
  ```xml
  <PublishTrimmed>true</PublishTrimmed>
  ```

### Issue: Performance on First Run

The first run might be slower due to extraction and JIT compilation. Subsequent runs will be faster.

### Issue: Permissions Error

On Linux/macOS, you might need to make the executable file executable:
```bash
chmod +x ./PPacker
```

## Verification Steps

1. **Check file size**: The executable should be around 80MB
2. **Test help command**: `.\PPacker.exe --help` should display help text
3. **Test example generation**: `.\PPacker.exe example --output test`
4. **Check dependencies**: The executable should run without requiring .NET installation

## Alternative Deployment Methods

### Global Tool
```bash
dotnet pack src\PPacker\PPacker.csproj -c Release
dotnet tool install -g --add-source .\bin\Release PPacker
```

### Framework-Dependent
```bash
dotnet publish src\PPacker\PPacker.csproj -c Release --self-contained false
```

### Docker (if needed)
```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:8.0
COPY dist/framework-dependent /app
WORKDIR /app
ENTRYPOINT ["dotnet", "PPacker.dll"]
```

## Getting Help

If you continue to have issues:
1. Check that you're using .NET 8.0 SDK or later
2. Verify your operating system is supported
3. Try building from a clean directory
4. Check Windows Defender or antivirus logs
5. Try running from command prompt as administrator

## Supported Platforms

- Windows x64 (tested)
- Windows x86 
- Linux x64
- macOS x64 (Intel)
- macOS ARM64 (Apple Silicon)