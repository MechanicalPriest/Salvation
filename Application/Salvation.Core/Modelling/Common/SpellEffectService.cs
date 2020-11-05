using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.Common
{
    /// <summary>
    /// Class for implementing spell effects
    /// </summary>
    public class SpellEffectService : SpellService, ISpellEffectService
    {
        // TODO: Move these variables somewhere that makes some more sense.
        /// <summary>
        /// Multiply this against coefficient to get scaled item spell values (flask buff)
        /// Comes from sc_scale_data.inc's __spell_scaling array (item section) for level 60.
        /// </summary>
        protected readonly double ItemCoefficientMultiplier = 95;
        /// <summary>
        /// Multiply this against coefficient to get scaled item spell values (potion buff)
        /// Comes from sc_scale_data.inc's __spell_scaling array (consumable section) for level 60.
        /// </summary>
        protected readonly double ConsumableCoefficientMultiplier = 25000;
        /// <summary>
        /// Badluck protection modifier for RPPM effects that generate buffs that could overlap
        /// </summary>
        protected readonly double RppmBadluckProtection = 1.13;

        public SpellEffectService(IGameStateService gameStateService)
            : base(gameStateService)
        {

        }

        /// <summary>
        /// Uptime as a percentage. 1.0 = 100%
        /// </summary>
        public virtual double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageIntellect(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageCriticalStrike(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageHaste(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageMastery(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageVersatility(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageMp5(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }
    }
}
