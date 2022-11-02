using Salvation.Core.Constants;
using Salvation.Core.State;

namespace Salvation.Core.Interfaces.Modelling.HolyPriest.Spells
{
    public interface IPrayerOfMendingSpellService : ISpellService
    {

    }

    public interface IPrayerOfMendingExtensions
    {
        public double GetAverageBounces(GameState gameState, BaseSpellData spellData);
    }
}
