using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Modelling.Common
{
    /// <summary>
    /// Class for implementing spell effects
    /// </summary>
    public class SpellEffectService : SpellService, ISpellEffectService
    {
        /// <summary>
        /// Multiple this against coefficient to get scaled item spell values (consumables)
        /// Comes from sc_scale_data.inc's __spell_scaling array (item section) for level 60.
        /// </summary>
        protected readonly double ItemCoefficientMultiplier = 95;
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
    }
}
