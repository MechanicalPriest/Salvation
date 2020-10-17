using Salvation.Core.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Utility.SpellDataUpdate
{
    public interface ISpellDataService<T> where T : SpellDataService
    {
        Task<BaseSpec> Generate();
    }
}
