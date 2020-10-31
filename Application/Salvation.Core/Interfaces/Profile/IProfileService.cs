﻿using Salvation.Core.Constants.Data;
using Salvation.Core.Profile;
using System.Collections.Generic;

namespace Salvation.Core.Interfaces.Profile
{
    public interface IProfileService
    {
        PlayerProfile GetDefaultProfile(Spec spec);
        PlayerProfile CloneProfile(PlayerProfile profile);
        PlayerProfile ValidateProfile(PlayerProfile profile);


        void SetCovenant(PlayerProfile profile, Covenant covenant, bool cleanupCovenantData = true);
        void SetSpellCastProfile(PlayerProfile profile, CastProfile castProfile);
        void SetProfileName(PlayerProfile profile, string profileName);
        List<Item> GetEquippedItems(PlayerProfile profile);
    }
}
