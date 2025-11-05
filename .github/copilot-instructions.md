# PPacker - MonoGame Texture Atlas Packer

## Project Overview
PPacker is a command-line tool for packing PNG files into texture atlases for MonoGame projects. It combines multiple sprites into optimized atlases with JSON metadata and animation definitions, designed for integration into MonoGame build pipelines.

## Architecture & Key Components

### Core Components
- **`Core/BinPacker`**: Bottom-left bin packing algorithm with rotation support
- **`Core/SpriteProcessor`**: Image loading, trimming, and atlas generation using ImageSharp
- **`Core/AtlasPacker`**: Main coordinator that orchestrates the packing process
- **`Models/`**: Configuration and data models for JSON serialization

### Data Flow
1. JSON config → Load sprites (individual PNGs or sprite sheets) → Pack into atlas → Generate metadata + animations
2. Supports mixing individual sprites and existing sprite sheets with metadata
3. Outputs atlas PNG, sprite data JSON, and optional animation definitions JSON

## Development Workflows

### Building and Testing
```bash
# Build and run with sample config
dotnet build
dotnet run -- --config examples/sample-config.json --verbose

# Run unit tests
dotnet test

# Generate example configuration
dotnet run -- example --output ./test-examples
```

### Creating New Features
- **Adding packing algorithms**: Extend `BinPacker` class or create new implementations
- **New sprite processors**: Add methods to `SpriteProcessor` for different image operations
- **Animation patterns**: Extend `AnimationPattern` model and update `AtlasPacker.GenerateFrameNames()`

### Debugging
- Use `--verbose` flag to see detailed packing information
- Test with small sprite sets in `examples/` directory
- Breakpoints in `AtlasPacker.PackAsync()` for full workflow debugging

## Project Conventions

### Code Organization
- `src/PPacker/`: Main source code with `Core/`, `Models/` subdirectories
- `tests/PPacker.Tests/`: Unit tests mirroring source structure
- `examples/`: Sample configurations and sprite data files
- `.github/`: Repository metadata and CI/CD workflows

### Key Patterns
- **Configuration-driven**: All operations controlled by JSON config files
- **Fluent validation**: Configuration validation in `Program.ValidateConfig()`
- **Async/await**: File I/O operations use async patterns throughout
- **Resource disposal**: `SpriteInfo` implements `IDisposable` for ImageSharp cleanup
- **Immutable data**: Models use init-only properties where possible

### JSON Serialization
- Uses `System.Text.Json` with `PropertyNameCaseInsensitive = true`
- Models use `[JsonPropertyName]` attributes for consistent naming
- Configuration supports both individual sprites and sprite sheet inputs

## Key Integration Points

### Command Line Interface
- Built with `System.CommandLine` for argument parsing
- Main commands: pack (default), example generation
- Options: `--config`, `--verbose`, `--output-dir`
- Error handling with appropriate exit codes

### Image Processing
- **ImageSharp**: Core image manipulation (loading, cropping, compositing)
- **Trimming**: Automatic transparent pixel removal with offset tracking
- **Rotation**: Optional 90-degree rotation for better packing efficiency
- **Format support**: PNG input/output with configurable compression

### External Dependencies
- `SixLabors.ImageSharp`: Image processing and manipulation
- `System.CommandLine`: CLI framework and argument parsing
- `System.Text.Json`: Configuration and data serialization

## Common Tasks

### Adding New Packing Algorithms
1. Create new class inheriting from or similar to `BinPacker`
2. Implement `Pack(List<PackingRectangle>)` method
3. Update `AtlasPacker` to use new algorithm based on config option
4. Add configuration properties to `AtlasConfig` model

### Extending Animation System
```csharp
// Add new pattern types to AnimationPattern
public class AnimationPattern 
{
    public string PatternType { get; set; } = "sequence"; // "sequence", "grid", etc.
    public GridPattern? GridPattern { get; set; }
    // ... existing properties
}
```

### Adding Input Format Support
1. Extend `SpriteProcessor.LoadSpriteData()` to handle new JSON schema
2. Add format detection logic based on file structure
3. Create converter methods for non-standard sprite data formats
4. Update documentation with new format examples

### Performance Optimization
- **Memory management**: Dispose `SpriteInfo` objects promptly using `using` statements
- **Large atlases**: Consider streaming for very large input sets
- **Parallel processing**: `PackingRectangle` creation can be parallelized
- **Caching**: Hash input files to skip unchanged sprites

## Troubleshooting

### Common Issues
- **Packing failures**: Check max atlas dimensions vs sprite sizes and count
- **Missing sprites**: Verify file paths are relative to config file location
- **Memory issues**: Large images should be processed with streaming or in batches
- **Animation frame not found**: Ensure sprite names match exactly (case-sensitive)

When working on this project, focus on:
1. Understanding the content pipeline architecture before making changes
2. Testing with various asset types and sizes
3. Maintaining compatibility with MonoGame standards
4. Optimizing for both build-time performance and runtime efficiency