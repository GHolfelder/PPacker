# Build and run the project
dotnet build
dotnet run -- --config examples/sample-config.json --verbose

# Run tests
dotnet test

# Generate example configuration
dotnet run -- example --output ./test-examples

# Build release version
dotnet build -c Release

# Create NuGet package
dotnet pack -c Release

# Install as global tool (after packing)
dotnet tool install -g --add-source ./bin/Release PPacker