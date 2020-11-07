using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using System;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface ISpellServiceFactory
    {
        ISpellService GetSpellService(Type spell);
    }
}
