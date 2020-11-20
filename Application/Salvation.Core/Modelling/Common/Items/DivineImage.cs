using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IDivineImageSpellSevice : ISpellService { }
    class DivineImage : SpellService, ISpellService<IDivineImageSpellSevice>
    {
        public DivineImage(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.DivineImage;
        }

        public override double GetAverageHealingBonus(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Healing is all of the spells the little windchime casts based on what you cast
            // 2 of them are broken and don't scale how they should and haven't been fixed still
            // Duration is garbage
            // Proc rate is atrocious and based on HW casts.

            return 0;
        }
    }
}
