using Salvation.Core.Constants;
using System.Threading.Tasks;

namespace Salvation.Utility.SpellDataUpdate
{
    public interface ISpellDataService<T> where T : SpellDataService
    {
        Task<BaseSpec> Generate();
    }
}
