using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Salvation.Core.Modelling
{
    public class StatWeightGenerator : IStatWeightGenerationService
    {
        public enum StatWeightType
        {
            EffectiveHealing = 0,
            RawHealing = 1,
            Damage = 2
        }

        private readonly IModellingService modellingService;
        private readonly IGameStateService gameStateService;
               

        public StatWeightGenerator(IModellingService modellingService,
            IGameStateService gameStateService)
        {
            this.modellingService = modellingService;
            this.gameStateService = gameStateService;
        }

        /// <summary>
        /// Generate stat weight results based on a specific gamestate by adding a specific 
        /// number of extra stats
        /// </summary>
        /// <param name="baseState">The baseline state</param>
        /// <param name="numAdditionalStats">Number of additional stats to add</param>
        /// <param name="swType">Type of stat weights to generate</param>
        public StatWeightResult Generate(
            GameState baseState, 
            int numAdditionalStats,
            StatWeightType swType = StatWeightType.EffectiveHealing)
        {
            StatWeightResult result = new StatWeightResult();

            // The following stages generate stat weights:
            // 1. Build a profile off of the base for each stat
            var statProfiles = GenerateStatProfiles(baseState, numAdditionalStats);

            // 2. Get results for each profile
            List<BaseModelResults> modelResults = GenerateModelResults(statProfiles);

            // 3. Compare them to build weight results
            switch (swType)
            {
                case StatWeightType.EffectiveHealing:
                    result = GenerateEffectiveHealingStatWeights(modelResults, "Effective Healing", baseState.Profile.Name, "Intellect Profile");
                    break;
                case StatWeightType.RawHealing:
                    result = GenerateRawHealingStatWeights(modelResults, "Raw Healing", baseState.Profile.Name, "Intellect Profile");
                    break;
                case StatWeightType.Damage:
                    //result = GenerateDamageStatWeights(modelResults);
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Create each of the profiles to have stat weights generated for
        /// </summary>
        /// <param name="baselineState">The baseline gamestate</param>
        /// <param name="numAdditionalStats">Additional stats to be added</param>
        internal List<GameState> GenerateStatProfiles(
            GameState baselineState, int numAdditionalStats)
        {
            var states = new List<GameState>();
            states.Add(baselineState);

            // Int
            var intState = gameStateService.CloneGameState(baselineState);
            intState.Profile.Intellect += numAdditionalStats;
            intState.Profile.Name = "Intellect Profile";
            states.Add(intState);

            // Haste
            var hasteState = gameStateService.CloneGameState(baselineState);
            hasteState.Profile.HasteRating += numAdditionalStats;
            hasteState.Profile.Name = "Haste Profile";
            states.Add(hasteState);

            // Crit
            var critState = gameStateService.CloneGameState(baselineState);
            critState.Profile.CritRating += numAdditionalStats;
            critState.Profile.Name = "Crit Profile";
            states.Add(critState);

            // Vers
            var versState = gameStateService.CloneGameState(baselineState);
            versState.Profile.VersatilityRating += numAdditionalStats;
            versState.Profile.Name = "Vers Profile";
            states.Add(versState);

            // Mastery
            var masteryState = gameStateService.CloneGameState(baselineState);
            masteryState.Profile.MasteryRating += numAdditionalStats;
            masteryState.Profile.Name = "Mastery Profile";
            states.Add(masteryState);

            return states;
        }

        internal List<BaseModelResults> GenerateModelResults(List<GameState> states)
        {
            var results = new List<BaseModelResults>();

            foreach (var state in states)
            {
                var result = modellingService.GetResults(state);

                results.Add(result);
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
