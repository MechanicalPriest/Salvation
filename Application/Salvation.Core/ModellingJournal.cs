using Salvation.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Core
{
    public class ModellingJournal : IModellingJournal
    {
        private List<string> JournalEntries { get; set; }

        public ModellingJournal()
        {
            JournalEntries = new List<string>();
        }
        public void Entry(string message)
        {
            JournalEntries.Add(message);
        }

        public List<string> GetJournal(bool removeDuplicates = false)
        {
            if (removeDuplicates)
            {
                return JournalEntries.Distinct().ToList();
            }

            return JournalEntries;
        }
    }
}
