using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces.Models.HolyPriest.Spells
{
    public interface IFlashHealSpellService : ISpellService
    {
        public decimal GetAverageHealing(GameState gameState, BaseSpellData spellData = null);
    }
}
