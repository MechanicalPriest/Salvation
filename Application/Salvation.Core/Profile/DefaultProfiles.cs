﻿using Salvation.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Profile
{
    /// <summary>
    /// TODO: Move this somewhere nicer, essentially just boilerplate code to help testing and development
    /// </summary>
    public class DefaultProfiles
    {
        public static BaseProfile GetDefaultProfile(int specId)
        {
            var spec = (Spec)specId;

            return GetDefaultProfile(spec);
        }

        public static BaseProfile GetDefaultProfile(Spec spec)
        {
            switch (spec)
            {
                case Spec.HolyPriest:
                    return generateHolyPriestProfile();

                case Spec.None:
                default:
                    throw new ArgumentOutOfRangeException("specid", "SpecID must be a valid supported spec");
            }
        }

        private static BaseProfile generateHolyPriestProfile()
        {
            var basicProfile = new BaseProfile()
            {
                SpecId = Spec.HolyPriest,
                Intellect = 1001,
                MasteryRating = 242,
                VersatilityRating = 139,
                HasteRating = 242,
                CritRating = 268,
                Casts = new List<CastProfile>()
                {
                    // SpellId, Efficiency, Overheal
                    new CastProfile(2060, 0.0603m, 0.1084m), // FH
                    new CastProfile(2061, 0.0664m, 0.3054m), // Heal
                    new CastProfile(139, 0.0364m, 0.3643m), // Renew
                    new CastProfile(33076, 0.9056m, 0.0219m), // PrayerOfMending
                    new CastProfile(596, 0.2931m, 0.2715m), // PrayerOfHealing
                    new CastProfile(132157, 0.0034m, 0.15m), // HolyNova
                    new CastProfile(2050, 0.677m, 0.1515m), // HolyWordSerenity
                    new CastProfile(34861, 0.7822m, 0.3234m), // HolyWordSanctify
                    new CastProfile(64843, 0.8805m, 0.314m), // DivineHymn
                    new CastProfile(32546, 0m, 0m), // BindingHeal
                    new CastProfile(204883, 0.8653m, 0.1417m), // CircleOfHealing
                    new CastProfile(110744, 0m, 0m), // DivineStar
                    new CastProfile(120517, 0.7596m, 0.3658m), // Halo
                    new CastProfile(265202, 0.874m, 0.3142m), // HolyWordSalvation
                    new CastProfile(238136, 0m, 0.2332m), // CosmicRipple
                    new CastProfile(17, 0m, 0.0m), // PowerWordShield
                    new CastProfile(77485, 0, 0.4224m), // Echo
                },
                T15Talent = 193155,
                FightLengthSeconds = 397
            };

            return basicProfile;
        }
    }
}