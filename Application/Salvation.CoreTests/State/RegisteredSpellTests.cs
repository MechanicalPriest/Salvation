using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    public class RegisteredSpellTests : BaseTest
    {
        [Test]
        public void Generates_Registered_Spells()
        {
            // Arrange
            IServiceProvider serviceProvider = new TestProvider();
            ISpellService spellFactoryFunc(Type type) => (ISpellService)serviceProvider.GetService(type);
            ISpellServiceFactory spellServiceFactory = new SpellServiceFactory(spellFactoryFunc);
            IGameStateService gameStateService = new GameStateService(null, new ProfileService(), new ConstantsService(), spellServiceFactory);
            var state = GetGameState();

            // Act
            gameStateService.RegisterSpells(state, new System.Collections.Generic.List<Core.Profile.Model.RegisteredSpell>());
            var spells = state.RegisteredSpells;

            // Assert
            Assert.GreaterOrEqual(spells.Count, 0);
        }

        [Test]
        public void Valid_Spell_Returns_Service()
        {
            IServiceProvider serviceProvider = new TestProvider();
            ISpellService spellFactoryFunc(Type type) => (ISpellService)serviceProvider.GetService(type);
            ISpellServiceFactory spellServiceFactory = new SpellServiceFactory(spellFactoryFunc);

            var spell = spellServiceFactory.GetSpellService(Core.Constants.Data.Spell.Renew);

            Assert.IsNotNull(spell);
        }

        [Test]
        public void Invalid_Spell_Returns_Null()
        {
            IServiceProvider serviceProvider = new TestProvider();
            ISpellService spellFactoryFunc(Type type) => (ISpellService)serviceProvider.GetService(type);
            ISpellServiceFactory spellServiceFactory = new SpellServiceFactory(spellFactoryFunc);

            var spell = spellServiceFactory.GetSpellService(Core.Constants.Data.Spell.Renew);

            Assert.IsNull(spell);
        }
    }

    public class TestProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ISpellService<IRenewSpellService>))
            {
                return new Renew(null);
            }

            return null;
        }
    }
}
