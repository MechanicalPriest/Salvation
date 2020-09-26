using System.Collections.Generic;

namespace Salvation.Core.Interfaces
{
    public interface IModellingJournal
    {
        public void Entry(string message);
        public List<string> GetJournal(bool removeDuplicates = false);
    }
}
