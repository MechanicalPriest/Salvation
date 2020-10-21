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
            SpellId = (int)Spell.AscendedBlast;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.AscendedBlast);

            // AB does ST damage and heals a random friendly (5 stack)
            // Coeff2 being 100 = 100%.

            double averageDamage = GetAverageDamage(gameState, spellData);
            var healTransferAmount = spellData.GetEffect(815550).BaseValue / 100d;

            double averageHeal = healTransferAmount * averageDamage;

            _journal.Entry($"[{spellData.Name}] Tooltip (Heal): {healTransferAmount}% of Dmg");

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.AscendedBlast);

            var holyPriestAuraDamageBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            // coeff1 * int * hpriest dmg mod * vers
            var damageSp = spellData.GetEffect(815465).SpCoefficient;
            var averageDamage = damageSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            // Apply Courageous Ascension conduit.
            if (_gameStateService.IsConduitActive(gameState, Conduit.CourageousAscension))
            {
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.CourageousAscension);
                var conduitData = _gameStateService.GetConduitData(gameState, Conduit.CourageousAscension);
                var damageMulti = (1d + (conduitData.Ranks[rank] / 100));
                _journal.Entry($"[{spellData.Name}] Applying Courageous Ascension conduit (r{rank + 1}): {damageMulti:0.##}");

                averageDamage *= damageMulti;
            }

            _journal.Entry($"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##}");

            averageDamage *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.AscendedBlast);

            if (!spellData.Overrides.ContainsKey(Override.CastsPerMinute))
                throw new ArgumentOutOfRangeException("Overrides", "Does not contain CastsPerMinute");

            var boonCPM = spellData.Overrides[Override.CastsPerMinute];

            if (!spellData.Overrides.ContainsKey(Override.AllowedDuration))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain AllowedDuration");

            var allowedDuration = spellData.Overrides[Override.AllowedDuration];

            var hastedCooldown = GetHastedCooldown(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            // Initial cast, and divide the remaining duration up by cooldown for remaining casts
            var maximumPotentialCasts = 1d + (allowedDuration - hastedGcd) / hastedCooldown;

            // This is the maximum potential casts per Boon CD
            maximumPotentialCasts = maximumPotentialCasts * boonCPM;

            return maximumPotentialCasts;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
