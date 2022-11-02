using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.State;
using SimcProfileParser;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Salvation.CoreTests.Profile
{
    [TestFixture]
    public class SimcProfileServiceTests
    {
        private string _profileStringBeitaky;
        private SimcProfileService _simcProfileService;

        [OneTimeSetUp]
        public async Task InitOnce()
        {
            // Load this from somewhere that doesn't change
            _profileStringBeitaky = await File.ReadAllTextAsync(
                Path.Combine("TestData", "Beitaky.simc"));

            _simcProfileService = new SimcProfileService(
                new SimcGenerationService(),
                new ProfileService()
                );
        }

        [Test]
        public async Task SPS_Applies_Basic_Properties()
        {
            // Arrange
            var baseProfile = new ProfileService().GetDefaultProfile(Spec.HolyPriest);

            // Act
            var profile = await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual("Beitaky", profile.Name);
            Assert.AreEqual(Race.NoRace, profile.Race);
            Assert.AreEqual("torghast", profile.Server);
            Assert.AreEqual("us", profile.Region);
            Assert.AreEqual(Spec.HolyPriest, profile.Spec);
            Assert.AreEqual(Class.Priest, profile.Class);
        }

        [Test]
        public async Task SPS_Applies_Items()
        {
            // Arrange
            var baseProfile = new ProfileService().GetDefaultProfile(Spec.HolyPriest);

            // Act
            var profile = await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);

            // Assert
            Assert.IsNotNull(profile);
            Assert.IsNotNull(profile.Items);
            Assert.LessOrEqual(80, profile.Items.Count);
        }

        [Test]
        public async Task SPS_Applies_Stats()
        {
            // Arrange
            var baseProfile = new ProfileService().GetDefaultProfile(Spec.HolyPriest);
            IGameStateService gameStateService = new GameStateService();

            IConstantsService constantsService = new ConstantsService();
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine("TestData", "BaseTests_constants.json")));

            // Act
            var profile = await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);
            var gameState = gameStateService.CreateValidatedGameState(profile, constants);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(2982.0d, gameStateService.GetIntellect(gameState));
            Assert.AreEqual(812.0d, gameStateService.GetVersatilityRating(gameState));
            Assert.AreEqual(237.0d, gameStateService.GetCriticalStrikeRating(gameState));
            Assert.AreEqual(426.0d, gameStateService.GetHasteRating(gameState));
            Assert.AreEqual(100.0d, gameStateService.GetMasteryRating(gameState));
        }

        [Test]
        public async Task SPS_Applies_Talents()
        {
            // Arrange
            var baseProfile = new ProfileService().GetDefaultProfile(Spec.HolyPriest);

            // Act
            var profile = await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);

            // Assert
            Assert.IsNotNull(profile);
            Assert.IsNotNull(profile.Talents);
            Assert.Greater(profile.Talents.Count, 0);
        }
    }
}
