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
                new ProfileGenerationService()
                );
        }

        [Test]
        public async Task SPS_Applies_Basic_Properties()
        {
            // Arrange
            var baseProfile = new ProfileGenerationService().GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);

            // Act
            await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);

            // Assert
            Assert.IsNotNull(baseProfile);
            Assert.AreEqual("Beitaky", baseProfile.Name);
            Assert.AreEqual(Race.Dwarf, baseProfile.Race);
        }

        [Test]
        public async Task SPS_Applies_Covenant_Properties()
        {
            // Arrange
            var baseProfile = new ProfileGenerationService().GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);

            // Act
            await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);

            // Assert
            Assert.IsNotNull(baseProfile);
            // Covenant
            Assert.IsNotNull(baseProfile.Covenant);
            Assert.AreEqual(Covenant.Kyrian, baseProfile.Covenant.Covenant);
            Assert.AreEqual(40, baseProfile.Covenant.Renown);
            // Conduits
            Assert.LessOrEqual(20, baseProfile.Covenant.AvailableConduits.Count);
            Assert.AreEqual(Conduit.ResonantWords, baseProfile.Covenant.AvailableConduits.First().Key);
            Assert.AreEqual(1, baseProfile.Covenant.AvailableConduits.First().Value);
            // Soulbinds
            Assert.LessOrEqual(2, baseProfile.Covenant.Soulbinds.Count);
            Assert.IsTrue(baseProfile.Covenant.Soulbinds.First().IsActive);
            Assert.AreEqual("pelagos", baseProfile.Covenant.Soulbinds.First().Name);
            Assert.LessOrEqual(1, baseProfile.Covenant.Soulbinds.First().ActiveConduits.Count);
            Assert.AreEqual(Conduit.CourageousAscension, baseProfile.Covenant.Soulbinds.First().ActiveConduits.First().Key); // TODO: Fixed with a newer version of SimcProfileParser
            Assert.AreEqual(1, baseProfile.Covenant.Soulbinds.First().ActiveConduits.First().Value);
        }

        [Test]
        public async Task SPS_Applies_Items()
        {
            // Arrange
            var baseProfile = new ProfileGenerationService().GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);

            // Act
            await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);

            // Assert
            Assert.IsNotNull(baseProfile);
            Assert.IsNotNull(baseProfile.Items);
            Assert.LessOrEqual(80, baseProfile.Items.Count);
        }

        [Test]
        public async Task SPS_Applies_Stats()
        {
            // Arrange
            var baseProfile = new ProfileGenerationService().GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);
            IGameStateService gameStateService = new GameStateService();

            IConstantsService constantsService = new ConstantsService();
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine("TestData", "BaseTests_constants.json")));

            // Act
            await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);
            var gameState = new GameState(baseProfile, constants);

            // Assert
            Assert.IsNotNull(baseProfile);
            Assert.AreEqual(1261.0d, gameStateService.GetIntellect(gameState));
            Assert.AreEqual(812.0d, gameStateService.GetVersatilityRating(gameState));
            Assert.AreEqual(238.0d, gameStateService.GetCriticalStrikeRating(gameState));
            Assert.AreEqual(427.0d, gameStateService.GetHasteRating(gameState));
            Assert.AreEqual(100.0d, gameStateService.GetMasteryRating(gameState));
        }
    }
}
