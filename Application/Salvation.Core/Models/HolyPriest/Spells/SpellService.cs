using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest.Spells
{
    public class SpellService : ISpellService
    {
        protected readonly IGameStateService gameStateService;
        protected readonly IModellingService modellingService;

        public SpellService(IGameStateService gameStateService, IModellingService modellingService)
        {
            this.gameStateService = gameStateService;
            this.modellingService = modellingService;
        }

        public virtual SpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            throw new NotImplementedException();
        }
    }
}
