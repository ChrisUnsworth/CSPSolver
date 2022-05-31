using CSPSolver.common.variables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common.search
{
    public interface IVariableOrderingHeuristic
    {
        IVariable Next(in IModel model, in IState state);
    }
}
