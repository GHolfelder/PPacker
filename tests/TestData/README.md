# Test Data

This directory contains test assets and configurations used for PPacker testing.

## Files

### Configuration Files
- `aseprite-config.json` - Test configuration for Aseprite JSON format processing
- `aseprite-data.json` - Sample Aseprite JSON export data for testing
- `basic-config.json` - Basic test configuration with multiple sprite inputs

### Assets
- `player/walk/walk_E.png` - Sample sprite image for testing sprite processing

## Usage

These files are used by the test suite and for manual testing during development. 

To test with these configurations:

```bash
# From project root
dotnet run -- --config tests/TestData/aseprite-config.json --verbose
dotnet run -- --config tests/TestData/basic-config.json --verbose
```

## Test Output

Test output files are generated in `test-output/` relative to the configuration file location.