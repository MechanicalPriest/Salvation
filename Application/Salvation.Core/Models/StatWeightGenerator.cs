using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models
{
    internal class StatWeightGenerator
    {
        // Give this a base profile
        // Add X of each stat
        // Calculate the stat weights
        // Return the stat weights
        public BaseProfile BaselineProfile { get; private set; }
        public int AdditionalStats { get; private set; }

        public StatWeightGenerator(BaseProfile baseProfile, int numAdditionalStats)
        {
            BaselineProfile = baseProfile;
            AdditionalStats = numAdditionalStats;
        }

        public StatWeightResult Generate()
        {
            StatWeightResult result = new StatWeightResult();

            // The following stages generate stat weights:
            // 1. Build a profile off of the base for each stat
            var statProfiles = GenerateStatProfiles(BaselineProfile);

            // 2. Get results for each profile
            List<BaseModelResults> modelResults = GenerateModelResults(statProfiles);

            // 3. Compare them to build weight results

            return result;
        }

        internal List<BaseProfile> GenerateStatProfiles(BaseProfile baselineProfile)
        {
            var statProfiles = new List<BaseProfile>();
            statProfiles.Add(baselineProfile);

            // Int
            var intProfile = BaseProfile.Clone(baselineProfile);
            intProfile.Intellect += 10;
            intProfile.Name = "Intellect Profile";
            statProfiles.Add(intProfile);

            // Haste
            var hasteProfile = BaseProfile.Clone(baselineProfile);
            hasteProfile.HasteRating += 10;
            hasteProfile.Name = "Haste Profile";
            statProfiles.Add(hasteProfile);
            
            // Crit
            var critProfile = BaseProfile.Clone(baselineProfile);
            critProfile.CritRating += 10;
            critProfile.Name = "Crit Profile";
            statProfiles.Add(critProfile);

            // Vers
            var versProfile = BaseProfile.Clone(baselineProfile);
            versProfile.VersatilityRating += 10;
            versProfile.Name = "Vers Profile";
            statProfiles.Add(versProfile);

            // Mastery
            var masteryProfile = BaseProfile.Clone(baselineProfile);
            masteryProfile.MasteryRating += 10;
            masteryProfile.Name = "Mastery Profile";
            statProfiles.Add(masteryProfile);

            return statProfiles;
        }

        internal List<BaseModelResults> GenerateModelResults(List<BaseProfile> statProfiles)
        {
            throw new NotImplementedException();
        }
    }
}
