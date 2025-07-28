using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CivClone
{
    /// <summary>
    /// Represents a city founded by the player or AI.  Cities
    /// consume surrounding tiles to produce food, trade and
    /// production.  Cities can build units, improvements and
    /// wonders, and they grow over time as food surpluses are
    /// accumulated.
    /// </summary>
    public class City
    {
        public string Name { get; set; }
        public Point Location { get; }
        public int Population { get; private set; } = 1;
        public Dictionary<string, bool> Improvements { get; } = new();
        public Queue<BuildItem> BuildQueue { get; } = new();

        // Base yields from city centre; additional yields come from
        // surrounding tiles.  Yields are tracked per turn.
        public int FoodYield { get; private set; }
        public int TradeYield { get; private set; }
        public int ProductionYield { get; private set; }

        // Accumulated food for growth and a simple growth cost function.
        private int FoodStock { get; set; }
        private int GrowthCost => (Population + 1) * 20;

        /// <summary>
        /// Returns the item currently being built or null if none.
        /// </summary>
        public BuildItem? CurrentBuildItem => BuildQueue.Count > 0 ? BuildQueue.Peek() : null;

        public City(string name, Point location)
        {
            Name = name;
            Location = location;
        }

        /// <summary>
        /// Calculates resource yields based on surrounding terrain and
        /// improvements.  In Civilization I, citizens worked tiles
        /// within a radius of 2.  For simplicity, this example
        /// assumes all adjacent tiles are worked.  Adjust as needed.
        /// </summary>
        public void UpdateYields(Map map)
        {
            FoodYield = 2;       // base food from city square
            TradeYield = 1;      // base trade
            ProductionYield = 1; // base production

            // Work tiles around the city.  Real rules would
            // assign citizens to specific tiles.
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int tx = Location.X + dx;
                    int ty = Location.Y + dy;
                    if (tx >= 0 && ty >= 0 && tx < map.Width && ty < map.Height)
                    {
                        var tile = map[tx, ty];
                        // Simplified yields: forest yields extra
                        // production, hills yield production, grassland
                        // yields food, etc.
                        switch (tile.Terrain)
                        {
                            case TerrainType.Grassland:
                                FoodYield += 2;
                                break;
                            case TerrainType.Plains:
                                FoodYield += 1;
                                ProductionYield += 1;
                                break;
                            case TerrainType.Forest:
                                ProductionYield += 2;
                                break;
                            case TerrainType.Hills:
                                ProductionYield += 2;
                                TradeYield += 1;
                                break;
                            case TerrainType.Mountain:
                                ProductionYield += 3;
                                break;
                            case TerrainType.Desert:
                                TradeYield += 2;
                                break;
                            case TerrainType.Ocean:
                                TradeYield += 1;
                                FoodYield += 1;
                                break;
                        }
                    }
                }
            }

            // Apply modifiers from improvements.  Example: a granary
            // increases food yield by 50%.
            if (Improvements.ContainsKey("Granary"))
            {
                FoodYield = (int)(FoodYield * 1.5f);
            }
        }

        /// <summary>
        /// Processes production at the end of a turn.  Production is
        /// applied to the item at the front of the build queue.
        /// Once the item is complete, it is removed and the next
        /// item begins construction.
        /// </summary>
        public void ProcessProduction()
        {
            if (BuildQueue.Count == 0) return;
            var current = BuildQueue.Peek();
            current.Progress += ProductionYield;
            if (current.Progress >= current.Cost)
            {
                // Item completed.  Apply its effect.
                BuildQueue.Dequeue();
                ApplyBuildItem(current);
            }
        }

        private void ApplyBuildItem(BuildItem item)
        {
            switch (item.Type)
            {
                case BuildItemType.Unit:
                    // Add unit to global unit list.  This would
                    // require access to the game world; omitted for
                    // brevity.
                    break;
                case BuildItemType.Improvement:
                    Improvements[item.Name] = true;
                    break;
                case BuildItemType.Wonder:
                    // Wonders are unique; ensure only one is built
                    // globally.  Global wonder tracking would occur
                    // elsewhere.
                    Improvements[item.Name] = true;
                    break;
            }
        }

        /// <summary>
        /// Processes food accumulation and population growth at the
        /// end of the turn.  Food surplus is added to the food
        /// stock.  When FoodStock exceeds GrowthCost, population
        /// increases and the stock is reduced.
        /// </summary>
        public void ProcessGrowth()
        {
            int foodNeeded = Population * 2;
            int surplus = FoodYield - foodNeeded;
            if (surplus > 0)
            {
                FoodStock += surplus;
                if (FoodStock >= GrowthCost)
                {
                    Population++;
                    FoodStock -= GrowthCost;
                }
            }
            else
            {
                // Starvation: lose population if food deficit
                if (FoodYield < foodNeeded)
                {
                    Population = Math.Max(1, Population - 1);
                    FoodStock = 0;
                }
            }
        }

        /// <summary>
        /// Enqueues a build item at the end of the queue.
        /// </summary>
        public void EnqueueBuildItem(BuildItem item)
        {
            BuildQueue.Enqueue(item);
        }
    }

    /// <summary>
    /// Represents an item being built in a city.  It could be a
    /// military unit, a city improvement (e.g., granary, temple) or
    /// a wonder.  Cost reflects production required.
    /// </summary>
    public class BuildItem
    {
        public string Name { get; }
        public BuildItemType Type { get; }
        public int Cost { get; }
        public int Progress { get; set; }

        public BuildItem(string name, BuildItemType type, int cost)
        {
            Name = name;
            Type = type;
            Cost = cost;
            Progress = 0;
        }
    }

    public enum BuildItemType
    {
        Unit,
        Improvement,
        Wonder
    }
}