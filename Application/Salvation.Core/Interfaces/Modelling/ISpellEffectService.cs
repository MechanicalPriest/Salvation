using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.State;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface ISpellEffectService : ISpellService
    {
        // Effect Behaviour
        double GetUptime(GameState gameState, BaseSpellData spellData);

        // Effect Properties
        double GetAverageCriticalStrike(GameState gameState, BaseSpellData spellData);
        double GetAverageHaste(GameState gameState, BaseSpellData spellData);
        double GetAverageIntellect(GameState gameState, BaseSpellData spellData);
        double GetAverageMastery(GameState gameState, BaseSpellData spellData);
        double GetAverageVersatility(GameState gameState, BaseSpellData spellData);
        double GetAverageMp5(GameState gameState, BaseSpellData spellData);
    }

    public interface ISpellEffectService<T> : ISpellEffectService
    {

    }
}
