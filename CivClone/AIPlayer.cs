using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CivClone
{
    /// <summary>
    /// Basic AI player implementation.  AI players control
    /// civilizations and make decisions about exploring, building
    /// cities, researching technologies and conducting diplomacy.
    /// This class provides a simple structure for managing AI
    /// behaviour; the actual algorithms should be implemented
    /// incrementally.
    /// </summary>
    public class AIPlayer
    {
        public string Name { get; }
        public Government Government { get; } = new();
        public TechTree TechTree { get; } = new();
        public List<Unit> Units { get; } = new();
        public List<City> Cities { get; } = new();

        private readonly Random _rand = new();

        public AIPlayer(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Executes a single AI turn.  In a simple version, the AI
        /// will move units randomly, found cities when settlers
        /// exist, and choose a technology to research.  More
        /// sophisticated behaviour can be added later.
        /// </summary>
        public void TakeTurn(Map map)
        {
            // Choose technology if none selected.
            if (TechTree.GetAvailableTechnologies() is var techs && techs != null)
            {
                foreach (var tech in techs)
                {
                    if (TechTree.BeginResearch(tech)) break;
                }
            }

            // Move units.  For settlers, attempt to found a city
            // on suitable terrain; for others, explore randomly.
            foreach (var unit in Units)
            {
                if (unit.Type == UnitType.Settler)
                {
                    // If not adjacent to a city, found one.
                    bool nearCity = false;
                    foreach (var city in Cities)
                    {
                        if (Math.Abs(city.Location.X - unit.Position.X) + Math.Abs(city.Location.Y - unit.Position.Y) <= 2)
                        {
                            nearCity = true;
                            break;
                        }
                    }
                    if (!nearCity)
                    {
                        // Found city
                        Cities.Add(new City("AI City", unit.Position));
                        // Convert settler into a worker; remove unit.
                        Units.Remove(unit);
                        break;
                    }
                }
                else
                {
                    // Random walk
                    var dx = _rand.Next(-1, 2);
                    var dy = _rand.Next(-1, 2);
                    var newPos = new Point(unit.Position.X + dx, unit.Position.Y + dy);
                    // Stay within map bounds
                    if (newPos.X >= 0 && newPos.Y >= 0 && newPos.X < map.Width && newPos.Y < map.Height)
                    {
                        unit.Move(newPos);
                    }
                }
            }
        }
    }
}