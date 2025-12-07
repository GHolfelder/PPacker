using PPacker.Core;
using System;
using System.Collections.Generic;

namespace TestBinPacker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing bin packer boundary logic...");
            
            // Test case: 512x512 atlas with 32x32 sprites
            var packer = new BinPacker(512, 512, 0, false); // no padding for simplicity
            var rectangles = new List<PackingRectangle>();
            
            // Create enough 32x32 sprites to fill exactly one row
            // 512 / 32 = 16 sprites should fit exactly in one row
            for (int i = 0; i < 16; i++)
            {
                rectangles.Add(new PackingRectangle(32, 32, $"sprite_{i}"));
            }
            
            var result = packer.Pack(rectangles);
            
            if (result != null)
            {
                Console.WriteLine($"Packed {result.PackedRectangles.Count} sprites");
                Console.WriteLine($"Atlas size: {result.ActualWidth}x{result.ActualHeight}");
                
                // Check first row
                var firstRow = result.PackedRectangles.Where(r => r.Y == 0).OrderBy(r => r.X).ToList();
                Console.WriteLine($"First row sprites: {firstRow.Count}");
                
                foreach (var sprite in firstRow)
                {
                    Console.WriteLine($"  {sprite.Name}: x={sprite.X}, y={sprite.Y}, w={sprite.Width}, h={sprite.Height}, right_edge={sprite.X + sprite.Width}");
                    
                    if (sprite.X + sprite.Width > 512)
                    {
                        Console.WriteLine($"  ERROR: Sprite {sprite.Name} extends beyond atlas boundary!");
                    }
                }
            }
            else
            {
                Console.WriteLine("Packing failed!");
            }
        }
    }
}