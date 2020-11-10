using Salvation.Core.Profile.Model;
using System.Threading.Tasks;

namespace Salvation.Core.Interfaces.Profile
{
    public interface ISimcProfileService
    {
        Task<PlayerProfile> ApplySimcProfileAsync(string simcAddonString, PlayerProfile profile);
    }
}
