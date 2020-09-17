using NUnit.Framework;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Models;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Salvation.CoreTests.Model
{
    [TestFixture]
    class StatWeightGeneratorTests
    {
        public GlobalConstants GetConstants()
        {
            var data = File.ReadAllText(@"constants.json");

            var constants = ConstantsManager.ParseConstants(data);

            return constants;
        }

        public BaseProfile GetBaseProfile()
        {
            var profile = DefaultProfiles.GetDefaultProfile(Core.Models.Spec.HolyPriest);

            profile.Covenant = Covenant.Venthyr;

            return profile;
        }

        public StatWeightGenerator GetDefaultGenerator()
        {
            var baseProfile = GetBaseProfile();
            var numAdditionalStats = 100;
            var sw = new StatWeightGenerator(baseProfile, numAdditionalStats);

            return sw;
        }

        [Test]
        public void StatWeightGeneratorConstructorSavesValues()
        {
            var baseProfile = GetBaseProfile();
            var numAdditionalStats = 100;
            var sw = new StatWeightGenerator(baseProfile, numAdditionalStats);

            Assert.AreEqual(baseProfile, sw.BaselineProfile);
            Assert.AreEqual(numAdditionalStats, sw.AdditionalStats);
        }

        [Test]
        public void StatWeightGeneratorGenerateNoNull()
        {
            var sw = GetDefaultGenerator();

            var result = sw.Generate();

            Assert.IsNotNull(result);
        }

        [Test]
        public void StatWeightGenerateGenerateCreatesProfiles()
        {
            var sw = GetDefaultGenerator();

            var result = sw.GenerateStatProfiles();

            Assert.NotZero(sw.StatProfiles.Count);
            Assert.IsTrue(sw.StatProfiles.Contains(sw.BaselineProfile));
        }
    }
}
