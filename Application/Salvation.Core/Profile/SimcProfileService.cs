using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Profile.Model;
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
        private readonly IProfileService _profileService;

        public SimcProfileService(ISimcGenerationService simcGenerationService,
            IProfileService profileService)
        {
            _simcGenerationService = simcGenerationService;
            _profileService = profileService;
        }

        public async Task<PlayerProfile> ApplySimcProfileAsync(string simcAddonString, PlayerProfile profile)
        {
            var simcProfile = await _simcGenerationService.GenerateProfileAsync(simcAddonString);

            ApplyCharacterDetails(profile, simcProfile.ParsedProfile);
            ApplyCovenant(profile, simcProfile.ParsedProfile);
            ApplyItems(profile, simcProfile.GeneratedItems);

            return _profileService.ValidateProfile(profile);
        }

        internal void ApplyCharacterDetails(PlayerProfile profile, SimcParsedProfile parsedProfile)
        {
            profile.Name = parsedProfile.Name;
            profile.Server = parsedProfile.Server;
            profile.Region = parsedProfile.Region;
            profile.Race = (Race)parsedProfile.RaceId;
            profile.Spec = (Spec)parsedProfile.SpecId;
            profile.Class = (Class)parsedProfile.ClassId;
            profile.Level = parsedProfile.Level;
            // Other fields not included: Role, Simc addon version
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
                    newSoulbind.ActiveTraits.Add((SoulbindTrait)soulbindSpell);
                }

                // Add the active conduits
                foreach (var conduit in soulbind.SocketedConduits)
                {
                    newSoulbind.ActiveConduits.Add((Conduit)conduit.SpellId, (uint)conduit.Rank);
                }

                newSoulbind.Name = soulbind.Name;
                newSoulbind.SoulbindId = soulbind.SoulbindId;

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
                    ItemSubType = item.ItemSubClass,
                    Equipped = item.Equipped
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
                        CooldownGroupDuration = effect.CooldownGroupDuration
                    };

                    // Populate this based on what we actually need
                    if (effect.Spell != null)
                    {
                        var newSpell = new BaseSpellData()
                        {
                            Id = effect.Spell.SpellId
                        };

                        newEffect.Spell = newSpell;
                    }

                    newItem.Effects.Add(newEffect);
                }

                profile.Items.Add(newItem);
            }
        }
    }
}
