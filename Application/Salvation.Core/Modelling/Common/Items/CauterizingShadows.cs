using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface ICauterizingShadowsSpellSevice : ISpellService { }
    class CauterizingShadows : SpellService, ISpellService<ICauterizingShadowsSpellSevice>
    {
        private readonly ISpellService<IShadowWordPainSpellService> _shadowWordPainSpellService;

        public CauterizingShadows(IGameStateService gameStateService,
            ISpellService<IShadowWordPainSpellService> shadowWordPainSpellService)
            : base(gameStateService)
        {
            Spell = Spell.CauterizingShadows;
            _shadowWordPainSpellService = shadowWordPainSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            // grab healing data, use the other methods to calc
            var healingSpell = _gameStateService.GetSpellData(gameState, Spell.CauterizingShadowsHeal);

            var healingSp = healingSpell.GetEffect(833801).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);
            // 3 from spelldata
            var numTargets = spellData.GetEffect(833798).BaseValue;

            return numTargets;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);
            // Maximum with the casts that don't do healing (target dies / sw:p is refreshed
            var maxCasts = GetMaximumCastsPerMinute(gameState, spellData);

            // Grab the playstyle value that indicates what percentage of sw:p casts expire
            var expiryPercent = _gameStateService.GetPlaystyle(gameState, "CauterizingShadowsSwpExpiryPercent");

            if(expiryPercent == null)
                throw new ArgumentOutOfRangeException("CauterizingShadowsSwpExpiryPercent", $"CauterizingShadowsSwpExpiryPercent needs to be set.");

            // Apply the expiry percent. 90% expire means we get max_casts * 0.9
            return maxCasts * expiryPercent.Value;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            // sw:p casts
            return _shadowWordPainSpellService.GetActualCastsPerMinute(gameState, null);
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            var healingSpell = _gameStateService.GetSpellData(gameState, Spell.CauterizingShadowsHeal);

            return base.TriggersMastery(gameState, healingSpell);
        }
    }
}
