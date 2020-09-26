using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class AscendedBlast : SpellService, IAscendedBlastSpellService
    {
        public AscendedBlast(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)SpellIds.AscendedBlast;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.AscendedBlast);

            // AB does ST damage and heals a random friendly (5 stack)
            // Coeff2 being 100 = 100%.

            decimal averageDamage = GetAverageDamage(gameState, spellData, moreData);

            decimal averageHeal = (spellData.Coeff2 / 100)
                * averageDamage;

            _journal.Entry($"[{spellData.Name}] Tooltip (Heal): {spellData.Coeff2}% of Dmg");

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.AscendedBlast);

            var holyPriestAuraDamageBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            // coeff1 * int * hpriest dmg mod * vers
            decimal averageDamage = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            // Apply Courageous Ascension conduit.
            if (_gameStateService.IsConduitActive(gameState, Conduit.CourageousAscension))
            {
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.CourageousAscension);
                var conduitData = _gameStateService.GetConduitData(gameState, Conduit.CourageousAscension);
                var damageMulti = (1 + (conduitData.Ranks[rank] / 100));
                _journal.Entry($"[{spellData.Name}] Applying Courageous Ascension conduit (r{rank + 1}): {damageMulti:0.##}");

                averageDamage *= damageMulti;
            }

            _journal.Entry($"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##}");

            averageDamage *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.AscendedBlast);

            if (moreData == null)
                throw new ArgumentNullException("moreData");

            if (!moreData.ContainsKey("BoonOfTheAscended.CPM"))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain BoonOfTheAscended.CPM");

            var boonCPM = moreData["BoonOfTheAscended.CPM"];

            if (!moreData.ContainsKey("BoonOfTheAscended.Duration"))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain BoonOfTheAscended.Duration");

            var allowedDuration = moreData["BoonOfTheAscended.Duration"];

            var hastedCooldown = GetHastedCooldown(gameState, spellData, moreData);
            var hastedGcd = GetHastedGcd(gameState, spellData, moreData);

            // Initial cast, and divide the remaining duration up by cooldown for remaining casts
            decimal maximumPotentialCasts = 1 + (allowedDuration - hastedGcd) / hastedCooldown;

            // This is the maximum potential casts per Boon CD
            maximumPotentialCasts = maximumPotentialCasts * boonCPM;

            return maximumPotentialCasts;
        }
    }
}
