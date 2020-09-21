using Salvation.Core.Models;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces.Models
{
    public interface IModellingService
    {
        BaseModelResults GetResults(BaseProfile profile);
    }
}
