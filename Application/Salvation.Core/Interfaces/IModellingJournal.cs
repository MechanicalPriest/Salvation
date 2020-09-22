using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces
{
    public interface IModellingJournal
    {
        public void Entry(string message);
        public List<string> GetJournal(bool removeDuplicates = false);
    }
}
