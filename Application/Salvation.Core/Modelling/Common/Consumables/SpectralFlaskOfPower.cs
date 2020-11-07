using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.Common.Consumables
{
    public interface ISpectralFlaskOfPower : ISpellEffectService { }
    public class SpectralFlaskOfPower : SpellEffectService, ISpellEffectService<ISpectralFlaskOfPower>
    {
        public SpectralFlaskOfPower(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.SpectralFlaskOfPower;
        }

        public override double GetAverageIntellect(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            double intellect = ItemCoefficientMultiplier * spellData.GetEffect(785591).Coefficient;
            
            return intellect * GetUptime(gameState, spellData);
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            return 1; // 100% uptime
        }
    }
}
