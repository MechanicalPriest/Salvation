using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Profile;
using SimcProfileParser.Interfaces;
using SimcProfileParser.Model.Generated;
using SimcProfileParser.Model.Profile;
using System.Collections.Generic;
using System.Threading.Tasks;
using ItemModType = Salvation.Core.Constants.Data.ItemModType;

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
            foreach (var soulbind in simcProfile.Soulbinds)
            {
                var newSoulbind = new SoulbindProfile();

                // Add the active soulbind spells
                foreach (var soulbindSpell in soulbind.SoulbindSpells)
                {
                    newSoulbind.ActiveSoulbinds.Add((Soulbind)soulbindSpell);
                }

                // Add the active conduits
                foreach (var conduit in soulbind.SocketedConduits)
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
            if (simcProfile.Covenant != null)
            {
                cp.Covenant = simcProfile.Covenant.ToLower() switch
                {
                    "kyrian" => Covenant.Kyrian,
                    "night fae" => Covenant.NightFae,
                    "necrolord" => Covenant.Necrolord,
                    "venthyr" => Covenant.Venthyr,
                    _ => Covenant.None,
                };
            }

            // Set renown
            cp.Renown = simcProfile.Renown;

            profile.Covenant = cp;
        }

        internal void ApplyItems(PlayerProfile profile, IList<SimcItem> items)
        {
            profile.Items = new List<Item>();

            foreach (var item in items)
            {
                var newItem = new Item
                {
                    ItemId = item.ItemId,
                    Name = item.Name,
                    ItemLevel = item.ItemLevel,
                    Slot = (InventorySlot)item.InventoryType,
                    ItemType = (ItemType)item.ItemClass,
                    ItemSubType = item.ItemSubClass
                };

                // Add the items mods
                foreach (var mod in item.Mods)
                {
                    var newMod = new ItemMod()
                    {
                        StatRating = mod.StatRating,
                        Type = (ItemModType)mod.Type
                    };

                    newItem.Mods.Add(newMod);
                }

                // Add the items gems
                foreach (var gem in item.Gems)
                {
                    var newGem = new ItemGem()
                    {
                        StatRating = gem.StatRating,
                        Type = (ItemModType)gem.Type
                    };

                    newItem.Gems.Add(newGem);
                }

                // Add the items effects
                foreach (var effect in item.Effects)
                {
                    var newEffect = new ItemEffect()
                    {
                        EffectId = effect.EffectId,
                        Type = effect.Type,
                        CooldownDuration = effect.CooldownDuration,
                        CooldownGroup = effect.CooldownGroup,
                        CooldownGroupDuration = effect.CooldownGroupDuration,
                        //Spell = effect.Spell // Populate this based on what we actually need later
                    };

                    newItem.Effects.Add(newEffect);
                }

                profile.Items.Add(newItem);
            }
        }
    }
}
