using NUnit.Framework;
using Salvation.Core.Profile;
using System;

namespace Salvation.CoreTests.Profile
{
    [TestFixture]
    public class DefaultProfileTests
    {
        private ProfileGenerationService _profileGenerationService;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _profileGenerationService = new ProfileGenerationService();
        }

        public void PGS_Creates_Profile()
        {
            // Arrange
            var spec = Core.Constants.Data.Spec.HolyPriest;

            // Act
            var profile = _profileGenerationService.GetDefaultProfile(spec);

            // Assert
            Assert.NotNull(profile);
            Assert.AreEqual(257, profile.SpecId);
        }

        [Test]
        public void PGS_Empty_Spec_Throws()
        {
            // Arrange
            var spec = Core.Constants.Data.Spec.None;

            // Act
            var methodCall = new TestDelegate(
                () => _profileGenerationService.GetDefaultProfile(spec));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }
    }
}
