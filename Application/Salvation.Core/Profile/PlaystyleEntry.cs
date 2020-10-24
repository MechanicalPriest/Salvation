namespace Salvation.Core.Profile
{
    public class PlaystyleEntry
    {
        public string Name { get; set; }
        /// <summary>
        /// The value of this entry
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// The spell this entry belongs to
        /// </summary>
        public uint SpellId { get; set; }

        public PlaystyleEntry()
        {

        }

        public PlaystyleEntry(string name, double value, uint spellId = 0)
        {
            Name = name;
            Value = value;
            SpellId = spellId;
        }
    }
}