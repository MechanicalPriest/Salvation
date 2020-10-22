using Salvation.Core.Constants;
using Salvation.Core.Profile;
using System.Collections.Generic;

namespace Salvation.Core.State
{
    public class GameState
    {
        public PlayerProfile Profile { get; set; }
        public GlobalConstants Constants { get; set; }
        internal List<string> JournalEntries { get; set; }

        public GameState()
        {
            JournalEntries = new List<string>();
        }

        public GameState(PlayerProfile profile, GlobalConstants constants)
            : this()
        {
            Profile = profile;
            Constants = constants;
        }
    }
}
