using PPacker.Core;
using PPacker.Models;
using System.IO;
using Xunit;

namespace PPacker.Tests.Core
{
    public class TiledMapProcessorTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly string _testTmxPath;
        private readonly string _testTsxPath;

        public TiledMapProcessorTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "PPacker_Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
            
            _testTmxPath = Path.Combine(_tempDir, "test.tmx");
            _testTsxPath = Path.Combine(_tempDir, "test.tsx");
            
            CreateTestFiles();
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        private void CreateTestFiles()
        {
            // Create a simple TSX file
            var tsxContent = """
                <?xml version="1.0" encoding="UTF-8"?>
                <tileset version="1.10" tiledversion="1.10.2" name="test_tileset" tilewidth="32" tileheight="32" tilecount="4" columns="2">
                 <properties>
                  <property name="description" value="Test tileset"/>
                 </properties>
                 <image source="test_tiles.png" width="64" height="64"/>
                 <tile id="0">
                  <properties>
                   <property name="type" value="grass"/>
                  </properties>
                 </tile>
                 <tile id="3">
                  <animation>
                   <frame tileid="3" duration="250"/>
                   <frame tileid="2" duration="250"/>
                  </animation>
                 </tile>
                </tileset>
                """;
            
            File.WriteAllText(_testTsxPath, tsxContent);

            // Create a simple TMX file
            var tmxContent = $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <map version="1.10" tiledversion="1.10.2" orientation="orthogonal" renderorder="right-down" width="4" height="3" tilewidth="32" tileheight="32" infinite="0" nextlayerid="3" nextobjectid="2">
                 <properties>
                  <property name="author" value="Test"/>
                 </properties>
                 <tileset firstgid="1" source="test.tsx"/>
                 <layer id="1" name="TestLayer" width="4" height="3">
                  <data encoding="csv">
                1,2,3,4,
                2,3,4,1,
                3,4,1,2
                </data>
                 </layer>
                 <objectgroup id="2" name="TestObjects">
                  <object id="1" name="test_object" type="TestType" x="64" y="32" width="32" height="32">
                   <properties>
                    <property name="test_prop" value="test_value"/>
                   </properties>
                  </object>
                 </objectgroup>
                </map>
                """;
            
            File.WriteAllText(_testTmxPath, tmxContent);
        }

        [Fact]
        public async Task LoadTilesetAsync_ShouldParseValidTsxFile()
        {
            // Act
            var tileset = await TiledMapProcessor.LoadTilesetAsync(_testTsxPath);

            // Assert
            Assert.NotNull(tileset);
            Assert.Equal("test_tileset", tileset.Name);
            Assert.Equal(32, tileset.TileWidth);
            Assert.Equal(32, tileset.TileHeight);
            Assert.Equal(4, tileset.TileCount);
            Assert.Equal(2, tileset.Columns);
            
            Assert.NotNull(tileset.Image);
            Assert.Equal("test_tiles.png", tileset.Image.Source);
            Assert.Equal(64, tileset.Image.Width);
            Assert.Equal(64, tileset.Image.Height);
            
            Assert.Single(tileset.Properties?.Properties ?? new List<TiledProperty>());
            Assert.Equal("description", tileset.Properties!.Properties[0].Name);
            Assert.Equal("Test tileset", tileset.Properties.Properties[0].Value);
        }

        [Fact]
        public async Task LoadMapAsync_ShouldParseValidTmxFile()
        {
            // Act
            var map = await TiledMapProcessor.LoadMapAsync(_testTmxPath);

            // Assert
            Assert.NotNull(map);
            Assert.Equal(4, map.Width);
            Assert.Equal(3, map.Height);
            Assert.Equal(32, map.TileWidth);
            Assert.Equal(32, map.TileHeight);
            Assert.Equal("orthogonal", map.Orientation);
            
            Assert.Single(map.Tilesets);
            var tileset = map.Tilesets[0];
            Assert.Equal(1, tileset.FirstGid);
            Assert.Equal("test_tileset", tileset.Name);
            
            Assert.Single(map.Layers);
            var layer = map.Layers[0];
            Assert.Equal("TestLayer", layer.Name);
            Assert.Equal(4, layer.Width);
            Assert.Equal(3, layer.Height);
            
            Assert.Single(map.ObjectGroups);
            var objectGroup = map.ObjectGroups[0];
            Assert.Equal("TestObjects", objectGroup.Name);
            Assert.Single(objectGroup.Objects);
            
            var obj = objectGroup.Objects[0];
            Assert.Equal("test_object", obj.Name);
            Assert.Equal("TestType", obj.Type);
            Assert.Equal(64, obj.X);
            Assert.Equal(32, obj.Y);
        }

        [Fact]
        public void ParseLayerData_ShouldParseCsvData()
        {
            // Arrange
            var layerData = new TiledLayerData
            {
                Encoding = "csv",
                Content = "1,2,3,4,5,6,7,8,9,10,11,12"
            };

            // Act
            var result = TiledMapProcessor.ParseLayerData(layerData, 4, 3);

            // Assert
            Assert.Equal(12, result.Length);
            Assert.Equal(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, result);
        }

        [Fact]
        public void ExtractImagePaths_ShouldFindTilesetImages()
        {
            // Arrange
            var map = new TiledMap
            {
                Tilesets = new List<TiledTilesetRef>
                {
                    new TiledTilesetRef
                    {
                        Image = new TiledImage { Source = "tileset1.png" }
                    },
                    new TiledTilesetRef
                    {
                        Image = new TiledImage { Source = "tileset2.png" },
                        Tiles = new List<TiledTile>
                        {
                            new TiledTile
                            {
                                Image = new TiledImage { Source = "tile_individual.png" }
                            }
                        }
                    }
                },
                ImageLayers = new List<TiledImageLayer>
                {
                    new TiledImageLayer
                    {
                        Image = new TiledImage { Source = "background.png" }
                    }
                }
            };

            // Act
            var imagePaths = TiledMapProcessor.ExtractImagePaths(map, _tempDir);

            // Assert
            Assert.Equal(4, imagePaths.Count);
            Assert.Contains(Path.Combine(_tempDir, "tileset1.png"), imagePaths);
            Assert.Contains(Path.Combine(_tempDir, "tileset2.png"), imagePaths);
            Assert.Contains(Path.Combine(_tempDir, "tile_individual.png"), imagePaths);
            Assert.Contains(Path.Combine(_tempDir, "background.png"), imagePaths);
        }

        [Fact]
        public void ConvertToMapData_ShouldCreateValidMapData()
        {
            // Arrange
            var map = new TiledMap
            {
                Width = 10,
                Height = 8,
                TileWidth = 32,
                TileHeight = 32,
                Orientation = "orthogonal",
                Tilesets = new List<TiledTilesetRef>
                {
                    new TiledTilesetRef
                    {
                        FirstGid = 1,
                        Name = "test_tileset",
                        TileWidth = 32,
                        TileHeight = 32,
                        Image = new TiledImage { Source = "test_tiles.png" }
                    }
                },
                Layers = new List<TiledLayer>
                {
                    new TiledLayer
                    {
                        Id = 1,
                        Name = "Ground",
                        Width = 10,
                        Height = 8,
                        Data = new TiledLayerData
                        {
                            Tiles = new List<TiledDataTile>
                            {
                                new TiledDataTile { Gid = 1 },
                                new TiledDataTile { Gid = 2 }
                            }
                        }
                    }
                }
            };

            var imageToSpriteMap = new Dictionary<string, string>
            {
                { "test_tiles", "sprite_test_tiles" }
            };

            // Act
            var mapData = TiledMapProcessor.ConvertToMapData(map, imageToSpriteMap, "atlas.png");

            // Assert
            Assert.NotNull(mapData);
            Assert.Equal(10, mapData.Width);
            Assert.Equal(8, mapData.Height);
            Assert.Equal(32, mapData.TileWidth);
            Assert.Equal(32, mapData.TileHeight);
            Assert.Equal("orthogonal", mapData.Orientation);
            Assert.Equal("atlas.png", mapData.AtlasFile);
            
            Assert.Single(mapData.Tilesets);
            var tileset = mapData.Tilesets[0];
            Assert.Equal("test_tileset", tileset.Name);
            Assert.Equal(1, tileset.FirstGid);
            Assert.Equal("sprite_test_tiles", tileset.AtlasSprite);
            
            Assert.Single(mapData.TileLayers);
            var layer = mapData.TileLayers[0];
            Assert.Equal("Ground", layer.Name);
            Assert.Equal(10, layer.Width);
            Assert.Equal(8, layer.Height);
        }

        [Fact]
        public async Task ConvertToMapData_ShouldCorrectlyDetectObjectTypes()
        {
            // Arrange
            var tmxContent = """
                <?xml version="1.0" encoding="UTF-8"?>
                <map version="1.10" tiledversion="1.11.2" orientation="orthogonal" renderorder="right-down" width="10" height="10" tilewidth="32" tileheight="32" infinite="0" nextlayerid="2" nextobjectid="8">
                 <objectgroup id="1" name="Test Objects">
                  <object id="1" name="Rectangle" type="collision" x="32" y="32" width="64" height="48"/>
                  <object id="2" name="Ellipse" type="area" x="128" y="32" width="64" height="48">
                   <ellipse/>
                  </object>
                  <object id="3" name="Circle" type="area" x="224" y="32" width="48" height="48">
                   <ellipse/>
                  </object>
                  <object id="4" name="Point" type="spawn" x="320" y="56">
                   <point/>
                  </object>
                  <object id="5" name="Polygon" type="platform" x="32" y="128" width="80" height="60">
                   <polygon points="0,0 80,0 60,30 20,60 0,60"/>
                  </object>
                  <object id="6" name="Polyline" type="path" x="128" y="128">
                   <polyline points="0,0 40,20 80,10 120,30"/>
                  </object>
                  <object id="7" name="Text" type="sign" x="224" y="128" width="96" height="32">
                   <text fontfamily="Arial" pixelsize="14" color="#ff0000" bold="1">Hello World!</text>
                  </object>
                 </objectgroup>
                </map>
                """;

            var testObjectTmxPath = Path.Combine(_tempDir, "object_test.tmx");
            await File.WriteAllTextAsync(testObjectTmxPath, tmxContent);

            // Act
            var map = await TiledMapProcessor.LoadMapAsync(testObjectTmxPath);
            var imageToSpriteMap = new Dictionary<string, string>();
            var mapData = TiledMapProcessor.ConvertToMapData(map, imageToSpriteMap, "test.png");

            // Assert
            Assert.Single(mapData.ObjectLayers);
            var objectLayer = mapData.ObjectLayers[0];
            Assert.Equal(7, objectLayer.Objects.Count);

            // Verify object types
            var rectangle = objectLayer.Objects.First(o => o.Name == "Rectangle");
            Assert.Equal("rectangle", rectangle.ObjectType);

            var ellipse = objectLayer.Objects.First(o => o.Name == "Ellipse");
            Assert.Equal("ellipse", ellipse.ObjectType);

            var circle = objectLayer.Objects.First(o => o.Name == "Circle");
            Assert.Equal("circle", circle.ObjectType);

            var point = objectLayer.Objects.First(o => o.Name == "Point");
            Assert.Equal("point", point.ObjectType);

            var polygon = objectLayer.Objects.First(o => o.Name == "Polygon");
            Assert.Equal("polygon", polygon.ObjectType);
            Assert.NotNull(polygon.Polygon);
            Assert.Equal(5, polygon.Polygon.Count);

            var polyline = objectLayer.Objects.First(o => o.Name == "Polyline");
            Assert.Equal("polyline", polyline.ObjectType);
            Assert.NotNull(polyline.Polyline);
            Assert.Equal(4, polyline.Polyline.Count);

            var text = objectLayer.Objects.First(o => o.Name == "Text");
            Assert.Equal("text", text.ObjectType);
            Assert.NotNull(text.Text);
            Assert.Equal("Hello World!", text.Text.Content);
            Assert.Equal("Arial", text.Text.FontFamily);
            Assert.True(text.Text.Bold);
        }

        [Fact]
        public async Task LoadMapAsync_ShouldThrowForNonExistentFile()
        {
            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                TiledMapProcessor.LoadMapAsync("nonexistent.tmx"));
        }

        [Fact]
        public async Task LoadTilesetAsync_ShouldThrowForNonExistentFile()
        {
            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                TiledMapProcessor.LoadTilesetAsync("nonexistent.tsx"));
        }
    }
}