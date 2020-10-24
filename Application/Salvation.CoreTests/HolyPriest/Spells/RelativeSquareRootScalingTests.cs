using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    class RelativeSquareRootScalingTests
    {
        [Test]
        public void Returns1_For_5_Targets()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var holyNova = new HolyNova(gameStateService);

            // Act
            var result = holyNova.GetTargetScaling(5);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ReturnsLess_For_6_Targets()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var holyNova = new HolyNova(gameStateService);

            // Act
            var result = holyNova.GetTargetScaling(6);

            // Assert
            Assert.AreNotEqual(1, result);
            Assert.AreEqual(0.91287092917527701d, result);
        }

        [Test]
        public void Returns1_For_0_Targets()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var holyNova = new HolyNova(gameStateService);

            // Act
            var result = holyNova.GetTargetScaling(0);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
