using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class HolyWordSalvation : SpellService, ISpellService<IHolyWordSalvationSpellService>
    {
        private readonly ISpellService<IHolyWordSerenitySpellService> _serenitySpellService;
        private readonly ISpellService<IHolyWordSanctifySpellService> _holyWordSanctifySpellService;
        private readonly ISpellService<IRenewSpellService> _renewSpellService;
        private readonly ISpellService<IPrayerOfMendingSpellService> _prayerOfMendingSpellService;

        public HolyWordSalvation(IGameStateService gameStateService,
            ISpellService<IHolyWordSerenitySpellService> serenitySpellService,
            ISpellService<IHolyWordSanctifySpellService> holyWordSanctifySpellService,
            ISpellService<IRenewSpellService> renewSpellService,
            ISpellService<IPrayerOfMendingSpellService> prayerOfMendingSpellService)
            : base(gameStateService)
        {
            Spell = Spell.HolyWordSalvation;
            _serenitySpellService = serenitySpellService;
            _holyWordSanctifySpellService = holyWordSanctifySpellService;
            _renewSpellService = renewSpellService;
            _prayerOfMendingSpellService = prayerOfMendingSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healingSp = spellData.GetEffect(709207).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Salv is (60 + (SerenityCPM + SancCPM) * SalvCDR) / (CastTime + Cooldown) + 1 / (FightLength / 60)
            // Essentially the CDR per minute is 60 + the CDR from holy words.

            // TODO: Add sanc here properly once implemented
            var cpmSerenity = _serenitySpellService.GetActualCastsPerMinute(gameState);
            var cpmSanctify = _holyWordSanctifySpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData);
            var hastedCT = GetHastedCastTime(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            var hwCDRSerenity = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.HolyWordSerenity);
            var hwCDRSanctify = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.HolyWordSanctify);

            double salvCDRPerMin = cpmSerenity * hwCDRSerenity +
                cpmSanctify * hwCDRSanctify;
            double maximumPotentialCasts = (60d + salvCDRPerMin) / (hastedCT + hastedCD)
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            // We need to add a 0-cost renew:
            var renewSpellData = _gameStateService.GetSpellData(gameState, Spell.Renew);

            renewSpellData.ManaCost = 0;
            renewSpellData.Gcd = 0;
            renewSpellData.BaseCastTime = 0;
            renewSpellData.Overrides[Override.NumberOfHealingTargets] = GetNumberOfHealingTargets(gameState, spellData);
            renewSpellData.Overrides[Override.CastsPerMinute] = GetActualCastsPerMinute(gameState, spellData); // Force the number of cpm

            // grab the result of the spell cast
            var renewResult = _renewSpellService.GetCastResults(gameState, renewSpellData);

            result.AdditionalCasts.Add(renewResult);

            var pomSpellData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfMending);

            pomSpellData.ManaCost = 0;
            pomSpellData.Gcd = 0;
            pomSpellData.BaseCastTime = 0;
            pomSpellData.BaseCooldown = 0;
            pomSpellData.Overrides[Override.NumberOfHealingTargets] = GetNumberOfHealingTargets(gameState, spellData);
            pomSpellData.Overrides[Override.ResultMultiplier] = 2; // Force the number of stacks
            pomSpellData.Overrides[Override.CastsPerMinute] = GetActualCastsPerMinute(gameState, spellData); // Force the number of cpm

            // grab the result of the spell cast
            var pomResult = _prayerOfMendingSpellService.GetCastResults(gameState, pomSpellData);
            result.AdditionalCasts.Add(pomResult);

            return result;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            // TODO: Clamp to raid size?
            return double.MaxValue;
        }
    }
}
