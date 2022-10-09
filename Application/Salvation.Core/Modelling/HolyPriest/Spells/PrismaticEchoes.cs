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
    public interface IPrismaticEchoesSpellService : ISpellService { }

    public class PrismaticEchoes : SpellService, ISpellService<IPrismaticEchoesSpellService>
    {
        public PrismaticEchoes(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.PrismaticEchoes;
        }

        public override double GetAverageMasteryIncreaseMultiplier(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.PrismaticEchoes);

            if (talent != null && talent.Rank > 0)
            {
                multi += (spellData.GetEffect(1028161).BaseValue / 100) * talent.Rank;
            }

            return multi;
        }
    }
}
