using Salvation.Core.State;
using System.Threading.Tasks;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface IComparisonModeller<T>
    {
        public Task<T> RunComparison(GameState baseState);
    }
}
