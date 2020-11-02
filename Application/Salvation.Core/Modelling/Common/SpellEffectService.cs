using Salvation.Core.Interfaces.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Modelling.Common
{
    /// <summary>
    /// Class for implementing spell effects
    /// </summary>
    public class SpellEffectService : SpellService
    {
        public SpellEffectService(IGameStateService gameStateService) 
            : base(gameStateService)
        {

        }
    }
}
