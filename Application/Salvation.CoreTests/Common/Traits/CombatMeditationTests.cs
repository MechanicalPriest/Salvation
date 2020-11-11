using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class CombatMeditationTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            var boonService = new CombatMeditationBoonMock(gameStateService);
            _spell = new CombatMeditation(gameStateService, boonService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageMastery_Throws_Without_ScaleValues()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.CombatMeditation);
            spellData.ScaleValues = new System.Collections.Generic.Dictionary<int, double>();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageMastery(_gameState, spellData));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageMastery_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageMastery(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageMastery_Adds_Average_Mastery()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("CombatMeditationOrbPickups", 1));

            // Act
            var value = _spell.GetAverageMastery(gamestate, null);

            // Assert
            Assert.AreEqual(61.596190476190479d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("CombatMeditationOrbPickups", 1));

            // Act
            var value = _spell.GetDuration(gamestate, null);

            // Assert
            Assert.AreEqual(37.0, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("CombatMeditationOrbPickups", 1));

            // Act
            var value = _spell.GetUptime(gamestate, null);

            // Assert
            Assert.AreEqual(10.571428571428571d, value);
        }
    }

    class CombatMeditationBoonMock : SpellService, ISpellService<IBoonOfTheAscendedSpellService>
    {
        public CombatMeditationBoonMock(IGameStateService gameStateService) 
            : base(gameStateService)
        {
        }

        public override double GetActualCastsPerMinute(GameState gameState, Core.Constants.BaseSpellData spellData = null)
        {
            return 2d / 7d;
        }
    }
}
