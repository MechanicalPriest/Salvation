using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.ViewModel
{
    public class ModellingResultsViewModel
    {
        public BaseModelResults ModelResults { get; set; }
        public List<string> JournalEntries { get; set; }
        public GameState GameState { get; set; }
        public StatWeightResult RawStatWeightResult { get; set; }
        public StatWeightResult EffectiveStatWeightResult { get; set; }
    }
}
