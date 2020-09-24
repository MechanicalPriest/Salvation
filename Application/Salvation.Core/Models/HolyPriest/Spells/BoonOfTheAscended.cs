using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest.Spells
{
    public class BoonOfTheAscended : SpellService, IBoonOfTheAscendedSpellService
    {
        private readonly IAscendedBlastSpellService ascendedBlastSpellService;
        private readonly IAscendedNovaSpellService ascendedNovaSpellService;

        public BoonOfTheAscended(IGameStateService gameStateService,
            IModellingJournal journal,
            IAscendedBlastSpellService ascendedBlastSpellService,
            IAscendedNovaSpellService ascendedNovaSpellService)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.BoonOfTheAscended;
            this.ascendedBlastSpellService = ascendedBlastSpellService;
            this.ascendedNovaSpellService = ascendedNovaSpellService;
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

            var abMoreData = new Dictionary<string, decimal>()
            {
                ["BoonOfTheAscended.Duration"] = duration,
                ["BoonOfTheAscended.CPM"] = boonCPM
            };

            var abSpellData = gameStateService.GetSpellData(gameState, SpellIds.AscendedBlast);

            var abResults = ascendedBlastSpellService.GetCastResults(gameState, abSpellData, abMoreData);
            result.AdditionalCasts.Add(abResults);

            // AN
            var leftoverCastTime = GetDuration(gameState, spellData, moreData) -
                (abResults.CastsPerMinute * abResults.Gcd);

            // Construct the extra data AN needs to know about to run
            var anMoreData = new Dictionary<string, decimal>()
            {
                ["BoonOfTheAscended.LeftoverDuration"] = leftoverCastTime,
                ["BoonOfTheAscended.CPM"] = boonCPM
            };
            moreData.Add("BoonOfTheAscended.Duration", leftoverCastTime);

            var anSpellData = gameStateService.GetSpellData(gameState, SpellIds.AscendedBlast);

            var anResults = ascendedNovaSpellService.GetCastResults(gameState, anSpellData, anMoreData);

            result.AdditionalCasts.Add(anResults);

            // AE

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
