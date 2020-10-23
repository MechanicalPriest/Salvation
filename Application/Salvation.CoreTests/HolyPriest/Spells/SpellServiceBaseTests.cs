using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class SpellServiceBaseTests
    {

        private GameState _gameState;
        [OneTimeSetUp]
        public void InitOnce()
        {
            IConstantsService constantsService = new ConstantsService();

            // Load this from somewhere that doesn't change
            var basePath = @"HolyPriest" + Path.DirectorySeparatorChar + "TestData";
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_profile.json")));

            _gameState = new GameState(profile, constants);
        }

        [Test]
        public void GetNumberOfHealingTargets_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetNumberOfHealingTargets(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetDuration_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetDuration(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }
    }
}
