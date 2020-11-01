using Salvation.Core.Profile;
using System.Threading.Tasks;

namespace Salvation.Core.Interfaces.Profile
{
    interface ISimcProfileService
    {
        Task<PlayerProfile> ApplySimcProfileAsync(string simcAddonString, PlayerProfile profile);
    }
}
