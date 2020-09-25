using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces.Modelling
{
    public interface IComparisonModeller<T>
    {
        public T RunComparison();
    }
}
