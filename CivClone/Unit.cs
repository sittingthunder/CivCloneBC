using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CivClone
{
    /// <summary>
    /// Enumeration of unit types roughly based on Civilization I.
    /// Each unit type defines its movement allowance, combat
    /// strength and special abilities.  Additional properties such
    /// as cost and prerequisite technologies could be added here.
    /// </summary>
    public enum UnitType
    {
        Settler,
        Warrior,
        Phalanx,
        Archer,
        Chariot,
        Horseman,
        Legion,
        Catapult,
        Knight,
        Crusader,
        Pikeman,
        Musketeer,
        Rifleman,
        Cavalry,
        Cannon,
        Artillery,
        Tank,
        Battleship,
        Cruiser,
        Carrier,
        Trireme,
        Caravel,
        Galleon,
        Transport,
        Submarine,
        Fighter,
        Bomber
    }

    /// <summary>
    /// Represents a unit on the map.  Units have a type, a tile
    /// position and remaining movement points for the current turn.
    /// </summary>
    public class Unit
    {
        public UnitType Type { get; }
        public Point Position { get; set; }
        public int MovementPoints { get; private set; }
        public int Attack { get; }
        public int Defense { get; }

        public Unit(UnitType type, Point position)
        {
            Type = type;
            Position = position;

            // Assign default stats based on type.  Real values
            // should match Civilization I.  MovementPoints
            // represents how many tiles the unit can move per turn.
            (MovementPoints, Attack, Defense) = type switch
            {
                UnitType.Settler => (1, 0, 1),
                UnitType.Warrior => (1, 1, 1),
                UnitType.Phalanx => (1, 1, 2),
                UnitType.Archer => (1, 2, 1),
                UnitType.Chariot => (2, 3, 1),
                UnitType.Horseman => (2, 2, 1),
                UnitType.Legion => (1, 3, 1),
                UnitType.Catapult => (1, 4, 1),
                UnitType.Knight => (2, 4, 2),
                UnitType.Crusader => (2, 5, 1),
                UnitType.Pikeman => (1, 1, 2),
                UnitType.Musketeer => (1, 3, 2),
                UnitType.Rifleman => (1, 4, 2),
                UnitType.Cavalry => (2, 6, 3),
                UnitType.Cannon => (1, 6, 2),
                UnitType.Artillery => (1, 8, 2),
                UnitType.Tank => (2, 10, 5),
                UnitType.Battleship => (4, 18, 12),
                UnitType.Cruiser => (5, 12, 6),
                UnitType.Carrier => (4, 1, 9),
                UnitType.Trireme => (3, 1, 1),
                UnitType.Caravel => (3, 1, 1),
                UnitType.Galleon => (3, 1, 1),
                UnitType.Transport => (3, 0, 3),
                UnitType.Submarine => (4, 10, 5),
                UnitType.Fighter => (8, 4, 1),
                UnitType.Bomber => (8, 12, 1),
                _ => (1, 1, 1)
            };
        }

        /// <summary>
        /// Resets the movement points at the start of a new turn.
        /// </summary>
        public void ResetMovement()
        {
            // Movement allowance is determined by unit type.
            MovementPoints = Type switch
            {
                UnitType.Settler => 1,
                UnitType.Warrior => 1,
                UnitType.Phalanx => 1,
                UnitType.Archer => 1,
                UnitType.Chariot => 2,
                UnitType.Horseman => 2,
                UnitType.Legion => 1,
                UnitType.Catapult => 1,
                UnitType.Knight => 2,
                UnitType.Crusader => 2,
                UnitType.Pikeman => 1,
                UnitType.Musketeer => 1,
                UnitType.Rifleman => 1,
                UnitType.Cavalry => 2,
                UnitType.Cannon => 1,
                UnitType.Artillery => 1,
                UnitType.Tank => 2,
                UnitType.Battleship => 4,
                UnitType.Cruiser => 5,
                UnitType.Carrier => 4,
                UnitType.Trireme => 3,
                UnitType.Caravel => 3,
                UnitType.Galleon => 3,
                UnitType.Transport => 3,
                UnitType.Submarine => 4,
                UnitType.Fighter => 8,
                UnitType.Bomber => 8,
                _ => 1
            };
        }

        /// <summary>
        /// Moves the unit to a new position and consumes movement points.
        /// In a full implementation this would involve pathfinding,
        /// terrain cost, and combat checks.
        /// </summary>
        public void Move(Point destination)
        {
            // Compute Manhattan distance as a simple cost estimate.
            var distance = Math.Abs(destination.X - Position.X) + Math.Abs(destination.Y - Position.Y);
            if (distance <= MovementPoints)
            {
                Position = destination;
                MovementPoints -= distance;
            }
        }
    }
}