using Salvation.Core.Constants;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.State
{
    public class GameState
    {
        public PlayerProfile Profile { get; set; }
        public GlobalConstants Constants { get; set; }

    }
}
