using Salvation.Core.Constants.Data;
using Salvation.Core.Profile.Model;
using System.Collections.Generic;

namespace Salvation.Core.Interfaces.Profile
{
    public interface IProfileService
    {
        PlayerProfile GetDefaultProfile(Spec spec);
        PlayerProfile CloneProfile(PlayerProfile profile);
        PlayerProfile ValidateProfile(PlayerProfile profile);

        // Below methods should only be used by ProfileService and GameStateService
        void SetSpellCastProfile(PlayerProfile profile, CastProfile castProfile);
        void SetProfileName(PlayerProfile profile, string profileName);
        List<Item> GetEquippedItems(PlayerProfile profile);
        void SetCovenant(PlayerProfile profile, CovenantProfile covenant);
        void AddTalent(PlayerProfile profile, Talent talent);
    }
}
