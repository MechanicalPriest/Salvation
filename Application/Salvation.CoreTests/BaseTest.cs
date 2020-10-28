using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System.IO;

namespace Salvation.CoreTests
{
    public class BaseTest
    {
        protected GameState GetGameState()
        {
            var basePath = "TestData";

            IConstantsService constantsService = new ConstantsService();
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine(basePath, "BaseTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine(basePath, "BaseTests_profile.json")));

            return new GameState(profile, constants);
        }
    }
}
