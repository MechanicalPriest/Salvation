using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Salvation.CoreTests.Model
{
    [TestFixture]
    class StatWeightGeneratorTests : BaseTest
    {
        public void Init()
        {
            
        }

        [Test]
        public void SWG_Generates_Profiles()
        {
            // Arrange
            var swg = new StatWeightGenerator(null, new GameStateService());
            var state = GetGameState();

            // Act
            var profiles = swg.GenerateStatProfiles(state, 100);

            // Assert
            Assert.IsNotNull(profiles);
            Assert.AreEqual(7, profiles.Count);
        }

        [Test]
        public void SWG_Generates_EH_Results()
        {
            // Arrange
            var swg = new StatWeightGenerator(new ModellingServiceMock(), new GameStateService());
            var state = GetGameState();

            // Act
            var profiles = swg.Generate(state, 100, StatWeightGenerator.StatWeightType.EffectiveHealing);

            // Assert
            Assert.IsNotNull(profiles);
        }

        [Test]
        public void SWG_Generates_RH_Results()
        {
            // Arrange
            var swg = new StatWeightGenerator(new ModellingServiceMock(), new GameStateService());
            var state = GetGameState();

            // Act
            var profiles = swg.Generate(state, 100, StatWeightGenerator.StatWeightType.RawHealing);

            // Assert
            Assert.IsNotNull(profiles);
        }
    }

    class ModellingServiceMock : IModellingService
    {
        public BaseModelResults GetResults(GameState state)
        {
            var result = new BaseModelResults()
            {
                Profile = state.Profile,
                TotalActualHPS = 10,
                TotalRawHPS = 10
            };

            return result;
        }
    }
}
