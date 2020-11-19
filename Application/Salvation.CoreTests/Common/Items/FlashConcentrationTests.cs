using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Items
{
    [TestFixture]
    class FlashConcentrationTests : BaseTest
    {
        private Heal _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new Heal(gameStateService);
            _gameState = GetGameState();

            // Create the item
            var flashConcentrationItem = new Item()
            {
                Equipped = true
            };
            flashConcentrationItem.Effects.Add(new ItemEffect()
            {
                Spell = new Core.Constants.BaseSpellData()
                {
                    Id = (int)Spell.FlashConcentration
                }
            });

            // Add it to the state
            _gameState.Profile.Items.Add(flashConcentrationItem);
        }

        [Test]
        public void FC_Throws_NoPlaystyle()
        {
            // Arrange

            // Act
            var methodCallGetFlashConcentrationCastTimeReduction = new TestDelegate(
                () => _spell.GetFlashConcentrationCastTimeReduction(_gameState, null));
            var methodCallGetFlashConcentrationHealingModifier = new TestDelegate(
                () => _spell.GetFlashConcentrationHealingModifier(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCallGetFlashConcentrationCastTimeReduction);
            Assert.Throws<ArgumentOutOfRangeException>(methodCallGetFlashConcentrationHealingModifier);
        }

        [Test]
        public void GetFlashConcentrationCastTimeReduction()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new PlaystyleEntry("FlashConcentrationAverageStacks", 5));
            
            // Act
            var value = _spell.GetFlashConcentrationCastTimeReduction(gamestate, null);

            // Assert
            Assert.AreEqual(0.75d, value);
        }

        [Test]
        public void GetFlashConcentrationHealingModifier()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new PlaystyleEntry("FlashConcentrationAverageStacks", 5));

            // Act
            var value = _spell.GetFlashConcentrationHealingModifier(gamestate, null);

            // Assert
            Assert.AreEqual(1.1499999999999999d, value);
        }

        [Test]
        public void GetHastedCastTime()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new PlaystyleEntry("FlashConcentrationAverageStacks", 5));

            // Act
            var value = _spell.GetHastedCastTime(gamestate, null);

            // Assert
            Assert.AreEqual(1.5495036222162597d, value);
        }

        [Test]
        public void GetAverageRawHealing_Increased()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            var gamestateWithout = GetGameState();
            gameStateService.OverridePlaystyle(gamestate, new PlaystyleEntry("FlashConcentrationAverageStacks", 5));

            // Act
            var valueWith = _spell.GetAverageRawHealing(gamestate, null);
            var valueWithout = _spell.GetAverageRawHealing(gamestateWithout, null);

            // Assert
            Assert.AreEqual(6046.5876546962272d, valueWith);
            Assert.AreEqual(5257.9023084315022d, valueWithout);
        }
    }
}
