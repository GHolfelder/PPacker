# PPacker - MonoGame Texture Atlas Packer

## Project Overview
PPacker is a command-line tool for packing PNG files into texture atlases for MonoGame projects. It combines multiple sprites into optimized atlases with JSON metadata and animation definitions, designed for integration into MonoGame build pipelines. Features fixed sprite positioning for fonts and UI elements, comprehensive Tiled TMX map support, and priority-based packing algorithms.

## Architecture & Key Components

### Core Components
- **`Core/BinPacker`**: Two-phase bin packing algorithm with fixed positioning, rotation support, and priority-based ordering
- **`Core/SpriteProcessor`**: Image loading, trimming, and atlas generation using ImageSharp
- **`Core/AtlasPacker`**: Main coordinator that orchestrates the packing process with sprite-to-input mapping
- **`Core/TiledMapProcessor`**: TMX/TSX file parsing and map conversion to MonoGame format
- **`Models/`**: Configuration and data models for JSON serialization, including fixed positioning metadata

### Data Flow
1. JSON config → Load sprites (individual PNGs, sprite sheets, or TMX maps) with positioning metadata → Two-phase pack (fixed first, then dynamic) → Generate metadata + animations + map data
2. Supports mixing individual sprites, existing sprite sheets, and Tiled TMX maps with metadata
3. Fixed positioning: Place critical sprites (fonts, UI) at exact coordinates while others pack dynamically around them
4. Outputs atlas PNG, sprite data JSON, optional animation definitions JSON, and map data JSON
5. TMX processing: Load TMX/TSX → Extract tileset images → Apply prefixes → Generate map names → Convert to MonoGame format

## Development Workflows

### Building and Testing
```bash
# Build and run with sample config
dotnet build
dotnet run -- --config examples/sample-config.json --verbose

# Test with fixed positioning
dotnet run -- --config examples/fixed-position-config.json --verbose

# Test with TMX maps
dotnet run -- --config examples/maps/maps-config.json --verbose

# Run unit tests
dotnet test

# Generate example configuration
dotnet run -- example --output ./test-examples
```

### Creating New Features
- **Adding packing algorithms**: Extend `BinPacker` class or create new implementations
- **Fixed positioning**: Modify `BinPacker.TryPlaceFixedRectangle()` for new positioning logic
- **Priority systems**: Update priority-based sorting in `BinPacker.Pack()` method  
- **New sprite processors**: Add methods to `SpriteProcessor` for different image operations
- **Animation patterns**: Extend `AnimationPattern` model and update `AtlasPacker.GenerateFrameNames()`
- **TMX processing**: Extend `TiledMapProcessor` for new TMX features or map formats
- **Map naming**: Modify prefix-based naming logic in `AtlasPacker.ProcessTmxMapsAsync()`
- **Sprite mapping**: Extend `AtlasPacker.FindInputForSprite()` for new input types

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
- Configuration supports individual sprites, sprite sheets, and TMX map inputs
- Fixed positioning: `fixedPosition` object with x,y coordinates and optional `priority`
- TMX processing generates prefix-based map names (`{prefix}map` or `map` fallback)
- Map data always serialized as array structure for consistency

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

### TMX/Map Processing
- **XML Parsing**: Uses `XmlSerializer` for TMX/TSX file parsing
- **External Tilesets**: Automatically loads referenced TSX files
- **Layer Support**: Handles tile layers, object layers, image layers
- **Data Formats**: Supports CSV and Base64 encoding with GZIP/ZLIB compression
- **Prefix-based Naming**: Maps named as `{prefix}map` (e.g., `desert_` → `desertmap`)
- **Validation**: Prevents duplicate map names during configuration validation

### External Dependencies
- `SixLabors.ImageSharp`: Image processing and manipulation
- `System.CommandLine`: CLI framework and argument parsing
- `System.Text.Json`: Configuration and data serialization

## Common Tasks

### Adding TMX/Map Features
1. Extend `TiledMapProcessor` methods for new TMX functionality
2. Update `TiledModels.cs` for new XML structures or properties
3. Modify `AtlasPacker.ProcessTmxMapsAsync()` for workflow changes
4. Add validation logic in `Program.ValidateConfig()` for new requirements
5. Update map naming logic if needed for special cases

### Modifying Map Naming Logic
```csharp
// Map naming in AtlasPacker.ProcessTmxMapsAsync()
var mapName = !string.IsNullOrEmpty(input.Prefix) 
    ? $"{input.Prefix.TrimEnd('_')}map"
    : "map";

// Validation happens in Program.ValidateConfig()
if (usedMapNames.Contains(mapName)) {
    // Handle conflict
}
```

### Adding New Packing Algorithms
1. Create new class inheriting from or similar to `BinPacker`
2. Implement `Pack(List<PackingRectangle>)` method
3. Update `AtlasPacker` to use new algorithm based on config option
4. Add configuration properties to `AtlasConfig` model

### Working with Fixed Positioning
```csharp
// Two-phase packing in BinPacker.Pack()
var fixedRects = rectangles.Where(r => r.IsFixed).ToList();
var dynamicRects = rectangles.Where(r => !r.IsFixed).ToList();

// Place fixed sprites first
foreach (var rect in fixedRects) {
    if (!TryPlaceFixedRectangle(rect)) return null;
}

// Then pack dynamic sprites with priority sorting
var sortedDynamicRects = dynamicRects
    .OrderByDescending(r => r.Priority)
    .ThenByDescending(r => r.Area);
```

### Adding Fixed Position Features
1. Extend `InputConfig` with new positioning properties (x, y coordinates)
2. Update `PackingRectangle` to include positioning metadata and validation
3. Modify `BinPacker.TryPlaceFixedRectangle()` for new placement logic
4. Enhance `AtlasPacker.FindInputForSprite()` for sprite-to-config mapping
5. Add validation in `Program.ValidateConfig()` for position conflicts

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
- **TMX file not found**: Verify TMX/TSX paths are relative to config file location
- **Duplicate map names**: Ensure TMX inputs have unique prefixes to avoid naming conflicts
- **External tileset errors**: Check that TSX files exist and image paths within them are correct
- **Map conversion issues**: Verify TMX layer data encoding/compression is supported (CSV, Base64+GZIP/ZLIB)
- **Fixed position conflicts**: Check for overlapping fixed positions or coordinates outside atlas bounds
- **Fixed position validation**: Ensure fixed sprites don't exceed atlas dimensions and don't overlap
- **Sprite-to-input mapping**: Verify `FindInputForSprite()` correctly matches sprite names to input configurations

When working on this project, focus on:
1. Understanding the content pipeline architecture before making changes
2. Testing with various asset types and sizes (PNG sprites, sprite sheets, TMX maps)
3. Maintaining compatibility with MonoGame standards
4. Optimizing for both build-time performance and runtime efficiency
5. Preserving TMX map data integrity and MonoGame format compatibility
6. Ensuring prefix-based naming logic handles edge cases properly
7. Testing fixed positioning with different sprite combinations and priority values
8. Validating that fixed sprites don't interfere with dynamic packing efficiency