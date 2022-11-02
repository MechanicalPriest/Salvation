using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PrayerOfHealing : SpellService, ISpellService<IPrayerOfHealingSpellService>
    {
        private readonly ISpellService<IRenewSpellService> _renewSpellService;

        public PrayerOfHealing(IGameStateService gameStateService,
            ISpellService<IRenewSpellService> renewSpellService)
            : base(gameStateService)
        {
            Spell = Spell.PrayerOfHealing;
            _renewSpellService = renewSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;
            var healingSp = spellData.GetEffect(105332).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            averageHeal *= GetSanctifiedPrayersMultiplier(gameState);

            // At least one target healed gets bonus healing from Prayerful Litany
            var averageFirstHeal = averageHeal
                * GetPrayerfulLitanyMultiplier(gameState);

            return averageFirstHeal + averageHeal * Math.Max(GetNumberOfHealingTargets(gameState, spellData) - 1, 0);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            double fillerCastTime = hastedCastTime == 0
                ? hastedGcd
                : hastedCastTime;

            double maximumPotentialCasts = 60d / fillerCastTime;

            return maximumPotentialCasts;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // PoH stores its number of targets in 288930.BaseValue
            var numTargets = spellData.GetEffect(288930).BaseValue;

            return numTargets;
        }

        public override double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            var hastedCT = base.GetHastedCastTime(gameState, spellData);

            hastedCT *= this.GetUnwaveringWillMultiplier(gameState);

            hastedCT *= this.GetPrayerCircleCastTimeMultiplier(gameState);

            return hastedCT;
        }

        internal double GetPrayerfulLitanyMultiplier(GameState gameState)
        {
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.PrayerfulLitany);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.PrayerfulLitany);

                multi += talentSpellData.GetEffect(1028559).BaseValue / 100;
            }

            return multi;
        }

        internal double GetSanctifiedPrayersMultiplier(GameState gameState)
        {
            // A basic santified prayers modifier is the percentage of PoH casts inside the SP window
            // This doesn't increase/decrease with Sanc casts, see #192
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.SanctifiedPrayers);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.SanctifiedPrayersBuff);

                var sanctifiedPrayersUptime = _gameStateService.GetPlaystyle(gameState, "SanctifiedPrayersUptime");

                if (sanctifiedPrayersUptime == null)
                    throw new ArgumentOutOfRangeException("SanctifiedPrayersUptime", $"SanctifiedPrayersUptime needs to be set.");

                multi += talentSpellData.GetEffect(288402).BaseValue / 100 * sanctifiedPrayersUptime.Value;
            }

            return multi;
        }

        internal double GetPrayerCircleCastTimeMultiplier(GameState gameState)
        {
            // A basic prayer circle modifier is the percentage of PoH casts inside the PC window
            // This doesn't increase/decrease with Sanc casts, see #192
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.PrayerCircle);

            if (talent != null && talent.Rank > 0)
            {
                // Figure out what percentage of buffs the calling spell gets
                var prayerCircleUptime = _gameStateService.GetPlaystyle(gameState, "PrayerCircleUptime");

                if (prayerCircleUptime == null)
                    throw new ArgumentOutOfRangeException("PrayerCircleUptime", $"PrayerCircleUptime needs to be set.");

                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.PrayerCircleBuff);

                // Divide this by actual casts to get the average multiplier per cast
                var castTimeReduction = talentSpellData.GetEffect(809046).BaseValue / 100 * -1;

                multi -= castTimeReduction * prayerCircleUptime.Value;
            }

            return multi;
        }

        internal double GetPrayerCircleManaReductionMultiplier(GameState gameState)
        {
            // A basic prayer circle modifier is the percentage of PoH casts inside the PC window
            // This doesn't increase/decrease with Sanc casts, see #192
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.PrayerCircle);

            if (talent != null && talent.Rank > 0)
            {
                // Figure out what percentage of buffs the calling spell gets
                var prayerCircleUptime = _gameStateService.GetPlaystyle(gameState, "PrayerCircleUptime");

                if (prayerCircleUptime == null)
                    throw new ArgumentOutOfRangeException("PrayerCircleUptime", $"PrayerCircleUptime needs to be set.");

                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.PrayerCircleBuff);

                // Divide this by actual casts to get the average multiplier per cast
                var manaReduction = talentSpellData.GetEffect(912580).BaseValue / 100 * -1;

                multi -= manaReduction * prayerCircleUptime.Value;
            }

            return multi;
        }

        public override double GetActualManaCost(GameState gameState, BaseSpellData spellData = null)
        {
            var manaCost = base.GetActualManaCost(gameState, spellData);

            manaCost *= GetPrayerCircleManaReductionMultiplier(gameState);

            return manaCost;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            if (_gameStateService.GetTalent(gameState, Spell.RevitalizingPrayers).Rank > 0)
            {
                var procsPerMinute = GetRevitalizingPrayersCpm(gameState, spellData);
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.RevitalizingPrayers);

                // We need to add the 0-cost renews:
                var renewSpellData = _gameStateService.GetSpellData(gameState, Spell.Renew);

                // Force the correct duration
                var duration = talentSpellData.GetEffect(1028558).BaseValue * 1000;
                if(_gameStateService.GetTalent(gameState, Spell.RapidRecovery).Rank > 0)
                {
                    var rapidRecoverySpellData = _gameStateService.GetSpellData(gameState, Spell.RapidRecovery);
                    duration -= rapidRecoverySpellData.GetEffect(1039631).BaseValue * 1000 * -1;
                }

                renewSpellData.ManaCost = 0;
                renewSpellData.Gcd = 0;
                renewSpellData.BaseCastTime = 0;
                renewSpellData.Overrides[Override.Duration] = duration;
                renewSpellData.Overrides[Override.NumberOfHealingTargets] = 1;
                renewSpellData.Overrides[Override.CastsPerMinute] = procsPerMinute; // Force the number of cpm

                // grab the result of the spell cast
                var renewResult = _renewSpellService.GetCastResults(gameState, renewSpellData);

                result.AdditionalCasts.Add(renewResult);
            }

            return result;
        }

        internal double GetRevitalizingPrayersCpm(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var cpm = 0d;

            if (_gameStateService.GetTalent(gameState, Spell.RevitalizingPrayers).Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.RevitalizingPrayers);

                // Calculate the CPM. It's a percentage chance per target healed by PoH.
                var numTargetsPerCast = GetNumberOfHealingTargets(gameState, spellData);
                var procChance = talentSpellData.GetEffect(1028557).BaseValue / 100;

                var procsPerCast = numTargetsPerCast * procChance;

                cpm = GetActualCastsPerMinute(gameState, spellData) * procsPerCast;
            }

            return cpm;
        }

        public override double GetRenewTicksPerMinute(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var renewTicksPerMinute = 0d;

            if (_gameStateService.GetTalent(gameState, Spell.RevitalizingPrayers).Rank > 0)
            {
                var procsPerMinute = GetRevitalizingPrayersCpm(gameState, spellData);
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.RevitalizingPrayers);
                var renewSpellData = _gameStateService.GetSpellData(gameState, Spell.Renew);

                renewSpellData.Duration = talentSpellData.GetEffect(1028558).BaseValue * 1000;
                renewSpellData.Overrides[Override.NumberOfHealingTargets] = 1;
                renewSpellData.Overrides[Override.CastsPerMinute] = procsPerMinute; // Force the number of cpm

                renewTicksPerMinute = _renewSpellService.GetRenewTicksPerMinute(gameState, renewSpellData);

                _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Revitalizing Prayers renew CPM: {procsPerMinute:N3}.");
            }

            return renewTicksPerMinute;
        }

        public override double GetRenewUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if (_gameStateService.GetTalent(gameState, Spell.RevitalizingPrayers).Rank > 0)
            {
                var procsPerMinute = GetRevitalizingPrayersCpm(gameState, spellData);

                var groupSize = _gameStateService.GetPlaystyle(gameState, "GroupSize");

                if (groupSize == null)
                    throw new ArgumentOutOfRangeException("GroupSize", $"GroupSize needs to be set.");

                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.RevitalizingPrayers);

                var duration = talentSpellData.GetEffect(1028558).BaseValue;
                var uptime = procsPerMinute * duration / groupSize.Value / 60;

                return uptime;
            }

            return 0;
        }
    }
}
