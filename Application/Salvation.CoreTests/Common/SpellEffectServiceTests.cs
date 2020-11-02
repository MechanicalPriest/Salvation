using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.Common.Consumables;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Salvation.CoreTests.Common
{
    /// <summary>
    /// A mixture of integration / unit tests to ensure the methods return expected results 
    /// regardless of different configuration values
    /// </summary>
    [TestFixture]
    public class SpellEffectServiceTests : BaseTest
    {
        private List<ISpellEffectService> Spells { get; set; }

        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            Spells = new List<ISpellEffectService>();

            IGameStateService gameStateService = new GameStateService();

            Spells.Add(new SpectralFlaskOfPower(gameStateService));

            _gameState = GetGameState();
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageIntellect))]
        public double GetAverageIntellect(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageIntellect(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageCriticalStrike))]
        public double GetAverageCriticalStrike(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageCriticalStrike(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageHaste))]
        public double GetAverageHaste(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageHaste(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageMastery))]
        public double GetAverageMastery(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageMastery(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageVersatility))]
        public double GetAverageVersatility(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageVersatility(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetUptime))]
        public double GetUptime(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetUptime(_gameState, null);

            // Assert
            return result;
        }
    }

    public class SpellEffectServiceTestsData
    {
        public static IEnumerable GetAverageIntellect
        {
            get
            {
                yield return new TestCaseData(typeof(SpectralFlaskOfPower)).Returns(69.999989999999997d);
            }
        }
        public static IEnumerable GetAverageCriticalStrike
        {
            get
            {
                yield return new TestCaseData(typeof(SpectralFlaskOfPower)).Returns(0);
            }
        }
        public static IEnumerable GetAverageHaste
        {
            get
            {
                yield return new TestCaseData(typeof(SpectralFlaskOfPower)).Returns(0);
            }
        }
        public static IEnumerable GetAverageMastery
        {
            get
            {
                yield return new TestCaseData(typeof(SpectralFlaskOfPower)).Returns(0);
            }
        }
        public static IEnumerable GetAverageVersatility
        {
            get
            {
                yield return new TestCaseData(typeof(SpectralFlaskOfPower)).Returns(0);
            }
        }

        public static IEnumerable GetUptime
        {
            get
            {
                yield return new TestCaseData(typeof(SpectralFlaskOfPower)).Returns(1);
            }
        }
    }
}
