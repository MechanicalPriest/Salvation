﻿using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.State;
using SimcProfileParser;
using System.IO;
using System.Threading.Tasks;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    public class StatRatingTests
    {
        IGameStateService _gameStateService;
        private GameState _state;

        [OneTimeSetUp]
        public async Task InitOnce()
        {
            var basePath = @"State" + Path.DirectorySeparatorChar + "TestData";
            var profileStringBeitaky = await File.ReadAllTextAsync(
                Path.Combine("Profile" + Path.DirectorySeparatorChar + "TestData", "Beitaky.simc"));

            var simcProfileService = new SimcProfileService(
                new SimcGenerationService(),
                new ProfileGenerationService()
                );

            IConstantsService constantsService = new ConstantsService();
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine(basePath, "StateTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine(basePath, "StateTests_profile.json")));

            await simcProfileService.ApplySimcProfileAsync(profileStringBeitaky, profile);

            _gameStateService = new GameStateService();

            _state = new GameState(profile, constants);
        }

        [Test]
        public void GSG_Calculates_CritRating()
        {
            // Arrange

            // Act
            var crit = _gameStateService.GetCriticalStrikeRating(_state);

            // Assert
            Assert.AreEqual(238, crit); // 238 on character sheet.
        }
    }
}
