using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.Modelling.HolyPriest.Spells;
using System;

namespace Salvation.Core.Modelling
{
    public class SpellServiceFactory : ISpellServiceFactory
    {
        private readonly Func<Type, ISpellService> _spellFactory;

        public SpellServiceFactory(Func<Type, ISpellService> spellFactory)
        {
            _spellFactory = spellFactory;
        }

        public ISpellService GetSpellService(Spell spell)
        {
            Type type = spell switch
            {
                Spell.AscendedBlast => typeof(IAscendedBlastSpellService),

                Spell.UnboundChangeling => typeof(IUnboundChangeling),
                _ => null
            };

            if (type == null)
                return null;

            var spellType = typeof(ISpellService<>).MakeGenericType(type);

            return _spellFactory(spellType);
        }
    }
}
