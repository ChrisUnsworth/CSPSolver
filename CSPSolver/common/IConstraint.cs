using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IConstraint
    {
        IEnumerable<IVariable> Variables { get; }

        IEnumerable<IVariable> Propagate(IState state);
    }
}
