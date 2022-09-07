using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface ITuftOfSmolderingPlumageSpellService : ISpellService { }
    public class TuftOfSmolderingPlumage : SpellService, ISpellService<ITuftOfSmolderingPlumageSpellService>
    {
        public TuftOfSmolderingPlumage(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.TuftOfSmolderingPlumage;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Use: Draw out a piece of the target's soul, decreasing their movement speed by 30% until the soul 
            // reaches you. The soul instantly heals you for 2995, and grants you up to 1050 Critical Strike for 16 sec. 
            // You gain more Critical Strike from lower health targets. (2 Min Cooldown)

            if (!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            var healSpell = _gameStateService.GetSpellData(gameState, Spell.TuftOfSmolderingPlumageBuff);

            // Get scale budget
            var scaledHealValue = healSpell.GetEffect(869705).GetScaledCoefficientValue(itemLevel);

            if (scaledHealValue == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"healSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var healAmount = scaledHealValue;

            // Get the percentage of the mirror that's filled up each cast
            var avgTargetHp = _gameStateService.GetPlaystyle(gameState, "TuftOfSmolderingPlumageAvgAllyHp");

            if (avgTargetHp == null)
                throw new ArgumentOutOfRangeException("TuftOfSmolderingPlumageAvgAllyHp", $"TuftOfSmolderingPlumageAvgAllyHp needs to be set.");

            // Total healing is the base heal amount then an additional 25%, depending on target HP.
            // 0.75 (75%) average hp = heal amount * (1.25 * (1 + (1 - 0.75)))
            var addedHealing = 1 + ((spellData.GetEffect(870061).BaseValue / 100) * (1 - avgTargetHp.Value));
            var totalHeal = healAmount * addedHealing;

            totalHeal *= _gameStateService.GetVersatilityMultiplier(gameState);

            totalHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return totalHeal * GetNumberOfHealingTargets(gameState, spellData);
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
            var healSpell = _gameStateService.GetSpellData(gameState, Spell.ManaboundMirrorHeal);

            // TODO: Add the direct heal effect from 344917?
            return true;
        }
    }
}
