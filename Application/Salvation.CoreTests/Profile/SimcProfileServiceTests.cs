using Newtonsoft.Json;
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
            var baseProfile = new ProfileService().GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);

            // Act
            var profile = await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual("Beitaky", profile.Name);
            Assert.AreEqual(Race.Dwarf, profile.Race);
            Assert.AreEqual("torghast", profile.Server);
            Assert.AreEqual("us", profile.Region);
            Assert.AreEqual(Spec.HolyPriest, profile.Spec);
        }

        [Test]
        public async Task SPS_Applies_Covenant_Properties()
        {
            // Arrange
            var baseProfile = new ProfileService().GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);

            // Act
            var profile = await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);

            // Assert
            Assert.IsNotNull(profile);
            // Covenant
            Assert.IsNotNull(profile.Covenant);
            Assert.AreEqual(Covenant.Kyrian, profile.Covenant.Covenant);
            Assert.AreEqual(40, profile.Covenant.Renown);
            // Conduits
            Assert.LessOrEqual(20, profile.Covenant.AvailableConduits.Count);
            Assert.AreEqual(Conduit.ResonantWords, profile.Covenant.AvailableConduits.First().Key);
            Assert.AreEqual(1, profile.Covenant.AvailableConduits.First().Value);
            // Soulbinds
            Assert.LessOrEqual(2, profile.Covenant.Soulbinds.Count);
            Assert.IsTrue(profile.Covenant.Soulbinds.First().IsActive);
            Assert.AreEqual("pelagos", profile.Covenant.Soulbinds.First().Name);
            Assert.LessOrEqual(1, profile.Covenant.Soulbinds.Skip(1).First().ActiveConduits.Count);
            Assert.AreEqual(Conduit.CourageousAscension, profile.Covenant.Soulbinds.Skip(1).First().ActiveConduits.First().Key); // TODO: Fixed with a newer version of SimcProfileParser
            Assert.AreEqual(1, profile.Covenant.Soulbinds.Skip(1).First().ActiveConduits.First().Value);
        }

        [Test]
        public async Task SPS_Applies_Items()
        {
            // Arrange
            var baseProfile = new ProfileService().GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);

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
            var baseProfile = new ProfileService().GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);
            IGameStateService gameStateService = new GameStateService();

            IConstantsService constantsService = new ConstantsService();
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine("TestData", "BaseTests_constants.json")));

            // Act
            var profile = await _simcProfileService.ApplySimcProfileAsync(_profileStringBeitaky, baseProfile);
            var gameState = new GameState(profile, constants);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(1261.0d, gameStateService.GetIntellect(gameState));
            Assert.AreEqual(812.0d, gameStateService.GetVersatilityRating(gameState));
            Assert.AreEqual(238.0d, gameStateService.GetCriticalStrikeRating(gameState));
            Assert.AreEqual(427.0d, gameStateService.GetHasteRating(gameState));
            Assert.AreEqual(100.0d, gameStateService.GetMasteryRating(gameState));
        }
    }
}
