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
    public interface ITwistOfFateSpellService : ISpellService { }

    public class TwistOfFate : SpellService, ISpellService<ITwistOfFateSpellService>
    {
        public TwistOfFate(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.TwistOfFate;
        }

        public override double GetAverageHealingMultiplier(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Get the healing bonus
            var healingBonus = 0.0d;

            var talent = _gameStateService.GetTalent(gameState, Spell.TwistOfFate);

            if (talent != null && talent.Rank > 0)
            {
                healingBonus += spellData.GetEffect(1028172).BaseValue / 100 * talent.Rank;
                healingBonus *= GetUptime(gameState, spellData);
            }

            return 1 + (healingBonus);
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            var twistedFateUptime = _gameStateService.GetPlaystyle(gameState, "TwistOfFateUptime");

            if (twistedFateUptime == null)
                throw new ArgumentOutOfRangeException("TwistOfFateUptime", $"TwistOfFateUptime needs to be set.");

            return twistedFateUptime.Value;
        }
    }
}
