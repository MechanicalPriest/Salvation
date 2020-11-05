using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Profile.Model;
using System.Collections.Generic;

namespace Salvation.Core.State
{
    public class GameState
    {
        [JsonProperty] // Workaround to let deserialising still set this property
        public PlayerProfile Profile { get; private set; }
        [JsonProperty] // Workaround to let deserialising still set this property
        public GlobalConstants Constants { get; private set; }
        /// <summary>
        /// A list of all the spells currently active for the modelling run. 
        /// Populated by calling GameStateService.RegisterSpells.
        /// </summary>
        internal List<RegisteredSpell> RegisteredSpells { get; set; }
        internal List<string> JournalEntries { get; set; }

        /// <summary>
        /// Initialise this using GameStateService.CreateValidatedGameState to validate the profile
        /// </summary>
        public GameState()
        {
            JournalEntries = new List<string>();
            RegisteredSpells = new List<RegisteredSpell>();
        }

        /// <summary>
        /// Initialise this using GameStateService.CreateValidatedGameState to validate the profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="constants"></param>
        public GameState(PlayerProfile profile, GlobalConstants constants)
            : this()
        {
            Profile = profile;
            Constants = constants;
        }
    }
}
