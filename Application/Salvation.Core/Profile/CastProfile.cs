namespace Salvation.Core.Profile
{
    public class CastProfile
    {
        public int SpellId { get; set; }
        /// <summary>
        /// A percentage of the maximum potential spellcasts
        /// </summary>
        public decimal Efficiency { get; set; }
        /// <summary>
        /// How much this spell overheals for on average
        /// </summary>
        public decimal OverhealPercent { get; set; }

        public CastProfile()
        {

        }

        public CastProfile(int spellId, decimal efficiency, decimal overhealPercent)
        {
            SpellId = spellId;
            Efficiency = efficiency;
            OverhealPercent = overhealPercent;
        }
    }
}