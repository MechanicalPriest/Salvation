using Salvation.Core.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salvation.Explorer.Modelling
{
    class CovenantComparisons
    {
        // The goal for this class is to build a bunch of different profiles to 
        // show how the covenant abilities work in various scenarios
        // For each profile:
        // 1. Add covenant
        // 2. Update the efficiency usage

        public BaseProfile GetBaseProfile()
        {
            BaseProfile profile;

            profile = DefaultProfiles.GetDefaultProfile(Core.Models.Spec.HolyPriest);

            return profile;
        }

        public BaseModelResults GetBaseResult()
        {
            var profile = GetBaseProfile();

            var constants = new ConstantsService().LoadConstantsFromFile();

            var holyPriest = new HolyPriestModel(constants, profile);

            return holyPriest.GetResults();
        }

        public BaseModelResults GetMindgamesResults()
        {
            var profile = GetBaseProfile();

            profile.Covenant = Covenant.Venthyr;
            profile.Name = "Mindgames";

            var mindgamesEfficiency = profile.Casts.Where(
                c => c.SpellId == (int)HolyPriestModel.SpellIds.MindGames).FirstOrDefault();

            if (mindgamesEfficiency == null)
                throw new ArgumentNullException("mindgames");

            mindgamesEfficiency.Efficiency = 1m;
            mindgamesEfficiency.OverhealPercent = 0m;

            var constants = new ConstantsService().LoadConstantsFromFile();

            var holyPriest = new HolyPriestModel(constants, profile);

            return holyPriest.GetResults();
        }

        public BaseModelResults GetFaeGuardiansDROnlyResults()
        {
            var profile = GetBaseProfile();

            profile.Covenant = Covenant.NightFae;
            profile.Name = "Fae Guardians - DR only 4k DTPS";

            var fgEfficiency = profile.Casts.Where(
                c => c.SpellId == (int)HolyPriestModel.SpellIds.FaeGuardians).FirstOrDefault();

            if (fgEfficiency == null)
                throw new ArgumentNullException("fae guardians");

            fgEfficiency.Efficiency = 1m;
            fgEfficiency.OverhealPercent = 0m;

            // Remove any DH casts
            var dhEfficiency = profile.Casts.Where(
                c => c.SpellId == (int)HolyPriestModel.SpellIds.FaeGuardians).FirstOrDefault();

            dhEfficiency.Efficiency = 0m;

            var constants = new ConstantsService().LoadConstantsFromFile();

            var holyPriest = new HolyPriestModel(constants, profile);

            return holyPriest.GetResults();
        }

        public BaseModelResults GetFaeGuardiansHymnCDRResults()
        {
            var profile = GetBaseProfile();

            profile.Covenant = Covenant.NightFae;
            profile.Name = "Fae Guardians - DR + Hymn";

            var fgEfficiency = profile.Casts.Where(
                c => c.SpellId == (int)HolyPriestModel.SpellIds.FaeGuardians).FirstOrDefault();

            if (fgEfficiency == null)
                throw new ArgumentNullException("fae guardians");

            fgEfficiency.Efficiency = 1m;
            fgEfficiency.OverhealPercent = 0m;

            var constants = new ConstantsService().LoadConstantsFromFile();

            var holyPriest = new HolyPriestModel(constants, profile);

            return holyPriest.GetResults();
        }
    }
}
