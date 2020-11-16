using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Profile.Model;
using SimcProfileParser.Interfaces;
using SimcProfileParser.Model.Generated;
using SimcProfileParser.Model.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
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
            ApplyTalents(profile, simcProfile.ParsedProfile.Talents);

            return _profileService.ValidateProfile(profile);
        }

        internal void ApplyCharacterDetails(PlayerProfile profile, SimcParsedProfile parsedProfile)
        {
            if(!string.IsNullOrEmpty(parsedProfile.Name))
                profile.Name = parsedProfile.Name;

            if (!string.IsNullOrEmpty(parsedProfile.Server))
                profile.Server = parsedProfile.Server;

            if (!string.IsNullOrEmpty(parsedProfile.Region))
                profile.Region = parsedProfile.Region;

            if(parsedProfile.RaceId != 0)
                profile.Race = (Race)parsedProfile.RaceId;

            if (parsedProfile.SpecId != 0)
                profile.Spec = (Spec)parsedProfile.SpecId;

            if (parsedProfile.ClassId != 0)
                profile.Class = (Class)parsedProfile.ClassId;

            if (parsedProfile.Level != 0)
                profile.Level = parsedProfile.Level;
            // Other fields not included: Role, Simc addon version
        }

        internal void ApplyCovenant(PlayerProfile profile, SimcParsedProfile simcProfile)
        {
            CovenantProfile cp = profile.Covenant;

            // Create a new covenant profile if the covenant has changed
            if (simcProfile.Covenant != null)
            {
                var newCovenant = simcProfile.Covenant.ToLower() switch
                {
                    "kyrian" => Covenant.Kyrian,
                    "night fae" => Covenant.NightFae,
                    "necrolord" => Covenant.Necrolord,
                    "venthyr" => Covenant.Venthyr,
                    _ => Covenant.None,
                };

                if (newCovenant != profile.Covenant.Covenant)
                {
                    cp = new CovenantProfile();
                    cp.Covenant = newCovenant;
                }
            }

            // Set Conduits - wipe them if we have a new set of available conduits
            if (simcProfile.Conduits.Count > 0)
                cp.AvailableConduits = new Dictionary<Conduit, int>();

            foreach (var conduit in simcProfile.Conduits)
            {
                cp.AvailableConduits.Add((Conduit)conduit.SpellId, conduit.Rank);
            }

            // Set Soulbinds
            foreach (var soulbind in simcProfile.Soulbinds)
            {
                var newSoulbind = new SoulbindProfile();

                // Update the existing soulbind if it already exists
                if (soulbind.SoulbindId != 0)
                {
                    var existingSoulbind = cp.Soulbinds.Where(s => s.SoulbindId == soulbind.SoulbindId).FirstOrDefault();
                    if (existingSoulbind != null)
                        newSoulbind = existingSoulbind;
                    else
                    {
                        newSoulbind.SoulbindId = soulbind.SoulbindId;
                        cp.Soulbinds.Add(newSoulbind);
                    }
                }

                if(!String.IsNullOrEmpty(soulbind.Name))
                    newSoulbind.Name = soulbind.Name;

                newSoulbind.IsActive = soulbind.IsActive;

                // Add the active soulbind spells - wipe them if we have new incoming ones
                if (soulbind.SoulbindSpells.Count > 0)
                    newSoulbind.ActiveTraits = new List<SoulbindTrait>();

                foreach (var soulbindSpell in soulbind.SoulbindSpells)
                {
                    newSoulbind.ActiveTraits.Add((SoulbindTrait)soulbindSpell);
                }

                // Add the active conduits - wipe them if we have new incoming ones
                if (soulbind.SocketedConduits.Count > 0)
                    newSoulbind.ActiveConduits = new Dictionary<Conduit, uint>();

                foreach (var conduit in soulbind.SocketedConduits)
                {
                    newSoulbind.ActiveConduits.Add((Conduit)conduit.SpellId, (uint)conduit.Rank);
                }
            }

            // Set renown
            if(simcProfile.Renown != 0)
                cp.Renown = simcProfile.Renown;

            profile.Covenant = cp;
        }

        internal void ApplyItems(PlayerProfile profile, IList<SimcItem> items)
        {
            if (items.Count == 0)
                return;

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
                    newEffect.Spell = GetBaseSpellData(effect.Spell, item.ItemLevel);

                    newItem.Effects.Add(newEffect);
                }

                profile.Items.Add(newItem);
            }
        }

        internal BaseSpellData GetBaseSpellData(SimcSpell spell, int itemLevel)
        {
            // TODO: This currently comes from HolyPriestSpellDataService.cs (Salvation.Utility) - centralise the logic
            if (spell == null)
                return null;

            var newSpell = new BaseSpellData
            {
                Id = spell.SpellId,
                Name = spell.Name,
                MaxRange = spell.MaxRange,
                BaseCastTime = spell.CastTime,
                BaseCooldown = spell.Cooldown,
                ChargeCooldown = spell.ChargeCooldown,
                Charges = spell.Charges,
                Duration = spell.Duration,
                Gcd = spell.Gcd / 1000d,
                ConduitRanks = spell.ConduitRanks,
                Rppm = spell.Rppm
            };

            newSpell.ScaleValues.Add(itemLevel, spell.ScaleBudget);

            // Check if RPPM is modified by spec or haste
            foreach (var rppmMod in spell.RppmModifiers)
            {
                // TODO: This references holy priest specifically, should be a method param based on specId
                if (rppmMod.RppmIsSpecModified && rppmMod.RppmSpec == (uint)Spec.HolyPriest)
                    newSpell.Rppm *= rppmMod.RppmCoefficient;

                if (rppmMod.RppmIsHasted)
                    newSpell.RppmIsHasted = true;
            }

            double manacost = 0;

            if (spell.PowerCosts != null && spell.PowerCosts.Count > 0)
            {
                if (spell.PowerCosts.ContainsKey(0))
                {
                    manacost = spell.PowerCosts[0];
                }

                foreach (var PowerCost in spell.PowerCosts)
                {
                    if (PowerCost.Key.Equals((uint)Spell.HolyPriest))
                    {
                        manacost = PowerCost.Value;
                        break;
                    }
                }
            }

            newSpell.ManaCost = manacost;

            foreach (var effect in spell.Effects)
            {
                var newEffect = GetBaseSpellDataEffect(effect, itemLevel);

                if (newEffect != null)
                    newSpell.Effects.Add(newEffect);
            }

            return newSpell;
        }

        internal BaseSpellDataEffect GetBaseSpellDataEffect(SimcSpellEffect effect, int itemLevel)
        {
            if (effect == null)
                return null;

            var newEffect = new BaseSpellDataEffect()
            {
                Id = effect.Id,
                BaseValue = effect.BaseValue,
                SpCoefficient = effect.SpCoefficient,
                Coefficient = effect.Coefficient,
                TriggerSpellid = effect.TriggerSpellId,
                Amplitude = effect.Amplitude,
                TriggerSpell = GetBaseSpellData(effect.TriggerSpell, itemLevel),
                Type = effect.EffectType,
            };

            return newEffect;
        }

        private void ApplyTalents(PlayerProfile profile, IReadOnlyList<int> talents)
        {
            if (talents.Count == 0)
                return;

            profile.Talents = new List<Talent>();

            for(var i = 0; i < talents.Count; i++)
            {
                if (talents[i] != 0)
                    profile.Talents.Add(HolyPriestTalents[i][talents[i] - 1]);
            }
        }

        internal List<List<Talent>> HolyPriestTalents = new List<List<Talent>>()
        {
            new List<Talent>() { Talent.Enlightenment, Talent.TrailOfLight, Talent.RenewedFaith },
            new List<Talent>() { Talent.AngelsMercy, Talent.BodyAndSoul, Talent.AngelicFeather },
            new List<Talent>() { Talent.CosmicRipple, Talent.GuardianAngel, Talent.Afterlife },
            new List<Talent>() { Talent.PsychicVoice, Talent.Censure, Talent.ShiningForce },
            new List<Talent>() { Talent.SurgeOfLight, Talent.BindingHeal, Talent.PrayerCircle },
            new List<Talent>() { Talent.Benediction, Talent.DivineStar, Talent.Halo },
            new List<Talent>() { Talent.LightOfTheNaaru, Talent.Apotheosis, Talent.HolyWordSalvation },
        };
    }
}
