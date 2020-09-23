using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest.Spells
{
    public class AscendedBlast : SpellService, IAscendedBlastSpellService
    {
        public AscendedBlast(IGameStateService gameStateService,
            IModellingJournal journal)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.AscendedBlast;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.AscendedBlast);

            // AB does ST damage and heals a random friendly (5 stack)
            // Coeff2 being 100 = 100%.

            decimal averageDamage = GetAverageDamage(gameState, spellData);

            decimal averageHeal = (spellData.Coeff2 / 100)
                * averageDamage;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            return averageHeal * spellData.NumberOfHealingTargets;
        }

        public override decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.AscendedBlast);

            var holyPriestAuraDamageBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            // coeff3 * int * hpriest dmg mod * vers
            decimal averageDamage = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            // Apply Courageous Ascension conduit.
            if (gameStateService.IsConduitActive(gameState, Conduit.CourageousAscension))
            {
                var rank = gameStateService.GetConduitRank(gameState, Conduit.CourageousAscension);
                var conduitData = gameStateService.GetConduitData(gameState, Conduit.CourageousAscension);
                var damageMulti = (1 + (conduitData.Ranks[rank] / 100));
                journal.Entry($"[{spellData.Name}] Applying Courageous Ascension conduit (r{rank + 1}): {damageMulti:0.##}");

                averageDamage *= damageMulti;
            }

            journal.Entry($"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##}");

            averageDamage *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageDamage * spellData.NumberOfDamageTargets;
        }

        public AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData, decimal castableTimeframe, decimal boonActualCPM)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            AveragedSpellCastResult result = new AveragedSpellCastResult();

            result.SpellName = spellData.Name;
            result.SpellId = SpellId;

            // TODO: Move this to a Dictionary<string, decimal> style optional params object
            result.CastsPerMinute = GetActualCastsPerMinute(gameState, spellData, castableTimeframe, boonActualCPM);
            result.CastTime = GetHastedCastTime(gameState, spellData);
            result.Cooldown = GetHastedCooldown(gameState, spellData);
            result.Damage = GetAverageDamage(gameState, spellData);
            result.Duration = GetDuration(gameState, spellData);
            result.Gcd = GetHastedGcd(gameState, spellData);
            result.Healing = GetAverageHealing(gameState, spellData);
            result.ManaCost = GetActualManaCost(gameState, spellData);
            result.MaximumCastsPerMinute = GetMaximumCastsPerMinute(gameState, spellData);
            result.NumberOfDamageTargets = GetNumberOfDamageTargets(gameState, spellData);
            result.NumberOfHealingTargets = GetNumberOfHealingTargets(gameState, spellData);
            result.Overhealing = GetAverageOverhealing(gameState, spellData);
            result.RawHealing = GetAverageRawHealing(gameState, spellData);

            if (spellData.IsMasteryTriggered)
            {
                var echoResult = GetHolyPriestMasteryResult(gameState, spellData);
                if (echoResult != null)
                    result.AdditionalCasts.Add(echoResult);
            }

            return result;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            throw new NotImplementedException();
        }

        public decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData, decimal castableTimeframe, decimal boonActualCPM)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.AscendedBlast);

            var hastedCooldown = GetHastedCooldown(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            // Initial cast, and divide the remaining duration up by cooldown for remaining casts
            decimal maximumPotentialCasts = 1 + (castableTimeframe - hastedGcd) / hastedCooldown;

            // This is the maximum potential casts per Boon CD
            maximumPotentialCasts = maximumPotentialCasts * boonActualCPM;

            return maximumPotentialCasts;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            throw new NotImplementedException();
        }
    }
}
