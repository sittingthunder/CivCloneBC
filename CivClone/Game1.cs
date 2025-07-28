using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CivClone
{
    /// <summary>
    /// The main game class for the Civilization I clone.  This class
    /// manages the MonoGame game loop, draws the world map, and
    /// dispatches user input to the appropriate subsystems.  Most of
    /// the game logic is encapsulated in helper classes such as
    /// Map, Unit, City and TechTree.
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch = default!;
        private Map _map = default!;
        private List<Unit> _units = new();
        private List<City> _cities = new();
        private TechTree _techTree = new();

        // AI players
        private List<AIPlayer> _aiPlayers = new();

        // Basic textures for tiles, units and cities.  In a full
        // implementation these would be loaded from content.
        private Texture2D _terrainTexture = default!;
        private Texture2D _unitTexture = default!;
        private Texture2D _cityTexture = default!;

        // Game state
        private int _currentTurn = 0;
        private bool _playerTurn = true;
        private int _selectedUnitIndex = 0;

        // Screen state and simple menu
        private enum GameState { MainMenu, Playing }
        private GameState _state = GameState.MainMenu;
        private Menu _mainMenu = new Menu(new[] { "Start Game", "Exit" });
        private SpriteFont? _font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Create a simple random map on start.  The Map
            // constructor handles terrain generation and tile
            // creation.
            _map = new Map(width: 80, height: 50);

            // Create a starting settler unit for the player.
            var settler = new Unit(UnitType.Settler, new Point(_map.Width / 2, _map.Height / 2));
            _units.Add(settler);

            // Create AI player with a settler far from the player
            var ai = new AIPlayer("AI");
            ai.Units.Add(new Unit(UnitType.Settler, new Point(5, 5)));
            _aiPlayers.Add(ai);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Placeholder textures.  These should be replaced with
            // proper tileset and unit sprites loaded via the
            // ContentManager.  A single white pixel is scaled during
            // drawing to represent a tile.
            _terrainTexture = new Texture2D(GraphicsDevice, 1, 1);
            _terrainTexture.SetData(new[] { Color.White });

            _unitTexture = new Texture2D(GraphicsDevice, 1, 1);
            _unitTexture.SetData(new[] { Color.Blue });

            _cityTexture = new Texture2D(GraphicsDevice, 1, 1);
            _cityTexture.SetData(new[] { Color.Red });

            try
            {
                // Attempt to load a font named "DefaultFont".  This
                // requires a .spritefont file processed by the MonoGame
                // Content Pipeline.  If not available the game will
                // continue without on‑screen text.  See README for
                // instructions on adding fonts.
                _font = Content.Load<SpriteFont>("DefaultFont");
            }
            catch
            {
                _font = null;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboard = Keyboard.GetState();

            switch (_state)
            {
                case GameState.MainMenu:
                    // Navigate menu
                    if (keyboard.IsKeyDown(Keys.Up)) _mainMenu.MoveUp();
                    if (keyboard.IsKeyDown(Keys.Down)) _mainMenu.MoveDown();
                    if (keyboard.IsKeyDown(Keys.Enter))
                    {
                        var selection = _mainMenu.SelectedOption;
                        if (selection == "Start Game")
                        {
                            _state = GameState.Playing;
                        }
                        else if (selection == "Exit")
                        {
                            Exit();
                        }
                    }
                    break;
                case GameState.Playing:
                    if (_playerTurn && _units.Count > 0)
                    {
                        var unit = _units[_selectedUnitIndex];
                        // Movement keys
                        if (keyboard.IsKeyDown(Keys.Left) && unit.MovementPoints > 0)
                            unit.Move(new Point(Math.Max(0, unit.Position.X - 1), unit.Position.Y));
                        if (keyboard.IsKeyDown(Keys.Right) && unit.MovementPoints > 0)
                            unit.Move(new Point(Math.Min(_map.Width - 1, unit.Position.X + 1), unit.Position.Y));
                        if (keyboard.IsKeyDown(Keys.Up) && unit.MovementPoints > 0)
                            unit.Move(new Point(unit.Position.X, Math.Max(0, unit.Position.Y - 1)));
                        if (keyboard.IsKeyDown(Keys.Down) && unit.MovementPoints > 0)
                            unit.Move(new Point(unit.Position.X, Math.Min(_map.Height - 1, unit.Position.Y + 1)));

                        // Found city: press F
                        if (keyboard.IsKeyDown(Keys.F))
                        {
                            if (unit.Type == UnitType.Settler)
                            {
                                bool occupied = false;
                                foreach (var city in _cities)
                                {
                                    if (city.Location == unit.Position)
                                    {
                                        occupied = true;
                                        break;
                                    }
                                }
                                if (!occupied)
                                {
                                    var city = new City("City" + (_cities.Count + 1), unit.Position);
                                    _cities.Add(city);
                                    _units.RemoveAt(_selectedUnitIndex);
                                    if (_selectedUnitIndex >= _units.Count)
                                        _selectedUnitIndex = Math.Max(0, _units.Count - 1);
                                }
                            }
                        }

                        // Cycle selected unit: press Tab
                        if (keyboard.IsKeyDown(Keys.Tab) && _units.Count > 0)
                        {
                            _selectedUnitIndex = (_selectedUnitIndex + 1) % _units.Count;
                        }

                        // End turn: press Enter
                        if (keyboard.IsKeyDown(Keys.Enter))
                        {
                            EndTurn();
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            if (_state == GameState.MainMenu)
            {
                // Draw menu centered on the screen
                _mainMenu.Draw(_spriteBatch, _font, new Vector2(100, 100));
            }
            else
            {
                // Draw the map
                _map.Draw(_spriteBatch, _terrainTexture);
                // Draw units
                foreach (var unit in _units)
                {
                    var rect = new Rectangle(unit.Position.X * Map.TileSize,
                                             unit.Position.Y * Map.TileSize,
                                             Map.TileSize, Map.TileSize);
                    _spriteBatch.Draw(_unitTexture, rect, Color.Blue);
                }
                // Draw cities
                foreach (var city in _cities)
                {
                    var rect = new Rectangle(city.Location.X * Map.TileSize,
                                             city.Location.Y * Map.TileSize,
                                             Map.TileSize, Map.TileSize);
                    _spriteBatch.Draw(_cityTexture, rect, Color.Red);
                }
                // Draw on‑screen information (turn number and research)
                if (_font != null)
                {
                    // Draw turn number
                    string info = $"Turn: {_currentTurn}";
                    var pos = new Vector2(10, GraphicsDevice.Viewport.Height - 40);
                    _spriteBatch.DrawString(_font, info, pos, Color.White);
                    // Draw current research
                    string? currentResearch = _techTree.GetCurrentResearch();
                    string researchInfo = currentResearch != null ? $"Research: {currentResearch}" : "Research: None";
                    var pos2 = new Vector2(10, GraphicsDevice.Viewport.Height - 20);
                    _spriteBatch.DrawString(_font, researchInfo, pos2, Color.White);
                }
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Ends the player's turn, processes city production,
        /// research and growth, runs AI turns and resets unit
        /// movement points.  This is a simplified turn system.
        /// </summary>
        private void EndTurn()
        {
            // Process player's cities
            int totalTrade = 0;
            foreach (var city in _cities)
            {
                city.UpdateYields(_map);
                totalTrade += city.TradeYield;
                city.ProcessProduction();
                city.ProcessGrowth();
            }
            // Add research points and begin a new tech if none
            _techTree.AddResearchPoints(totalTrade);
            if (_techTree.GetCurrentResearch() == null)
            {
                foreach (var techName in _techTree.GetAvailableTechnologies())
                {
                    if (_techTree.BeginResearch(techName)) break;
                }
            }
            // Reset unit movement
            foreach (var unit in _units)
            {
                unit.ResetMovement();
            }
            // Switch to AI turn
            _playerTurn = false;
            // AI players take their turns
            foreach (var ai in _aiPlayers)
            {
                // AI research: accumulate trade from its cities
                int aiTrade = 0;
                foreach (var c in ai.Cities)
                {
                    c.UpdateYields(_map);
                    aiTrade += c.TradeYield;
                    c.ProcessProduction();
                    c.ProcessGrowth();
                }
                ai.TechTree.AddResearchPoints(aiTrade);
                ai.TakeTurn(_map);
            }
            // Reset AI units movement
            foreach (var ai in _aiPlayers)
            {
                foreach (var u in ai.Units)
                {
                    u.ResetMovement();
                }
            }
            // Back to player
            _playerTurn = true;
            _currentTurn++;
        }
    }
}