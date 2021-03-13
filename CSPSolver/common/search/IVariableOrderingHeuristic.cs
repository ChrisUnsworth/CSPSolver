using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common.search
{
    public interface IVariableOrderingHeuristic
    {
        IVariable Next(IModel model);
    }
}
