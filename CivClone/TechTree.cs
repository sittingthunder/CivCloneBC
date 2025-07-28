using System.Collections.Generic;

namespace CivClone
{
    /// <summary>
    /// Manages the technology tree.  Tracks available
    /// technologies, research progress and determines which
    /// technologies are currently researchable.  Only one
    /// technology may be researched at a time, following
    /// Civilization I's design【470431666070268†L302-L315】.
    /// </summary>
    public class TechTree
    {
        private readonly Dictionary<string, Technology> _technologies = new();
        private Technology? _currentResearch;
        private int _researchAccumulated;

        public TechTree()
        {
            // Initialize basic technologies.  Costs and
            // dependencies loosely follow Civilization I.
            AddTechnology(new Technology("Pottery", 20));
            AddTechnology(new Technology("Wheel", 30));
            AddTechnology(new Technology("Alphabet", 30));
            AddTechnology(new Technology("Bronze Working", 40));
            AddTechnology(new Technology("Ceremonial Burial", 30));
            AddTechnology(new Technology("Horseback Riding", 40, "Wheel"));
            AddTechnology(new Technology("Iron Working", 50, "Bronze Working"));
            AddTechnology(new Technology("Mathematics", 60, "Alphabet", "Bronze Working"));
            // Additional technologies would be added here.
        }

        private void AddTechnology(Technology tech)
        {
            _technologies[tech.Name] = tech;
        }

        /// <summary>
        /// Returns the name of the technology currently being researched,
        /// or null if none is selected.
        /// </summary>
        public string? GetCurrentResearch()
        {
            return _currentResearch?.Name;
        }

        /// <summary>
        /// Returns true if the named technology has been researched.
        /// </summary>
        public bool IsResearched(string name) => _technologies.TryGetValue(name, out var tech) && tech.Researched;

        /// <summary>
        /// Begins researching the specified technology.  Returns
        /// false if the technology does not exist, is already
        /// researched or prerequisites are not met.
        /// </summary>
        public bool BeginResearch(string name)
        {
            if (!_technologies.TryGetValue(name, out var tech)) return false;
            if (tech.Researched) return false;
            foreach (var prereq in tech.Prerequisites)
            {
                if (!IsResearched(prereq)) return false;
            }
            _currentResearch = tech;
            _researchAccumulated = 0;
            return true;
        }

        /// <summary>
        /// Adds research points towards the current technology.  When
        /// the cost is met or exceeded the technology is marked as
        /// researched and the current research is cleared.
        /// </summary>
        public void AddResearchPoints(int points)
        {
            if (_currentResearch == null) return;
            _researchAccumulated += points;
            if (_researchAccumulated >= _currentResearch.Cost)
            {
                _currentResearch.Researched = true;
                _currentResearch = null;
                _researchAccumulated = 0;
            }
        }

        /// <summary>
        /// Lists all technologies that can currently be researched.
        /// A technology is researchable if it is not yet researched
        /// and all of its prerequisites are researched.
        /// </summary>
        public IEnumerable<string> GetAvailableTechnologies()
        {
            foreach (var tech in _technologies.Values)
            {
                if (tech.Researched) continue;
                bool prerequisitesMet = true;
                foreach (var prereq in tech.Prerequisites)
                {
                    if (!IsResearched(prereq)) { prerequisitesMet = false; break; }
                }
                if (prerequisitesMet) yield return tech.Name;
            }
        }
    }
}