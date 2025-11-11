using System.CommandLine;
using System.Reflection;
using System.Text.Json;
using PPacker.Core;
using PPacker.Models;

namespace PPacker;

/// <summary>
/// Main program entry point
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        // Always print version banner on startup
        var version = typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "unknown";
        Console.WriteLine($"PPacker v{version}");

        var rootCommand = new RootCommand("PPacker - MonoGame Texture Atlas Packer")
        {
            Description = "Pack PNG files into texture atlases for MonoGame projects with sprite animation support."
        };

        // Configuration file option
        var configOption = new Option<FileInfo?>(
            name: "--config",
            description: "Path to the configuration JSON file")
        {
            IsRequired = true
        };
        configOption.AddAlias("-c");

        // Verbose option
        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Enable verbose output")
        {
            IsRequired = false
        };
        verboseOption.AddAlias("-v");

        // Output directory option (optional override)
        var outputDirOption = new Option<DirectoryInfo?>(
            name: "--output-dir",
            description: "Override output directory from config")
        {
            IsRequired = false
        };
        outputDirOption.AddAlias("-o");

        rootCommand.AddOption(configOption);
        rootCommand.AddOption(verboseOption);
        rootCommand.AddOption(outputDirOption);

        rootCommand.SetHandler(async (config, verbose, outputDir) =>
        {
            await HandlePackCommand(config!, verbose, outputDir);
        }, configOption, verboseOption, outputDirOption);

        // Add example command
        var exampleCommand = new Command("example", "Generate example configuration files");
        var exampleOutputOption = new Option<DirectoryInfo?>(
            name: "--output",
            description: "Directory to create example files in (default: current directory)")
        {
            IsRequired = false
        };
        exampleOutputOption.AddAlias("-o");

        exampleCommand.AddOption(exampleOutputOption);
        exampleCommand.SetHandler(async (outputDir) =>
        {
            await HandleExampleCommand(outputDir);
        }, exampleOutputOption);

        rootCommand.AddCommand(exampleCommand);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task HandlePackCommand(FileInfo configFile, bool verbose, DirectoryInfo? outputDir)
    {
        try
        {
            if (verbose)
            {
                Console.WriteLine($"Configuration file: {configFile.FullName}");
            }

            if (!configFile.Exists)
            {
                Console.WriteLine($"Error: Configuration file not found: {configFile.FullName}");
                Environment.Exit(1);
                return;
            }

            // Load configuration
            var configJson = await File.ReadAllTextAsync(configFile.FullName);
            var config = JsonSerializer.Deserialize<PackerConfig>(configJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config == null)
            {
                Console.WriteLine("Error: Failed to parse configuration file");
                Environment.Exit(1);
                return;
            }

            // Override output directory if specified
            if (outputDir != null)
            {
                var outputDirPath = outputDir.FullName;
                config.Output.ImagePath = Path.Combine(outputDirPath, Path.GetFileName(config.Output.ImagePath));
                config.Output.DataPath = Path.Combine(outputDirPath, Path.GetFileName(config.Output.DataPath));
                if (!string.IsNullOrEmpty(config.Output.AnimationPath))
                {
                    config.Output.AnimationPath = Path.Combine(outputDirPath, Path.GetFileName(config.Output.AnimationPath));
                }
            }

            // Validate configuration
            if (!ValidateConfig(config))
            {
                Environment.Exit(1);
                return;
            }

            if (verbose)
            {
                PrintConfigSummary(config);
            }

            // Pack the atlas
            var packer = new AtlasPacker(config, verbose);
            var success = await packer.PackAsync();

            if (success)
            {
                Console.WriteLine("Packing completed successfully!");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Packing failed!");
                Environment.Exit(1);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (verbose)
            {
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            Environment.Exit(1);
        }
    }

    private static async Task HandleExampleCommand(DirectoryInfo? outputDir)
    {
        var targetDir = outputDir?.FullName ?? Directory.GetCurrentDirectory();
        
        Console.WriteLine($"Creating example configuration files in: {targetDir}");

        try
        {
            // Ensure target directory exists
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // Create example configuration
            var exampleConfig = new PackerConfig
            {
                Inputs = new List<InputConfig>
                {
                    new()
                    {
                        ImagePath = "sprites/player.png",
                        DataPath = null,
                        Prefix = "player_"
                    },
                    new()
                    {
                        ImagePath = "sprites/enemies.png",
                        DataPath = "sprites/enemies.json",
                        Prefix = "enemy_"
                    }
                },
                Output = new OutputConfig
                {
                    ImagePath = "output/atlas.png",
                    DataPath = "output/atlas.json",
                    AnimationPath = "output/animations.json"
                },
                Atlas = new AtlasConfig
                {
                    MaxWidth = 2048,
                    MaxHeight = 2048,
                    Padding = 2,
                    AllowRotation = false,
                    TrimSprites = true,
                    PowerOfTwo = true
                },
                Animations = new List<AnimationConfig>
                {
                    new()
                    {
                        Name = "player_walk",
                        Pattern = new AnimationPattern
                        {
                            NamePattern = "player_walk_{0:D2}",
                            StartFrame = 1,
                            EndFrame = 8
                        },
                        FrameDuration = 100,
                        Loop = true
                    },
                    new()
                    {
                        Name = "player_idle",
                        Frames = new List<string> { "player_idle_01", "player_idle_02", "player_idle_03" },
                        FrameDuration = 200,
                        Loop = true
                    }
                }
            };

            var configPath = Path.Combine(targetDir, "ppacker-config.json");
            var configJson = JsonSerializer.Serialize(exampleConfig, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(configPath, configJson);

            // Create example sprite data
            var exampleSpriteData = new AtlasData
            {
                Width = 512,
                Height = 256,
                Sprites = new List<SpriteData>
                {
                    new() { Name = "enemy_goblin", X = 0, Y = 0, Width = 32, Height = 32, Rotated = false },
                    new() { Name = "enemy_orc", X = 32, Y = 0, Width = 48, Height = 48, Rotated = false },
                    new() { Name = "enemy_dragon", X = 80, Y = 0, Width = 64, Height = 64, Rotated = false }
                }
            };

            var spriteDataPath = Path.Combine(targetDir, "example-sprites.json");
            var spriteDataJson = JsonSerializer.Serialize(exampleSpriteData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(spriteDataPath, spriteDataJson);

            // Create README
            var readmePath = Path.Combine(targetDir, "PPACKER_EXAMPLES.md");
            var readmeContent = @"# PPacker Example Configuration

This directory contains example configuration files for PPacker.

## Files

- `ppacker-config.json` - Main configuration file showing all available options
- `example-sprites.json` - Example sprite data file format

## Usage

1. Organize your sprite files according to the input paths in the config
2. Update the file paths in `ppacker-config.json` to match your project structure
3. Run PPacker:

```bash
ppacker --config ppacker-config.json
```

## Configuration Options

### Inputs
- `imagePath`: Path to PNG file
- `dataPath`: Optional JSON file with sprite definitions
- `prefix`: Optional prefix for sprite names

### Atlas Settings
- `maxWidth`/`maxHeight`: Maximum atlas dimensions
- `padding`: Pixels between sprites
- `allowRotation`: Allow sprite rotation for better packing
- `trimSprites`: Remove transparent borders
- `powerOfTwo`: Force power-of-2 dimensions

### Animations
- Use `frames` for explicit frame lists
- Use `pattern` for automatic frame sequence generation
";

            await File.WriteAllTextAsync(readmePath, readmeContent);

            Console.WriteLine("Example files created:");
            Console.WriteLine($"  - {configPath}");
            Console.WriteLine($"  - {spriteDataPath}");
            Console.WriteLine($"  - {readmePath}");
            Console.WriteLine();
            Console.WriteLine("To test PPacker, run:");
            Console.WriteLine($"  ppacker --config \"{configPath}\"");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating example files: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static bool ValidateConfig(PackerConfig config)
    {
        if (!config.Inputs.Any())
        {
            Console.WriteLine("Error: No input files specified in configuration");
            return false;
        }

        foreach (var input in config.Inputs)
        {
            if (string.IsNullOrEmpty(input.ImagePath))
            {
                Console.WriteLine("Error: Input image path cannot be empty");
                return false;
            }
        }

        if (string.IsNullOrEmpty(config.Output.ImagePath))
        {
            Console.WriteLine("Error: Output image path cannot be empty");
            return false;
        }

        if (string.IsNullOrEmpty(config.Output.DataPath))
        {
            Console.WriteLine("Error: Output data path cannot be empty");
            return false;
        }

        if (config.Atlas.MaxWidth <= 0 || config.Atlas.MaxHeight <= 0)
        {
            Console.WriteLine("Error: Atlas dimensions must be positive");
            return false;
        }

        return true;
    }

    private static void PrintConfigSummary(PackerConfig config)
    {
        Console.WriteLine();
        Console.WriteLine("Configuration Summary:");
        Console.WriteLine($"  Inputs: {config.Inputs.Count} file(s)");
        Console.WriteLine($"  Max Atlas Size: {config.Atlas.MaxWidth}x{config.Atlas.MaxHeight}");
        Console.WriteLine($"  Padding: {config.Atlas.Padding}px");
        Console.WriteLine($"  Allow Rotation: {config.Atlas.AllowRotation}");
        Console.WriteLine($"  Trim Sprites: {config.Atlas.TrimSprites}");
        Console.WriteLine($"  Power of Two: {config.Atlas.PowerOfTwo}");
        Console.WriteLine($"  Animations: {config.Animations?.Count ?? 0} defined");
        Console.WriteLine();
    }
}