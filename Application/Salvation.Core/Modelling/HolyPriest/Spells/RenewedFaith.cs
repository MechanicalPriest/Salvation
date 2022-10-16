using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface IRenewedFaithSpellService : ISpellService { }

    public class RenewedFaith : SpellService, ISpellService<IRenewedFaithSpellService>
    {
        public RenewedFaith(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.RenewedFaith;
        }

        public override double GetAverageHealingMultiplier(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Get the healing bonus            
            var healingBonus = spellData.GetEffect(842754).BaseValue / 100;

            // Get the average uptime of renew on each group member
            return 1 + (healingBonus * GetUptime(gameState, spellData));
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            var uptime = _gameStateService.GetRenewUptime(gameState);
            return uptime;
        }
    }
}
