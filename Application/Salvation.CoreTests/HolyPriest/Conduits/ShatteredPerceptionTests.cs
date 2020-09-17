using NUnit.Framework;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Salvation.CoreTests.HolyPriest.Conduits
{
    [TestFixture]
    class ShatteredPerceptionTests
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

        public HolyPriestModel GetBaseModel()
        {
            var profile = GetBaseProfile();

            var model = new HolyPriestModel(GetConstants(), profile);
            
            return model;
        }

        public HolyPriestModel GetChangedModel()
        {
            var profile = GetBaseProfile();

            profile.Conduits.Add(Conduit.ShatteredPerceptions, 0);

            var model = new HolyPriestModel(GetConstants(), profile);

            return model;
        }

        [Test]
        public void AdditionalDurationIsAdded()
        {
            var baseModel = GetBaseModel();
            var baseResults = baseModel.GetResults();

            var changedModel = GetChangedModel();
            var changedResults = changedModel.GetResults();

            var baseMindGames = baseResults.SpellCastResults.Where(s => s.SpellName == "Mindgames").FirstOrDefault();
            var changedMindGames = changedResults.SpellCastResults.Where(s => s.SpellName == "Mindgames").FirstOrDefault();

            Assert.Greater(changedMindGames.Duration, baseMindGames.Duration);
        }

        [Test]
        public void AdditionalDamageIsAdded()
        {
            var baseModel = GetBaseModel();
            var baseResults = baseModel.GetResults();

            var changedModel = GetChangedModel();
            var changedResults = changedModel.GetResults();

            var baseMindGames = baseResults.SpellCastResults.Where(s => s.SpellName == "Mindgames").FirstOrDefault();
            var changedMindGames = changedResults.SpellCastResults.Where(s => s.SpellName == "Mindgames").FirstOrDefault();

            var conduitData = changedModel.GetConduitDataById((int)Conduit.ShatteredPerceptions);
            var rank = changedModel.Profile.Conduits[Conduit.ShatteredPerceptions];

            var changeAmount = conduitData.Ranks[rank] / 100;

            Assert.AreEqual(baseMindGames.Damage * (1 + changeAmount), changedMindGames.Damage);
        }
    }
}
