using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.HolyPriest.Spells;
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
            var state = GetGameState();
            IGameStateService gameStateService = new GameStateService();

            // Act
            gameStateService.RegisterSpells(state);
            var spells = state.RegisteredSpells;

            // Assert
            Assert.GreaterOrEqual(spells.Count, 0);
        }

        [Test]
        public void MiscTest()
        {
            IServiceProvider serviceProvider = new TestProvider();
            ISpellService spellFactoryFunc(Type type) => (ISpellService)serviceProvider.GetService(type);
            ISpellServiceFactory spellServiceFactory = new SpellServiceFactory(spellFactoryFunc);

            var abType = typeof(AscendedBlast);
            var type = typeof(ISpellService<>).MakeGenericType(abType);

            var spell = spellServiceFactory.GetSpellService(type);

            Assert.IsNotNull(spell);
        }
    }

    public class TestProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ISpellService<AscendedBlast>))
            {
                return new AscendedBlast(null);
            }

            return null;
        }
    }
}
