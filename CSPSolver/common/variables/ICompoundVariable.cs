using System.Collections.Generic;

namespace CSPSolver.common.variables
{
    public interface ICompoundVariable
    {
        IEnumerable<IVariable> GetChildren();
    }
}
