namespace CivClone
{
    /// <summary>
    /// Enumeration of government types in Civilization I.  Each
    /// government affects corruption, production, trade and
    /// citizen happiness.  You could expand this type to include
    /// specific modifiers.
    /// </summary>
    public enum GovernmentType
    {
        Despotism,
        Monarchy,
        Republic,
        Democracy,
        Communism,
        Fundamentalism
    }

    /// <summary>
    /// Represents the current government of a civilization.  Holds
    /// modifiers that affect the entire empire.  In Civilization I
    /// switching governments requires discovering certain
    /// technologies and causes a period of anarchy.
    /// </summary>
    public class Government
    {
        public GovernmentType Type { get; private set; } = GovernmentType.Despotism;
        public int ProductionModifier { get; private set; } = 0;
        public int TradeModifier { get; private set; } = 0;
        public int CorruptionModifier { get; private set; } = 0;

        public void ChangeGovernment(GovernmentType type)
        {
            Type = type;
            // Adjust modifiers based on the selected government.
            switch (type)
            {
                case GovernmentType.Despotism:
                    ProductionModifier = 0;
                    TradeModifier = 0;
                    CorruptionModifier = 3;
                    break;
                case GovernmentType.Monarchy:
                    ProductionModifier = 1;
                    TradeModifier = 1;
                    CorruptionModifier = 2;
                    break;
                case GovernmentType.Republic:
                    ProductionModifier = 1;
                    TradeModifier = 2;
                    CorruptionModifier = 1;
                    break;
                case GovernmentType.Democracy:
                    ProductionModifier = 1;
                    TradeModifier = 3;
                    CorruptionModifier = 0;
                    break;
                case GovernmentType.Communism:
                    ProductionModifier = 2;
                    TradeModifier = 0;
                    CorruptionModifier = 1;
                    break;
                case GovernmentType.Fundamentalism:
                    ProductionModifier = 2;
                    TradeModifier = 1;
                    CorruptionModifier = 2;
                    break;
            }
        }
    }
}