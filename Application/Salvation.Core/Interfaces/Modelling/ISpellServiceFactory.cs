using Salvation.Core.Constants.Data;
using System;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface ISpellServiceFactory
    {
        ISpellService GetSpellService(Spell spell);
    }
}
