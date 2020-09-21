using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest.Spells
{
    public class FlashHeal : SpellService, IFlashHealSpellService
    {
        public FlashHeal(IGameStateService gameStateService, IModellingService modellingService)
            : base (gameStateService, modellingService)
        {

        }

        public decimal GetAverageHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.FlashHeal);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            
            // Flash Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * gameStateService.GetCriticalStrikeMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            return averageHeal * spellData.NumberOfHealingTargets;
        }
    }
}
