using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PowerWordShield : SpellService, IPowerWordShieldSpellService
    {
        public PowerWordShield(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.PowerWordShield;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            if (_gameStateService.IsConduitActive(gameState, Conduit.CharitableSoul))
            {
                var csSpellData = _gameStateService.GetSpellData(gameState, Spell.CharitableSoul);

                // Turn the rank value into a multiplier. "Rank" 10 = 0.10
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.CharitableSoul);
                var rankMulti = csSpellData.ConduitRanks[rank] / 100;

                AveragedSpellCastResult csComponent = new AveragedSpellCastResult
                {
                    SpellId = (int)Spell.CharitableSoul,
                    SpellName = csSpellData.Name,
                    RawHealing = result.RawHealing * rankMulti,
                    Healing = result.Healing * rankMulti,
                    Cooldown = 0,
                    Duration = 0,
                    Gcd = 0,
                    ManaCost = 0,
                    NumberOfHealingTargets = 1
                };
                csComponent.MakeSpellHaveNoCasts();

                result.AdditionalCasts.Add(csComponent);
            }

            return result;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // SP% * Intellect * Vers * Hpriest Aura
            // TODO: For some reason PW:S is done kinda weird. No basevalue of spcoeff.
            // It just seems to use $shield=${$SP*1.8*(1+$@versadmg)*
            // It also doesn't scaling with the healing aura bonus, see issue #71
            var absorbSp = 1.8;

            double averageHeal = absorbSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            double fillerCastTime = hastedCastTime == 0d
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
            return 1;
        }
    }
}
