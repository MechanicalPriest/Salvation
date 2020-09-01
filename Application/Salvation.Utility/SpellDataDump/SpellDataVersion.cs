using System.Text.RegularExpressions;

namespace Salvation.Utility.SpellDataDump
{
    

    class SpellDataVersion
    {
        public string String;
        public string SimCraftVersion;
        public string WowVersion;
        public string WowTag;
        public string WowPatch;

        public SpellDataVersion(string versionString)
        {
            String = versionString;

            string versionPattern = @"^SimulationCraft (\d+-\d+) for World of Warcraft ([0-9\.]+) (\S+) \(([^\)]+)\)";

            Match match = Regex.Match(versionString, versionPattern);

            if (match.Success)
            {
                SimCraftVersion = match.Groups[1].Value;
                WowVersion = match.Groups[2].Value;
                WowTag = match.Groups[3].Value;
                WowPatch = match.Groups[4].Value;
            }
        }
    }
}
