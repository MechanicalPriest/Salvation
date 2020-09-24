using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Salvation.Core.Modelling
{
    public class StatWeightGenerator
    {
        public enum StatWeightType
        {
            EffectiveHealing = 0,
            RawHealing = 1,
            Damage = 2
        }

        private IConstantsService constantsManager { get; set; }

        public StatWeightGenerator(IConstantsService cm)
        {
            constantsManager = cm;
        }

        public StatWeightResult Generate(
            BaseProfile baseProfile, 
            int numAdditionalStats,
            StatWeightType swType = StatWeightType.EffectiveHealing)
        {
            StatWeightResult result = new StatWeightResult();

            // The following stages generate stat weights:
            // 1. Build a profile off of the base for each stat
            var statProfiles = GenerateStatProfiles(baseProfile, numAdditionalStats);

            // 2. Get results for each profile
            List<BaseModelResults> modelResults = GenerateModelResults(statProfiles);

            // 3. Compare them to build weight results
            switch (swType)
            {
                case StatWeightType.EffectiveHealing:
                    result = GenerateEffectiveHealingStatWeights(modelResults, "Effective Healing", baseProfile.Name, "Intellect Profile");
                    break;
                case StatWeightType.RawHealing:
                    result = GenerateRawHealingStatWeights(modelResults, "Raw Healing", baseProfile.Name, "Intellect Profile");
                    break;
                case StatWeightType.Damage:
                    //result = GenerateDamageStatWeights(modelResults);
                    break;
                default:
                    break;
            }

            return result;
        }

        internal List<BaseProfile> GenerateStatProfiles(
            BaseProfile baselineProfile, int numAdditionalStats)
        {
            var statProfiles = new List<BaseProfile>();
            statProfiles.Add(baselineProfile);

            // Int
            var intProfile = BaseProfile.Clone(baselineProfile);
            intProfile.Intellect += numAdditionalStats;
            intProfile.Name = "Intellect Profile";
            statProfiles.Add(intProfile);

            // Haste
            var hasteProfile = BaseProfile.Clone(baselineProfile);
            hasteProfile.HasteRating += numAdditionalStats;
            hasteProfile.Name = "Haste Profile";
            statProfiles.Add(hasteProfile);
            
            // Crit
            var critProfile = BaseProfile.Clone(baselineProfile);
            critProfile.CritRating += numAdditionalStats;
            critProfile.Name = "Crit Profile";
            statProfiles.Add(critProfile);

            // Vers
            var versProfile = BaseProfile.Clone(baselineProfile);
            versProfile.VersatilityRating += numAdditionalStats;
            versProfile.Name = "Vers Profile";
            statProfiles.Add(versProfile);

            // Mastery
            var masteryProfile = BaseProfile.Clone(baselineProfile);
            masteryProfile.MasteryRating += numAdditionalStats;
            masteryProfile.Name = "Mastery Profile";
            statProfiles.Add(masteryProfile);

            return statProfiles;
        }

        internal List<BaseModelResults> GenerateModelResults(List<BaseProfile> statProfiles)
        {
            var results = new List<BaseModelResults>();

            foreach (var profile in statProfiles)
            {
                var constants = constantsManager.LoadConstantsFromFile();
                // TODO: Create a new state and throw that against the ModellingService
                //var model = ModelManager.LoadModel(profile, constants);
                //var result = model.GetResults();

                //results.Add(result);
            }

            return results;
        }

        internal StatWeightResult GenerateEffectiveHealingStatWeights(
            List<BaseModelResults> modelResults,
            string statWeightName,
            string baselineProfileName,
            string normalisedProfileName)
        {
            StatWeightResult swResults = new StatWeightResult();
            swResults.Name = statWeightName;

            // Get the baseline resultset
            var baseResult = modelResults.Where(m => m.Profile.Name == baselineProfileName).FirstOrDefault();
            if (baseResult == null)
                throw new NullReferenceException("Base profile does not exist in the resultset. Try again.");

            // get the resultset we're normalising against
            var normalisedResult = modelResults.Where(m => m.Profile.Name == normalisedProfileName).FirstOrDefault();
            if (normalisedResult == null)
                throw new NullReferenceException("Normalised profile does not exist in the resultset. Try again.");
            
            // Calculate the +HPS of the normalised resultset and add it to the stat weights with weight 1
            var normalisedExtraEffectiveHPS = normalisedResult.TotalActualHPS - baseResult.TotalActualHPS;
            StatWeightResultEntry normalisedEntry = new StatWeightResultEntry()
            {
                Stat = normalisedResult.Profile.Name,
                Value = normalisedExtraEffectiveHPS,
                Weight = 1
            };
            swResults.Results.Add(normalisedEntry);

            foreach(var result in modelResults.Where(
                m => m.Profile.Name != baselineProfileName && m.Profile.Name != normalisedProfileName)
                .ToList())
            {
                // A secondary's weight for HPS is added HPS divided by added int HPS 
                // this normalises int with a weight of 1.
                var calculatedAdditionalHPS = result.TotalActualHPS - baseResult.TotalActualHPS;
                var calculatedWeight = calculatedAdditionalHPS / normalisedExtraEffectiveHPS;

                StatWeightResultEntry swEntry = new StatWeightResultEntry()
                {
                    Stat = result.Profile.Name,
                    Value = calculatedAdditionalHPS,
                    Weight = calculatedWeight
                };

                swResults.Results.Add(swEntry);
            }


            return swResults;
        }

        internal StatWeightResult GenerateRawHealingStatWeights(
            List<BaseModelResults> modelResults,
            string statWeightName,
            string baselineProfileName,
            string normalisedProfileName)
        {
            StatWeightResult swResults = new StatWeightResult();
            swResults.Name = statWeightName;

            // Get the baseline resultset
            var baseResult = modelResults.Where(m => m.Profile.Name == baselineProfileName).FirstOrDefault();
            if (baseResult == null)
                throw new NullReferenceException("Base profile does not exist in the resultset. Try again.");

            // get the resultset we're normalising against
            var normalisedResult = modelResults.Where(m => m.Profile.Name == normalisedProfileName).FirstOrDefault();
            if (normalisedResult == null)
                throw new NullReferenceException("Normalised profile does not exist in the resultset. Try again.");

            // Calculate the +HPS of the normalised resultset and add it to the stat weights with weight 1
            var normalisedExtraRawHPS = normalisedResult.TotalRawHPS - baseResult.TotalRawHPS;
            StatWeightResultEntry normalisedEntry = new StatWeightResultEntry()
            {
                Stat = normalisedResult.Profile.Name,
                Value = normalisedExtraRawHPS,
                Weight = 1
            };
            swResults.Results.Add(normalisedEntry);

            foreach (var result in modelResults.Where(
                m => m.Profile.Name != baselineProfileName && m.Profile.Name != normalisedProfileName)
                .ToList())
            {
                // A secondary's weight for HPS is added HPS divided by added int HPS 
                // this normalises int with a weight of 1.
                var calculatedAdditionalHPS = result.TotalRawHPS - baseResult.TotalRawHPS;
                var calculatedWeight = calculatedAdditionalHPS / normalisedExtraRawHPS;

                StatWeightResultEntry swEntry = new StatWeightResultEntry()
                {
                    Stat = result.Profile.Name,
                    Value = calculatedAdditionalHPS,
                    Weight = calculatedWeight
                };

                swResults.Results.Add(swEntry);
            }


            return swResults;
        }
    }
}
