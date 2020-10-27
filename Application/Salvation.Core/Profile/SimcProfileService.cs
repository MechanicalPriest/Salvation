using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Profile;
using SimcProfileParser.Interfaces;
using SimcProfileParser.Model.Generated;
using SimcProfileParser.Model.Profile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.Profile
{
    public class SimcProfileService : ISimcProfileService
    {
        private readonly ISimcGenerationService _simcGenerationService;
        private readonly IProfileGenerationService _profileGenerationService;

        public SimcProfileService(ISimcGenerationService simcGenerationService,
            IProfileGenerationService profileGenerationService)
        {
            _simcGenerationService = simcGenerationService;
            _profileGenerationService = profileGenerationService;
        }

        public async Task ApplySimcProfileAsync(string simcAddonString, PlayerProfile profile)
        {
            var simcProfile = await _simcGenerationService.GenerateProfileAsync(simcAddonString);

            ApplyCharacterDetails(profile, simcProfile.ParsedProfile);
            ApplyCovenant(profile, simcProfile.ParsedProfile);
            ApplyItems(profile, simcProfile.GeneratedItems);
        }

        internal void ApplyCharacterDetails(PlayerProfile profile, SimcParsedProfile parsedProfile)
        {
            profile.Name = parsedProfile.Name;
            profile.Server = parsedProfile.Server;
            profile.Region = parsedProfile.Region;

            // TODO: Spec
            // TODO: Class
            // TODO: Level
            // TODO: Role?
            // TODO: Simc addon version?
        }

        internal void ApplyCovenant(PlayerProfile profile, SimcParsedProfile simcProfile)
        {
            CovenantProfile cp = new CovenantProfile();

            // Set Conduits
            foreach (var conduit in simcProfile.Conduits)
            {
                cp.AvailableConduits.Add((Conduit)conduit.SpellId, conduit.Rank);
            }

            // Set Soulbinds
            foreach(var soulbind in simcProfile.Soulbinds)
            {
                var newSoulbind = new SoulbindProfile();

                // Add the active soulbind spells
                foreach(var soulbindSpell in soulbind.SoulbindSpells)
                {
                    newSoulbind.ActiveSoulbinds.Add((Soulbind)soulbindSpell);
                }

                // Add the active conduits
                foreach(var conduit in simcProfile.Conduits)
                {
                    newSoulbind.ActiveConduits.Add((Conduit)conduit.SpellId, conduit.Rank);
                }

                newSoulbind.Name = soulbind.Name;
                // TODO: Update this once the new version of the library is added
                //newSoulbind.SoulbindId = soulbind.SoulbindId;

                newSoulbind.IsActive = soulbind.IsActive;

                cp.Soulbinds.Add(newSoulbind);
            }

            // Set the covenant
            cp.Covenant = simcProfile.Covenant.ToLower() switch
            {
                "kyrian" => Covenant.Kyrian,
                "night fae" => Covenant.NightFae,
                "necrolord" => Covenant.Necrolord,
                "venthyr" => Covenant.Venthyr,
                _ => Covenant.None,
            };

            // Set renown
            cp.Renown = simcProfile.Renown;

            profile.Covenant = cp;
        }

        internal void ApplyItems(PlayerProfile profile, IList<SimcItem> items)
        {
            foreach(var item in items)
            {

            }
        }
    }
}
