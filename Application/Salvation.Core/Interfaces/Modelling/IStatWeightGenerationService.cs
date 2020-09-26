using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using static Salvation.Core.Modelling.StatWeightGenerator;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface IStatWeightGenerationService
    {
        StatWeightResult Generate(GameState state, int numAdditionalStats,
            StatWeightType swType = StatWeightType.EffectiveHealing);
    }
}
