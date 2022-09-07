using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IManaboundMirrorSpellService : ISpellService { }
    public class ManaboundMirror : SpellService, ISpellService<IManaboundMirrorSpellService>
    {
        public ManaboundMirror(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.ManaboundMirror;
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

            var healSpell = _gameStateService.GetSpellData(gameState, Spell.ManaboundMirrorHeal);

            // Get scale budget
            var scaledBaseHealAmount = healSpell.GetEffect(868433).GetScaledCoefficientValue(itemLevel);
            var scaledBonusHealAmount = spellData.GetEffect(868611).GetScaledCoefficientValue(itemLevel);

            if (scaledBaseHealAmount == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"healSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            if (scaledBonusHealAmount == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"healSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var baseHealAmount = scaledBaseHealAmount;
            var bonusHealAmount = scaledBonusHealAmount;

            // Get the percentage of the mirror that's filled up each cast
            var avgMirrorFill = _gameStateService.GetPlaystyle(gameState, "ManaboundMirrorPercentMirrorFilled");

            if (avgMirrorFill == null)
                throw new ArgumentOutOfRangeException("ManaboundMirrorPercentMirrorFilled", $"ManaboundMirrorPercentMirrorFilled needs to be set.");

            var totalHeal = baseHealAmount + (bonusHealAmount * avgMirrorFill.Value);

            totalHeal *= _gameStateService.GetVersatilityMultiplier(gameState);

            totalHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return totalHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            var healSpell = _gameStateService.GetSpellData(gameState, Spell.ManaboundMirrorHeal);
            var hastedCd = GetHastedCooldown(gameState, healSpell);
            //var fightLength = _gameStateService.GetFightLength(gameState);

            // TODO: If this can be stacked up pre-fight, add the one at the start of the fight
            // TODO: Factor in you can get your first usage about 30-seconds in rather than a minute.
            return 60 / hastedCd;
                //+ 1d / (fightLength / 60d); // plus one at the start of the fight
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            var healSpell = _gameStateService.GetSpellData(gameState, Spell.ManaboundMirrorHeal);

            // The spell effect type is nested down inside a couple of effect trigger spells
            return base.TriggersMastery(gameState, healSpell);
        }
    }
}
