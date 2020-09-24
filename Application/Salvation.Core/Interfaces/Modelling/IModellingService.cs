using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface IModellingService
    {
        BaseModelResults GetResults(GameState state);
    }
}
