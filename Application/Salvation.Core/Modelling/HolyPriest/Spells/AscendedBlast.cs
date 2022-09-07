using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class AscendedBlast : SpellService, ISpellService<IAscendedBlastSpellService>
    {
        public AscendedBlast(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.AscendedBlast;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // AB does ST damage and heals a random friendly (5 stack)
            // Coeff2 being 100 = 100%.

            double averageDamage = GetAverageDamage(gameState, spellData);
            var healTransferAmount = spellData.GetEffect(815550).BaseValue / 100d;

            // Doesn't seem to be effected by the healing aura multiplier
            double averageHeal = healTransferAmount * averageDamage;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip (Heal): {healTransferAmount}% of Dmg");

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;

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
                var conduitData = _gameStateService.GetSpellData(gameState, Spell.CourageousAscension);
                var damageMulti = (1d + (conduitData.ConduitRanks[rank] / 100));
                _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Applying Courageous Ascension conduit (r{rank + 1}): {damageMulti:0.##}");

                averageDamage *= damageMulti;
            }

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##}");

            averageDamage *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

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
            maximumPotentialCasts *= boonCPM;

            return maximumPotentialCasts;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMinimumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            // Ascended Blast Spellid doesnt have the "right" type, heal component does
            var healData = _gameStateService.GetSpellData(gameState, Spell.AscendedBlastHeal);

            return base.TriggersMastery(gameState, healData);
        }
    }
}
