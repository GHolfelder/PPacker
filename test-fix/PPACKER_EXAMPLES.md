# PPacker Example Configuration

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
