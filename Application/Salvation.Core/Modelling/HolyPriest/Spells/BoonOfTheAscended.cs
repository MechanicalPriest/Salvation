using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class BoonOfTheAscended : SpellService, IBoonOfTheAscendedSpellService
    {
        private readonly IAscendedBlastSpellService _ascendedBlastSpellService;
        private readonly IAscendedNovaSpellService _ascendedNovaSpellService;
        private readonly IAscendedEruptionSpellService _ascendedEruptionSpellService;

        public BoonOfTheAscended(IGameStateService gameStateService,
            IModellingJournal journal,
            IAscendedBlastSpellService ascendedBlastSpellService,
            IAscendedNovaSpellService ascendedNovaSpellService,
            IAscendedEruptionSpellService ascendedEruptionSpellService)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.BoonOfTheAscended;
            _ascendedBlastSpellService = ascendedBlastSpellService;
            _ascendedNovaSpellService = ascendedNovaSpellService;
            _ascendedEruptionSpellService = ascendedEruptionSpellService;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.BoonOfTheAscended);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            // Boon of the Ascended is a short duration
            // Ascended Blast with a cooldown is usually cast as often as possible using efficiency
            // Nova is then cast with its own efficiency
            // Finally eruption is cast and has value based on the previous casts & targets hit.

            // AB 
            // based on the duration and the Boon CPM, figure out how often it's being cast
            var duration = GetDuration(gameState, spellData);
            var boonCPM = GetActualCastsPerMinute(gameState, spellData);

            // Construct the extra data AB needs to know about to run
            var abSpellData = _gameStateService.GetSpellData(gameState, Spell.AscendedBlast);
            abSpellData.Overrides[Override.CastsPerMinute] = (double)boonCPM;
            abSpellData.Overrides[Override.AllowedDuration] = (double)duration;

            var abResults = _ascendedBlastSpellService.GetCastResults(gameState, abSpellData);
            result.AdditionalCasts.Add(abResults);

            // AN
            var leftoverCastTime = GetDuration(gameState, spellData) -
                (abResults.CastsPerMinute * abResults.Gcd);

            var anSpellData = _gameStateService.GetSpellData(gameState, Spell.AscendedNova);
            anSpellData.Overrides[Override.CastsPerMinute] = (double)boonCPM;
            anSpellData.Overrides[Override.AllowedDuration] = (double)leftoverCastTime;

            var anResults = _ascendedNovaSpellService.GetCastResults(gameState, anSpellData);

            result.AdditionalCasts.Add(anResults);

            // AE
            // 1 base stack + 5 per AB + 1 per AE target
            var boonStacks = 1 + abResults.CastsPerMinute * 5 + anResults.CastsPerMinute * anResults.NumberOfDamageTargets;

            var aeSpellData = _gameStateService.GetSpellData(gameState, Spell.AscendedNova);
            aeSpellData.Overrides[Override.ResultMultiplier] = (double)boonStacks;

            var aeResults = _ascendedEruptionSpellService.GetCastResults(gameState, aeSpellData);

            result.AdditionalCasts.Add(aeResults);

            return result;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.BoonOfTheAscended);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            decimal maximumPotentialCasts = 60m / (hastedCastTime + hastedCd)
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
