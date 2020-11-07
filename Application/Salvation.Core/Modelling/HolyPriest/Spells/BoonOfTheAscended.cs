using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class BoonOfTheAscended : SpellService, ISpellService<IBoonOfTheAscendedSpellService>
    {
        private readonly ISpellService<IAscendedBlastSpellService> _ascendedBlastSpellService;
        private readonly ISpellService<IAscendedNovaSpellService> _ascendedNovaSpellService;
        private readonly ISpellService<IAscendedEruptionSpellService> _ascendedEruptionSpellService;

        public BoonOfTheAscended(IGameStateService gameStateService,
            ISpellService<IAscendedBlastSpellService> ascendedBlastSpellService,
            ISpellService<IAscendedNovaSpellService> ascendedNovaSpellService,
            ISpellService<IAscendedEruptionSpellService> ascendedEruptionSpellService)
            : base(gameStateService)
        {
            Spell = Spell.BoonOfTheAscended;
            _ascendedBlastSpellService = ascendedBlastSpellService;
            _ascendedNovaSpellService = ascendedNovaSpellService;
            _ascendedEruptionSpellService = ascendedEruptionSpellService;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

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
            abSpellData.Overrides[Override.CastsPerMinute] = boonCPM;
            abSpellData.Overrides[Override.AllowedDuration] = duration;

            var abResults = _ascendedBlastSpellService.GetCastResults(gameState, abSpellData);
            result.AdditionalCasts.Add(abResults);

            // AN
            var leftoverCastTime = duration - (abResults.CastsPerMinute * abResults.Gcd);

            var anSpellData = _gameStateService.GetSpellData(gameState, Spell.AscendedNova);
            anSpellData.Overrides[Override.CastsPerMinute] = boonCPM;
            anSpellData.Overrides[Override.AllowedDuration] = leftoverCastTime;

            var anResults = _ascendedNovaSpellService.GetCastResults(gameState, anSpellData);

            result.AdditionalCasts.Add(anResults);

            // AE
            // 1 base stack + 5 per AB + 1 per AE target
            var boonStacks = 1 + abResults.CastsPerMinute * 5 + anResults.CastsPerMinute * anResults.NumberOfDamageTargets;

            var aeSpellData = _gameStateService.GetSpellData(gameState, Spell.AscendedNova);
            aeSpellData.Overrides[Override.ResultMultiplier] = boonStacks;

            var aeResults = _ascendedEruptionSpellService.GetCastResults(gameState, aeSpellData);

            result.AdditionalCasts.Add(aeResults);

            return result;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd)
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }
    }
}
