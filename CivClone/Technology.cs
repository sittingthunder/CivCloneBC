using System.Collections.Generic;

namespace CivClone
{
    /// <summary>
    /// Represents a single technology in the technology tree.  Each
    /// technology has a name, a cost in research points and a list
    /// of prerequisite technologies.  When researched, it unlocks
    /// units, improvements or wonders.  The benefits are stored
    /// externally (e.g., in the TechTree or BuildItem definitions).
    /// </summary>
    public class Technology
    {
        public string Name { get; }
        public int Cost { get; }
        public List<string> Prerequisites { get; } = new();
        public bool Researched { get; set; }

        public Technology(string name, int cost, params string[] prerequisites)
        {
            Name = name;
            Cost = cost;
            Prerequisites.AddRange(prerequisites);
            Researched = false;
        }
    }
}