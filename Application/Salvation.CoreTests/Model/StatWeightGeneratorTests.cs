using NUnit.Framework;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Modelling;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Salvation.CoreTests.Model
{
    [TestFixture]
    class StatWeightGeneratorTests
    {
        public PlayerProfile GetBaseProfile()
        {
            var profileGen = new ProfileGenerationService();
            var profile = profileGen.GetDefaultProfile(Spec.HolyPriest);

            return profile;
        }

        public StatWeightGenerator GetDefaultGenerator()
        {
            var sw = new StatWeightGenerator(new ConstantsService());

            return sw;
        }

        [Test]
        public void SWGGenerateNoNull()
        {
            var baseProfile = GetBaseProfile();
            var numAdditionalStats = 100;
            var sw = GetDefaultGenerator();

            var result = sw.Generate(baseProfile, numAdditionalStats);

            Assert.IsNotNull(result);
        }

        [Test]
        public void SWGGenerateCreatesProfiles()
        {
            var sw = GetDefaultGenerator();
            var baseProfile = GetBaseProfile();
            var numAdditionalStats = 100;

            var result = sw.GenerateStatProfiles(baseProfile, numAdditionalStats);

            Assert.NotZero(result.Count);
            Assert.IsTrue(result.Contains(baseProfile));
        }

        [Test]
        public void SWGGenerateModelResultsGivesResults()
        {
            var sw = GetDefaultGenerator();
            var baseProfile = GetBaseProfile();
            var numAdditionalStats = 100;

            var result = sw.GenerateStatProfiles(baseProfile, numAdditionalStats);
            var results = sw.GenerateModelResults(result);

            Assert.NotZero(results.Count);
        }

        [Test]
        public void SWGGenerateEffectiveGivesResults()
        {
            var baseProfile = GetBaseProfile();
            var numAdditionalStats = 100;
            var sw = GetDefaultGenerator();

            var results = sw.Generate(baseProfile, numAdditionalStats,
                StatWeightGenerator.StatWeightType.EffectiveHealing);

            Assert.NotNull(results);
            Assert.NotZero(results.Results.Count);
            Assert.AreEqual(results.Name, "Effective Healing");
        }

        [Test]
        public void SWGGenerateRawGivesResults()
        {
            var baseProfile = GetBaseProfile();
            var numAdditionalStats = 100;
            var sw = GetDefaultGenerator();

            var results = sw.Generate(baseProfile, numAdditionalStats,
                StatWeightGenerator.StatWeightType.RawHealing);

            Assert.NotNull(results);
            Assert.NotZero(results.Results.Count);
            Assert.AreEqual(results.Name, "Raw Healing");
        }
    }
}
