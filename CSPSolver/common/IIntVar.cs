using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IIntVar : IVariable<int>
    {
        int GetDomainMax(IState state);

        int GetDomainMin(IState state);
    }
}
