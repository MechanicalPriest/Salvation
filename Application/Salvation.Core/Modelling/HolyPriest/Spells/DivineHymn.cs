using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class DivineHymn : SpellService, ISpellService<IDivineHymnSpellService>
    {
        public DivineHymn(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.DivineHymn;
            // TODO: Include the hymn healing amp bonus in the GetGlobalHealingMultiplier in some way.
            // May need to then remove it for the hymn specific healing calcs.
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            // Effect 59166 holds the aura bonus, 4% per stack.
            var divineHymnAura = spellData.GetEffect(59162).TriggerSpell.GetEffect(59166).BaseValue / 100;

            // DH's average heal for the first tick is:
            // SP% * Intellect * Vers * Hpriest Aura
            var healingSp = spellData.GetEffect(59162).TriggerSpell.GetEffect(59165).SpCoefficient;

            double firstTickRaid = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            // double it if we have 5 or less (dungeon group buff)
            double firstTickParty = firstTickRaid * 2;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {firstTickRaid:0.##} & {firstTickParty:0.##} (first tick)");
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {firstTickRaid * 5:0.##} & {firstTickParty * 5:0.##} (all ticks)");

            // Pick whether we're in part or raid
            double baseTick = GetNumberOfHealingTargets(gameState, spellData) <= 5 ? firstTickParty : firstTickRaid;

            // TODO: Include a configurable variable here to set the average number of ticks.
            double numTicks = spellData.GetEffect(59162).TriggerSpell.MaxStacks;

            baseTick *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            // This is the healing per tick, then add on the bonus healing per tick.
            double averageHeal = baseTick * numTicks + baseTick * divineHymnAura * ((numTicks - 1) * numTicks / 2);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCooldown = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            // DH is simply 60 / CD + 1 / (FightLength / 60)
            // Number of casts per minute plus one cast at the start of the encounter
            double maximumPotentialCasts = 60d / hastedCooldown
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
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
