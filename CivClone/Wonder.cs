using System.Collections.Generic;

namespace CivClone
{
    /// <summary>
    /// Represents a unique world wonder.  Wonders confer
    /// powerful bonuses and can only be built once.  The
    /// implementation of wonder effects lies elsewhere in the
    /// game logic; this class simply defines their existence and
    /// prerequisites.
    /// </summary>
    public class Wonder
    {
        public string Name { get; }
        public int Cost { get; }
        public List<string> PrerequisiteTechnologies { get; } = new();
        public bool Built { get; set; }

        public Wonder(string name, int cost, params string[] prerequisites)
        {
            Name = name;
            Cost = cost;
            PrerequisiteTechnologies.AddRange(prerequisites);
            Built = false;
        }
    }
}