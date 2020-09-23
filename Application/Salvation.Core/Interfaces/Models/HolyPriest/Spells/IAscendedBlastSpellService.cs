using Salvation.Core.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.Common;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces.Models.HolyPriest.Spells
{
    public interface IAscendedBlastSpellService : ISpellService
    {
        AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData, decimal castableTimeframe, decimal boonActualCPM);
        decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData, decimal castableTimeframe, decimal boonActualCPM);
    }
}
