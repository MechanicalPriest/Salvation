using Salvation.Core.Constants.Data;
using Salvation.Core.Profile;
using System.Collections.Generic;

namespace Salvation.Core.Interfaces.Profile
{
    public interface IProfileService
    {
        PlayerProfile GetDefaultProfile(Spec spec);
        PlayerProfile CloneProfile(PlayerProfile profile);


        void AddConduit(PlayerProfile profile, Conduit conduit, uint rank);
        void RemoveConduit(PlayerProfile profile, Conduit conduit);
        void AddTalent(PlayerProfile profile, Talent talent);
        void RemoveTalent(PlayerProfile profile, Talent talent);
        void SetCovenant(PlayerProfile profile, Covenant covenant, bool cleanupCovenantData = true);
        void RemoveCovenantData(PlayerProfile profile);
        void SetSpellCastProfile(PlayerProfile profile, CastProfile castProfile);
        void SetProfileName(PlayerProfile profile, string profileName);
        void EquipItem(PlayerProfile profile, Item item);
        List<Item> GetEquippedItems(PlayerProfile profile);
    }
}
