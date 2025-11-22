# Tiled Map Example

This directory contains a simple example of using PPacker with Tiled map files.

## Files

- `simple_map.tmx` - A simple 8x6 tile map with multiple layers
- `grass_tileset.tsx` - A tileset with 9 tiles (32x32 each) in a 3x3 grid
- `tiled-config.json` - PPacker configuration for processing the map
- `grass_tiles.png` - Required tileset image (see below)

## Creating the Tileset Image

You need to create a `grass_tiles.png` image file with the following specifications:

- Dimensions: 96x96 pixels (3 columns × 3 rows of 32x32 tiles)
- Format: PNG with transparency support
- Content: A simple 3x3 grid of grass/stone/water tile textures

### Suggested tile layout:
```
[Grass] [Flower] [Stone]
[Grass] [Stone]  [Water]
[Water] [Water]  [Water]
```

## Running the Example

1. Create the required `grass_tiles.png` image file
2. Run PPacker from the examples/tiled-map directory:

```bash
dotnet run --project ../../src/PPacker -- --config tiled-config.json --verbose
```

## Expected Output

The packing process will:

1. Parse the TMX map file and load the referenced TSX tileset
2. Extract the `grass_tiles.png` image and add it to the atlas as `map_grass_tiles`
3. Generate three output files:
   - `output/atlas.png` - The packed texture atlas
   - `output/atlas.json` - Sprite data for the atlas
   - `output/map.json` - Map data for MonoGame consumption

## Map Data Structure

The generated `map.json` will contain:

- Map dimensions and properties
- Tileset information with atlas sprite references
- Layer data for "Ground" and "Water" tile layers
- Object data for spawn points, chests, collision boxes, and paths
- All custom properties from the Tiled map

This data can be consumed by MonoGame libraries for tile-based game rendering.