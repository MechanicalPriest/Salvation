using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Models;
using Salvation.Core.Models.Common;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces.Models.HolyPriest.Spells
{
    public interface ISpellService
    {/// <summary>
     /// TODO: Rename SpellCastResult into SpellResultModel
     /// </summary>
     /// <param name="model"></param>
     /// <param name="spell"></param>
     /// <returns></returns>
        SpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null);
    }

    /// <summary>
    /// Could use the following if needed to override a spells model / data. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public interface ISpellService<T> : ISpellService where T : BaseSpell
    //{
    //    //SpellCastResult GetCastResults(HolyPriestModel model, T spellOverride = null);
    //}
}
