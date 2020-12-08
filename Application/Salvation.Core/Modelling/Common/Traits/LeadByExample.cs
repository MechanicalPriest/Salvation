using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface ILeadByExampleSpellService : ISpellService { }

    internal class LeadByExample : SpellService, ISpellService<ILeadByExampleSpellService>
    {
        private readonly ISpellService<IUnholyNovaSpellService> _unholyNovaSpellService;

        public LeadByExample(IGameStateService gameStateService,
            ISpellService<IUnholyNovaSpellService> unholyNovaSpellService)
            : base(gameStateService)
        {
            Spell = Spell.LeadByExample;
            _unholyNovaSpellService = unholyNovaSpellService;
        }

        public override double GetAverageIntellectBonus(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Unholy Nova increases your $pri by $342181s1% and up to $342181s3 nearby allies' primary stat by $342181s2% for 
            // ${$s3*$<mod>}.1 sec. You gain $342181s2% additional $pri for each ally affected.
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.LeadByExampleBuff);

            var selfIncreaseAmount = buffSpellData.GetEffect(843050).BaseValue / 100;
            var allyIncreaseAmount = buffSpellData.GetEffect(872120).BaseValue / 100;

            var includeAllyBuffs = _gameStateService.GetPlaystyle(gameState, "LeadByExampleIncludeAllyBuffs");

            if (includeAllyBuffs == null)
                throw new ArgumentOutOfRangeException("LeadByExampleIncludeAllyBuffs", $"LeadByExampleIncludeAllyBuffs needs to be set.");
            
            var nearbyAllies = _gameStateService.GetPlaystyle(gameState, "LeadByExampleNearbyAllies");

            if (nearbyAllies == null)
                throw new ArgumentOutOfRangeException("LeadByExampleNearbyAllies", $"LeadByExampleIncludeAllyBuffs needs to be set.");

            selfIncreaseAmount += (allyIncreaseAmount * nearbyAllies.Value);

            if (includeAllyBuffs.Value == 1)
                selfIncreaseAmount += (allyIncreaseAmount * nearbyAllies.Value);

            return selfIncreaseAmount * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = spellData.GetEffect(843079).BaseValue;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Value of 1 = 100% uptime
            return GetActualCastsPerMinute(gameState, spellData) * GetDuration(gameState, spellData) / 60;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            return _unholyNovaSpellService.GetActualCastsPerMinute(gameState, null);
        }
    }
}
