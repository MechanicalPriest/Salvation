using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Consumables;
using Salvation.Core.State;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private List<ISpellService> _spells;

        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _spells = new List<ISpellService>();

            IGameStateService gameStateService = new GameStateService();

            _spells.Add(new SpectralFlaskOfPower(gameStateService));
            _spells.Add(new SpiritualManaPotion(gameStateService));

            _gameState = GetGameState();
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageIntellect))]
        public double GetAverageIntellect(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageIntellect(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageCriticalStrike))]
        public double GetAverageCriticalStrike(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageCriticalStrike(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageHaste))]
        public double GetAverageHaste(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageHaste(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageMastery))]
        public double GetAverageMastery(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageMastery(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageVersatility))]
        public double GetAverageVersatility(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageVersatility(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetUptime))]
        public double GetUptime(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetUptime(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetAverageMp5))]
        public double GetAverageMp5(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageMp5(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetHastedCooldown))]
        public double GetHastedCooldown(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetHastedCooldown(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetMaximumCastsPerMinute))]
        public double GetMaximumCastsPerMinute(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellEffectServiceTestsData), nameof(SpellEffectServiceTestsData.GetActualCastsPerMinute))]
        public double GetActualCastsPerMinute(Type t)
        {
            // Arrange
            var spellService = _spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetActualCastsPerMinute(_gameState, null);

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

        public static IEnumerable GetAverageMp5
        {
            get
            {
                yield return new TestCaseData(typeof(SpectralFlaskOfPower)).Returns(0);
                yield return new TestCaseData(typeof(SpiritualManaPotion)).Returns(136.02015113350123d);
            }
        }

        public static IEnumerable GetHastedCooldown
        {
            get
            {
                yield return new TestCaseData(typeof(SpiritualManaPotion)).Returns(300d);
            }
        }

        public static IEnumerable GetMaximumCastsPerMinute
        {
            get
            {
                yield return new TestCaseData(typeof(SpiritualManaPotion)).Returns(0.30226700251889166d);
            }
        }

        public static IEnumerable GetActualCastsPerMinute
        {
            get
            {
                yield return new TestCaseData(typeof(SpiritualManaPotion)).Returns(0.27204030226700249d);
            }
        }
    }
}
