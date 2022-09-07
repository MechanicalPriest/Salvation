using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IMacabreSheetMusicSpellService : ISpellService { }
    public class MacabreSheetMusic : SpellService, ISpellService<IMacabreSheetMusicSpellService>
    {
        public MacabreSheetMusic(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.MacabreSheetMusic;
        }

        public override double GetAverageHaste(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if(!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            var triggerSpell = _gameStateService.GetSpellData(gameState, Spell.MacabreSheetMusicTrigger);

            // Get scale budget
            var scaledHasteAmount = triggerSpell.GetEffect(871310).GetScaledCoefficientValue(itemLevel);

            if (scaledHasteAmount == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"buffSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var hasteAmount = scaledHasteAmount;

            // 3 stacks, average of 2 for its duration
            var buffSpell = _gameStateService.GetSpellData(gameState, Spell.MacabreSheetMusicBuff);
            var averageCritAmount = hasteAmount * buffSpell.MaxStacks;

            return averageCritAmount * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var buffSpell = _gameStateService.GetSpellData(gameState, Spell.MacabreSheetMusicBuff);

            return buffSpell.Duration / 1000;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Uptime is duration * cpm
            return (GetDuration(gameState, spellData) * GetActualCastsPerMinute(gameState, spellData)) / 60;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            return 60 / hastedCd
                + 1d / (fightLength / 60d);
        }
    }
}
