using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    internal static class Pontifex
    {
        internal static double GetPontifexMultiplier(GameState gameState, ISpellService spellService)
        {
            var multi = 1.0d;

            var talent = spellService.GameStateService.GetTalent(gameState, Spell.Pontifex);

            if (talent != null && talent.Rank > 0)
            {
                // Required spellservices
                var heal = spellService.GameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.Heal).FirstOrDefault();
                var flashHeal = spellService.GameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.FlashHeal).FirstOrDefault();

                // First get the number of procs per minute from crits
                var cpmFlashHeal = flashHeal.SpellService.GetActualCastsPerMinute(gameState);
                var cpmHeal = heal.SpellService.GetActualCastsPerMinute(gameState);

                var totalTriggerableCpm = cpmFlashHeal + cpmHeal;
                var totalTriggers = totalTriggerableCpm * (spellService.GameStateService.GetCriticalStrikeMultiplier(gameState) - 1);

                // Now remove from that the number that Salvation uses
                totalTriggers = Math.Max(0, totalTriggers - GetPontifexSalvStacksConsumedPerMinute(gameState, spellService.GameStateService));

                // Now figure out what percentage of buffs the calling spell gets
                var percentageBuffsSerenity = spellService.GameStateService.GetPlaystyle(gameState, "PontifexPercentUsageSerenity");

                if (percentageBuffsSerenity == null)
                    throw new ArgumentOutOfRangeException("PontifexPercentUsageSerenity", $"PontifexPercentUsageSerenity needs to be set.");

                var percentageOfBuffs = 0.0d;

                if (spellService.SpellId == (int)Spell.HolyWordSerenity)
                    percentageOfBuffs = percentageBuffsSerenity.Value;
                else if (spellService.SpellId == (int)Spell.HolyWordSanctify)
                    percentageOfBuffs = 1 - percentageBuffsSerenity.Value;
                else
                    return multi;

                // Number of buffs spent on the case
                // This is the least out of either total_triggers * percentage_of_buffs
                // OR: actual_casts * 2 (this accounts for serenity/sanc being able to consume 2 stacks)
                var numBuffsSpent = Math.Min(totalTriggers * percentageOfBuffs, spellService.GetActualCastsPerMinute(gameState) * 2);

                // Extra healing for all buffed casts
                var talentSpellData = spellService.GameStateService.GetSpellData(gameState, Spell.Pontifex);
                var healingMulti = talentSpellData.GetEffect(1028183).TriggerSpell.GetEffect(1028200).BaseValue / 100;
                var extraHealing = numBuffsSpent * healingMulti;

                // Divine this by actual casts to get the average multiplier per cast
                var multiIncrease = extraHealing / spellService.GetActualCastsPerMinute(gameState);
                multi += multiIncrease;
            }

            return multi;
        }

        internal static double GetPontifexMultiplier(this HolyWordSerenity serenity, GameState gameState)
        {
            return GetPontifexMultiplier(gameState, serenity);
        }

        internal static double GetPontifexMultiplier(this HolyWordSanctify sanctify, GameState gameState)
        {
            return GetPontifexMultiplier(gameState, sanctify);
        }

        internal static double GetPontifexMultiplier(this HolyWordSalvation salvation, GameState gameState)
        {
            var multi = 1.0d;

            var talent = salvation.GameStateService.GetTalent(gameState, Spell.Pontifex);

            if (talent != null && talent.Rank > 0)
            {
                var avgSalvStacks = salvation.GameStateService.GetPlaystyle(gameState, "PontifexAverageSalvationStacks");

                if (avgSalvStacks == null)
                    throw new ArgumentOutOfRangeException("PontifexAverageSalvationStacks", $"PontifexAverageSalvationStacks needs to be set.");
                
                var talentSpellData = salvation.GameStateService.GetSpellData(gameState, Spell.Pontifex);
                var healingMulti = talentSpellData.GetEffect(1028183).TriggerSpell.GetEffect(1028200).BaseValue / 100;

                multi += healingMulti * avgSalvStacks.Value;
            }

            return multi;
        }

        internal static double GetPontifexSalvStacksConsumedPerMinute(GameState gameState, IGameStateService gameStateService)
        {
            // TODO: Move this centrally, there's nothing even here that's Serenity specific.
            var stacks = 0.0d;

            // Include salv this way to prevent circular dependency errors.
            var salv = gameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.HolyWordSalvation).FirstOrDefault();

            if (salv != null)
            {
                var avgSalvStacks = gameStateService.GetPlaystyle(gameState, "PontifexAverageSalvationStacks");

                if (avgSalvStacks == null)
                    throw new ArgumentOutOfRangeException("PontifexAverageSalvationStacks", $"PontifexAverageSalvationStacks needs to be set.");

                var salvCpm = salv.SpellService.GetActualCastsPerMinute(gameState, salv.SpellData);

                stacks = avgSalvStacks.Value * salvCpm;
            }

            return stacks;
        }
    }
}
