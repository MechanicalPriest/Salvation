namespace Salvation.Core.Profile
{
    public class CastProfile
    {
        public int SpellId { get; set; }
        /// <summary>
        /// A percentage of the maximum potential spellcasts
        /// </summary>
        public double Efficiency { get; set; }
        /// <summary>
        /// How much this spell overheals for on average
        /// </summary>
        public double OverhealPercent { get; set; }

        public CastProfile()
        {

        }

        public CastProfile(int spellId, double efficiency, double overhealPercent)
        {
            SpellId = spellId;
            Efficiency = efficiency;
            OverhealPercent = overhealPercent;
        }
    }
}