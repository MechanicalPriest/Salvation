using Salvation.Core.Constants;

namespace Salvation.Core.Interfaces.Constants
{
    public interface IConstantsService
    {
        GlobalConstants LoadConstantsFromFile();
        GlobalConstants ParseConstants(string rawConstants);
        string DefaultDirectory { get; }
        string DefaultFilename { get; }
        void SetDefaultDirectory(string defaultFilePath);
        void SetDefaultFilename(string defaultFilename);
    }
}
