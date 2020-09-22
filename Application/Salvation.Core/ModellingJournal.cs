using Salvation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salvation.Core
{
    class ModellingJournal : IModellingJournal
    {
        private List<string> journalEntries { get; set; }

        public ModellingJournal()
        {
            journalEntries = new List<string>();
        }
        public void Entry(string message)
        {
            journalEntries.Add(message);
        }

        public List<string> GetJournal(bool removeDuplicates = false)
        {
            if(removeDuplicates)
            {
                return journalEntries.Distinct().ToList();
            }

            return journalEntries;
        }
    }
}
