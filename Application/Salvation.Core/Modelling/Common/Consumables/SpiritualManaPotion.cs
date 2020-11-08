using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Consumables
{
    public interface ISpiritualManaPotionSpellService : ISpellService { }
    public class SpiritualManaPotion : SpellService, ISpellService<ISpiritualManaPotionSpellService>
    {
        public SpiritualManaPotion(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.SpiritualManaPotion;
        }

        public override double GetAverageMp5(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            double manaPerCast = ConsumableCoefficientMultiplier * spellData.GetEffect(785599).Coefficient;

            // 1 cast at the start of the fight then 1 every ~300 seconds

            return manaPerCast * GetActualCastsPerMinute(gameState, spellData) / 60 * 5;
        }

        public override double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            return 300; // Cooldown is a 5-minute shared CD. Unknown location in game data at this time.
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var cooldown = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            return (1d + Math.Floor(fightLength / cooldown)) / fightLength * 60;
        }
    }
}
