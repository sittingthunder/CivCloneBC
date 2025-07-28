using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CivClone
{
    /// <summary>
    /// Represents the world map.  The map is a two‑dimensional array of
    /// tiles.  Each tile stores terrain type and yields.  The Map
    /// class also handles terrain generation and drawing.
    /// </summary>
    public class Map
    {
        public const int TileSize = 16;
        private readonly int _width;
        private readonly int _height;
        private readonly Tile[,] _tiles;

        public int Width => _width;
        public int Height => _height;

        public Map(int width, int height)
        {
            _width = width;
            _height = height;
            _tiles = new Tile[width, height];
            GenerateTerrain();
        }

        /// <summary>
        /// Indexer to get or set a tile at the specified position.
        /// Allows code like map[x, y] to retrieve a Tile instance.
        /// </summary>
        public Tile this[int x, int y]
        {
            get => _tiles[x, y];
            set => _tiles[x, y] = value;
        }

        /// <summary>
        /// Generates a simple random terrain.  In a full game this
        /// would use noise functions and climate rules to create
        /// realistic land masses and resource distribution.
        /// </summary>
        private void GenerateTerrain()
        {
            var rand = new Random();
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    // Simple noise: choose ocean on the edges and
                    // random land types in the interior.
                    TerrainType type;
                    if (x < 5 || y < 5 || x > _width - 6 || y > _height - 6)
                    {
                        type = TerrainType.Ocean;
                    }
                    else
                    {
                        var val = rand.NextDouble();
                        if (val < 0.05) type = TerrainType.Mountain;
                        else if (val < 0.15) type = TerrainType.Hills;
                        else if (val < 0.35) type = TerrainType.Forest;
                        else if (val < 0.55) type = TerrainType.Plains;
                        else type = TerrainType.Grassland;
                    }
                    _tiles[x, y] = new Tile(type);
                }
            }
        }

        /// <summary>
        /// Draws the map to the screen.  Each tile is drawn as a
        /// filled rectangle with a colour based on terrain type.
        /// MonoGame's SpriteBatch is used for drawing.  In a full
        /// implementation you would draw from a tileset texture.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D terrainTexture)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var tile = _tiles[x, y];
                    var rect = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                    spriteBatch.Draw(terrainTexture, rect, tile.Colour);
                }
            }
        }
    }

    /// <summary>
    /// Terrain types roughly based on Civilization I.  Each type
    /// corresponds to different yields and movement costs.  In the
    /// original game, yields include food, trade and production.
    /// </summary>
    public enum TerrainType
    {
        Grassland,
        Plains,
        Forest,
        Hills,
        Mountain,
        Desert,
        Tundra,
        Arctic,
        Ocean,
    }

    /// <summary>
    /// Represents a single map tile.  Stores terrain type and
    /// provides properties for colour and yields.  You could expand
    /// this class to include improvements (roads, railroads),
    /// resources and fog‑of‑war visibility.
    /// </summary>
    public class Tile
    {
        public TerrainType Terrain { get; }

        public Tile(TerrainType terrain)
        {
            Terrain = terrain;
        }

        /// <summary>
        /// Returns a colour for the terrain.  This is a helper
        /// property used when drawing the map with coloured
        /// rectangles.  Replace with actual sprites when using a
        /// tileset.
        /// </summary>
        public Color Colour => Terrain switch
        {
            TerrainType.Grassland => Color.ForestGreen,
            TerrainType.Plains => Color.PaleGoldenrod,
            TerrainType.Forest => Color.DarkGreen,
            TerrainType.Hills => Color.SaddleBrown,
            TerrainType.Mountain => Color.LightGray,
            TerrainType.Desert => Color.Khaki,
            TerrainType.Tundra => Color.LightSeaGreen,
            TerrainType.Arctic => Color.White,
            TerrainType.Ocean => Color.CornflowerBlue,
            _ => Color.Magenta
        };

        // Placeholder yields; in a full implementation you would
        // assign food, production and trade values based on terrain
        // type and improvements.  Example: grassland yields 2 food,
        // 1 trade, 0 production by default.
    }
}