using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class BoonOfTheAscended : SpellService, IBoonOfTheAscendedSpellService
    {
        private readonly IAscendedBlastSpellService ascendedBlastSpellService;
        private readonly IAscendedNovaSpellService ascendedNovaSpellService;
        private readonly IAscendedEruptionSpellService ascendedEruptionSpellService;

        public BoonOfTheAscended(IGameStateService gameStateService,
            IModellingJournal journal,
            IAscendedBlastSpellService ascendedBlastSpellService,
            IAscendedNovaSpellService ascendedNovaSpellService,
            IAscendedEruptionSpellService ascendedEruptionSpellService)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.BoonOfTheAscended;
            this.ascendedBlastSpellService = ascendedBlastSpellService;
            this.ascendedNovaSpellService = ascendedNovaSpellService;
            this.ascendedEruptionSpellService = ascendedEruptionSpellService;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.BoonOfTheAscended);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData, moreData);

            // Boon of the Ascended is a short duration
            // Ascended Blast with a cooldown is usually cast as often as possible using efficiency
            // Nova is then cast with its own efficiency
            // Finally eruption is cast and has value based on the previous casts & targets hit.

            // AB 
            // based on the duration and the Boon CPM, figure out how often it's being cast
            var duration = GetDuration(gameState, spellData, moreData);
            var boonCPM = GetActualCastsPerMinute(gameState, spellData, moreData);

            // Construct the extra data AB needs to know about to run
            var abMoreData = new Dictionary<string, decimal>()
            {
                ["BoonOfTheAscended.Duration"] = duration,
                ["BoonOfTheAscended.CPM"] = boonCPM
            };

            var abResults = ascendedBlastSpellService.GetCastResults(gameState, null, abMoreData);
            result.AdditionalCasts.Add(abResults);

            // AN
            var leftoverCastTime = GetDuration(gameState, spellData, moreData) -
                (abResults.CastsPerMinute * abResults.Gcd);

            var anMoreData = new Dictionary<string, decimal>()
            {
                ["BoonOfTheAscended.LeftoverDuration"] = leftoverCastTime,
                ["BoonOfTheAscended.CPM"] = boonCPM
            };

            var anResults = ascendedNovaSpellService.GetCastResults(gameState, null, anMoreData);

            result.AdditionalCasts.Add(anResults);

            // AE
            // 1 base stack + 5 per AB + 1 per AE target
            var boonStacks = 1 + abResults.CastsPerMinute * 5 + anResults.CastsPerMinute * anResults.NumberOfDamageTargets;
            var aeMoreData = new Dictionary<string, decimal>()
            {
                ["BoonOfTheAscended.BoonStacks"] = boonStacks
            };

            var aeResults = ascendedEruptionSpellService.GetCastResults(gameState, null, aeMoreData);

            result.AdditionalCasts.Add(aeResults);

            return result;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.BoonOfTheAscended);

            var hastedCastTime = GetHastedCastTime(gameState, spellData, moreData);
            var hastedCd = GetHastedCooldown(gameState, spellData, moreData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            decimal maximumPotentialCasts = 60m / (hastedCastTime + hastedCd)
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
