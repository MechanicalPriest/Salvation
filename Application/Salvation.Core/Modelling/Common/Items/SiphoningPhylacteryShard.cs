using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface ISiphoningPhylacteryShardSpellService : ISpellService { }
    class SiphoningPhylacteryShard : SpellService, ISpellService<ISiphoningPhylacteryShardSpellService>
    {
        public SiphoningPhylacteryShard(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.SiphoningPhylacteryShard;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if (!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            // Get scale budget
            var healSpell = _gameStateService.GetSpellData(gameState, Spell.SiphoningPhylacteryShardBuff);
            var scaledHealingValue = healSpell.GetEffect(871498).GetScaledCoefficientValue(itemLevel);

            if (scaledHealingValue == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"healSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var healingAmount = scaledHealingValue;

            healingAmount *= _gameStateService.GetVersatilityMultiplier(gameState);

            healingAmount *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return healingAmount * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            return 60 / hastedCd
                + 1d / (fightLength / 60d); // plus one at the start of the fight
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            // TODO: Add the direct heal effect from 345701?
            return true;
        }
    }
}
