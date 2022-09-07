using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IBronsCallToActionSpellService : ISpellService { }
    public class BronsCallToAction : SpellService, ISpellService<IBronsCallToActionSpellService>
    {
        public BronsCallToAction(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.BronsCallToAction;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            // Bron casts around 5 spells during his 30 seconds. Each one is around 115% SP.
            spellData = ValidateSpellData(gameState, spellData);

            var bronnSpellpower = _gameStateService.GetPlaystyle(gameState, "BronsCallToActionSpellpower");

            if (bronnSpellpower == null)
                throw new ArgumentOutOfRangeException("BronsCallToActionSpellpower", $"BronsCallToActionSpellpower needs to be set.");

            var healingSp = 1.15d; // Working out through testing, unable to find in Spelldata

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            // TODO: Again need number of casts per minute to get a good value on Bronn. Need to find a way to do this with the model

            var bronnPpm = _gameStateService.GetPlaystyle(gameState, "BronsCallToActionProcsPerMinute");

            if (bronnPpm == null)
                throw new ArgumentOutOfRangeException("BronsCallToActionProcsPerMinute", $"BronsCallToActionProcsPerMinute needs to be set.");
            
            var bronnCasts = _gameStateService.GetPlaystyle(gameState, "BronsCallToActionCastsPerProc");

            if (bronnCasts == null)
                throw new ArgumentOutOfRangeException("BronsCallToActionCastsPerProc", $"BronsCallToActionCastsPerProc needs to be set.");

            // Number of casts each time he's up multiplied by how often he's up
            return bronnCasts.Value * bronnPpm.Value;
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            // Does not trigger mastery. Unable to find in spelldata - presumably because it's a pet?
            return false;
        }
    }
}
