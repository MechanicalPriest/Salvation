using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
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
    public class PrayerOfMendingTests : BaseTest
    {
        private GameState _gameState;
        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void PoM_GetAverageRawHealing_Calculates_Override()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new PrayerOfMending(gameStateService);

            // Act

            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            var spellData = gameStateService.GetSpellData(_gameState, Spell.PrayerOfMending);
            var defaultNumStacks = spellData.GetEffect(22870).BaseValue;
            spellData.Overrides.Add(Override.ResultMultiplier, defaultNumStacks / 2);

            
            var resultOverride = spellService.GetAverageRawHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(4240.270125, resultDefault);
            Assert.AreEqual(resultDefault / 2, resultOverride);
        }

        [Test]
        public void GetHastedCastTime_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new PoMTestGetHastedCastTime(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetHastedCastTime(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }
    }

    public class PoMTestGetHastedCastTime : PrayerOfMending
    {
        public PoMTestGetHastedCastTime(IGameStateService gameStateService) 
            : base(gameStateService)
        {

        }

        public override double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            SpellId = 0;
            return base.GetHastedCastTime(gameState, spellData);
        }
    }
}
