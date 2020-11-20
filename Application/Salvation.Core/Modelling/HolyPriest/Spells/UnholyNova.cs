using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class UnholyNova : SpellService, ISpellService<IUnholyNovaSpellService>
    {
        private readonly ISpellService<IUnholyTransfusionSpellService> _unholyTransfuionSpellService;

        public UnholyNova(IGameStateService gameStateService,
            ISpellService<IUnholyTransfusionSpellService> unholyTransfuionSpellService)
            : base(gameStateService)
        {
            Spell = Spell.UnholyNova;
            _unholyTransfuionSpellService = unholyTransfuionSpellService;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            // Apply the transufion DoT/HoT
            var unholyTransfusionSpellData = _gameStateService.GetSpellData(gameState, Spell.UnholyTransfusionDoT);

            var uhtResults = _unholyTransfuionSpellService.GetCastResults(gameState, unholyTransfusionSpellData);

            result.AdditionalCasts.Add(uhtResults);

            return result;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healingSp = spellData.GetEffect(814521).TriggerSpell.GetEffect(875387).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd)
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // UN stores its max healing targets in effect 844015 #2
            var numTargets = spellData.GetEffect(844015).BaseValue;

            return numTargets;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return double.MaxValue;
        }
    }
}
