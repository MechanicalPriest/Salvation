﻿using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Profile;
using SimcProfileParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var basePath = @"Profile" + Path.DirectorySeparatorChar + "TestData";
            _profileStringBeitaky = await File.ReadAllTextAsync(
                Path.Combine(basePath, "Beitaky.simc"));

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
            Assert.AreEqual(Conduit.ShiningRadiance, baseProfile.Covenant.AvailableConduits.First().Key);
            Assert.AreEqual(1, baseProfile.Covenant.AvailableConduits.First().Value);
            // Soulbinds
            Assert.LessOrEqual(2, baseProfile.Covenant.Soulbinds.Count);
            Assert.IsTrue(baseProfile.Covenant.Soulbinds.First().IsActive);
            Assert.AreEqual("pelagos:7", baseProfile.Covenant.Soulbinds.First().Name);
        }
    }
}
