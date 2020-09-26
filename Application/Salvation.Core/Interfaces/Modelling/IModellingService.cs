using Salvation.Core.Modelling.Common;
using Salvation.Core.State;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface IModellingService
    {
        BaseModelResults GetResults(GameState state);
    }
}
