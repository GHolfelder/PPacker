param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("build", "standalone", "test", "pack", "clean", "example", "help")]
    [string]$Target,
    
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [ValidateSet("win-x64", "linux-x64", "osx-x64", "osx-arm64", "all")]
    [string]$Platform = "win-x64",
    
    [string]$OutputDir = "./dist",
    
    [switch]$NoTrimming,
    
    [switch]$VerboseOutput
)

function Write-Color($Message, $Color = "White") {
    Write-Host $Message -ForegroundColor $Color
}

function Test-DotNet {
    try {
        dotnet --version | Out-Null
        return $true
    } catch {
        Write-Color "Error: .NET SDK not found" Red
        return $false
    }
}

Write-Color "PPacker Build Script" Cyan
Write-Color "===================" Cyan

if (-not (Test-DotNet)) { exit 1 }

switch ($Target) {
    "build" {
        Write-Color "Building PPacker ($Configuration)..." Blue
        $args = @("build", "src/PPacker/PPacker.csproj", "-c", $Configuration)
        if ($VerboseOutput) { $args += "--verbosity", "normal" }
        & dotnet @args
        if ($LASTEXITCODE -eq 0) {
            Write-Color "Build completed successfully!" Green
        } else {
            Write-Color "Build failed!" Red
            exit 1
        }
    }
    
    "standalone" {
        Write-Color "Building standalone executables..." Blue
        
        $platforms = if ($Platform -eq "all") {
            @("win-x64", "linux-x64", "osx-x64", "osx-arm64")
        } else {
            @($Platform)
        }
        
        if (Test-Path $OutputDir) {
            Remove-Item $OutputDir -Recurse -Force
        }
        
        foreach ($plat in $platforms) {
            Write-Color "Building for $plat..." Yellow
            $outPath = Join-Path $OutputDir $plat
            
            $args = @(
                "publish", "src/PPacker/PPacker.csproj", 
                "-c", $Configuration, "-r", $plat,
                "--self-contained", "true", "-o", $outPath,
                "-p:PublishSingleFile=true"
            )
            
            if ($NoTrimming) {
                $args += "-p:PublishTrimmed=false"
                Write-Color "  Trimming disabled" Yellow
            }
            
            & dotnet @args
            
            if ($LASTEXITCODE -eq 0) {
                $exeName = if ($plat.StartsWith("win")) { "PPacker.exe" } else { "PPacker" }
                $exePath = Join-Path $outPath $exeName
                if (Test-Path $exePath) {
                    $size = [math]::Round((Get-Item $exePath).Length / 1MB, 2)
                    Write-Color "  $plat completed ($size MB)" Green
                }
            } else {
                Write-Color "  $plat failed!" Red
            }
        }
    }
    
    "test" {
        Write-Color "Running tests..." Blue
        $args = @("test", "tests/PPacker.Tests/PPacker.Tests.csproj")
        if ($VerboseOutput) { $args += "--verbosity", "normal" }
        & dotnet @args
        if ($LASTEXITCODE -eq 0) {
            Write-Color "All tests passed!" Green
        } else {
            Write-Color "Tests failed!" Red
            exit 1
        }
    }
    
    "pack" {
        Write-Color "Creating NuGet package..." Blue
        $args = @("pack", "src/PPacker/PPacker.csproj", "-c", $Configuration)
        & dotnet @args
        if ($LASTEXITCODE -eq 0) {
            Write-Color "Package created successfully!" Green
        } else {
            Write-Color "Package creation failed!" Red
            exit 1
        }
    }
    
    "clean" {
        Write-Color "Cleaning build artifacts..." Blue
        $paths = @("src/PPacker/bin", "src/PPacker/obj", "tests/PPacker.Tests/bin", "tests/PPacker.Tests/obj", $OutputDir)
        foreach ($path in $paths) {
            if (Test-Path $path) {
                Write-Color "  Removing $path" Yellow
                Remove-Item $path -Recurse -Force
            }
        }
        Write-Color "Clean completed!" Green
    }
    
    "example" {
        Write-Color "Generating example..." Blue
        & dotnet run --project src/PPacker/PPacker.csproj -- example --output ./examples-generated
        if ($LASTEXITCODE -eq 0) {
            Write-Color "Example generated in ./examples-generated" Green
        } else {
            Write-Color "Example generation failed!" Red
            exit 1
        }
    }
    
    "help" {
        Write-Color ""
        Write-Color "Available targets:" Green
        Write-Color "  build      - Build the project"
        Write-Color "  standalone - Create self-contained executables"
        Write-Color "  test       - Run unit tests"
        Write-Color "  pack       - Create NuGet package"
        Write-Color "  clean      - Clean build artifacts"
        Write-Color "  example    - Generate example configuration"
        Write-Color "  help       - Show this help"
        Write-Color ""
        Write-Color "Options:" Green
        Write-Color "  -Configuration Debug|Release  (Default: Release)"
        Write-Color "  -Platform win-x64|linux-x64|osx-x64|osx-arm64|all  (Default: win-x64)"
        Write-Color "  -OutputDir path  (Default: ./dist)"
        Write-Color "  -NoTrimming  (Disable trimming)"
        Write-Color "  -VerboseOutput  (Enable verbose output)"
        Write-Color ""
        Write-Color "Examples:" Yellow
        Write-Color "  .\build.ps1 build"
        Write-Color "  .\build.ps1 standalone -Platform all -NoTrimming"
        Write-Color "  .\build.ps1 test -VerboseOutput"
    }
}

Write-Color "Done!" Green