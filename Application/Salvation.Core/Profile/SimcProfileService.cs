﻿using Salvation.Core.Constants;
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
            ApplyItems(profile, simcProfile.GeneratedItems);
            ApplyTalents(profile, simcProfile.Talents);

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
        public Item CreateItem(SimcItem item)
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

            // Add missing effects for items by building fake effects
            switch(item.ItemId)
            {
                // Consumptive Infusion
                case 184022:

                    var buffSpell = _simcGenerationService.GenerateSpellAsync(new SimcSpellOptions()
                    {
                        ItemInventoryType = item.InventoryType,
                        ItemLevel = item.ItemLevel,
                        ItemQuality = item.Quality,
                        SpellId = (uint)Spell.ConsumptiveInfusionBuff
                    }).Result;

                    item.Effects.Add(new SimcItemEffect() { Spell = buffSpell });
                    break;

                default:;
                    break;
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

            return newItem;
        }

        internal void ApplyItems(PlayerProfile profile, IList<SimcItem> items)
        {
            if (items.Count == 0)
                return;

            profile.Items = new List<Item>();

            foreach (var item in items)
            {
                var newItem = CreateItem(item);

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

            newEffect.ScaleValues.Add(itemLevel, effect.ScaleBudget);

            return newEffect;
        }

        private void ApplyTalents(PlayerProfile profile, IList<SimcTalent> talents)
        {
            if (talents.Count == 0)
                return;

            profile.Talents = new List<Talent>();

            foreach(var talent in talents)
            {
                profile.Talents.Add(new Talent()
                {
                    Spell = (Spell)talent.SpellId,
                    SpellId = (int)talent.SpellId,
                    Rank = talent.Rank
                });
            }
        }
    }
}
