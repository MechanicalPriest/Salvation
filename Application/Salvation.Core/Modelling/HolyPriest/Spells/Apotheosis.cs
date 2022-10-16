using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface IApotheosisSpellService : ISpellService { }
    public class Apotheosis : SpellService, ISpellService<IApotheosisSpellService>
    {
        public Apotheosis(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.Apotheosis;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            return base.GetDuration(gameState, spellData);
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = GetDuration(gameState, spellData);

            var actualCpm = GetActualCastsPerMinute(gameState, spellData);

            var uptime = duration * actualCpm / 60;

            return uptime;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Halo is simply 60 / (CastTime + CD) + 1 / (FightLength / 60)
            // Number of casts per minute plus one cast at the start of the encounter
            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd)
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }
    }
}
