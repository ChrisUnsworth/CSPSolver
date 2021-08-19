using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common.variables
{
    public interface IIntVar : IVariable<int>
    {
        int Min { get; }

        int Size { get; }

        int Max { get; }

        int GetDomainMax(IState state);

        int GetDomainMin(IState state);

        bool SetMax(IState state, int max);

        bool SetMin(IState state, int min);

    }
}
