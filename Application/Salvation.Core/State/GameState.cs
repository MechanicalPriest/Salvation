using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Profile;
using System.Collections.Generic;

namespace Salvation.Core.State
{
    public class GameState
    {
        [JsonProperty] // Workaround to let deserialising still set this property
        public PlayerProfile Profile { get; private set; }
        [JsonProperty] // Workaround to let deserialising still set this property
        public GlobalConstants Constants { get; private set; }
        internal List<string> JournalEntries { get; set; }

        /// <summary>
        /// Initialise this using .CreateValidatedGameState to validate the profile
        /// </summary>
        public GameState()
        {
            JournalEntries = new List<string>();
        }

        /// <summary>
        /// Initialise this using .CreateValidatedGameState to validate the profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="constants"></param>
        public GameState(PlayerProfile profile, GlobalConstants constants)
            : this()
        {
            Profile = profile;
            Constants = constants;
        }

        public void SetProfile(PlayerProfile profile)
        {
            Profile = profile;
        }

        public void SetConstants(GlobalConstants constants)
        {
            Constants = constants;
        }
    }
}
