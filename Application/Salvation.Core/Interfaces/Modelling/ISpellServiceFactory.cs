using System;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface ISpellServiceFactory
    {
        ISpellService GetSpellService(Type spell);
    }
}
