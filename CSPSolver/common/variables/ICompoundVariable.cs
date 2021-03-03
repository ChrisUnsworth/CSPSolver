using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common.variables
{
    public interface ICompoundVariable
    {
        IEnumerable<IVariable> GetChildren();
    }
}
