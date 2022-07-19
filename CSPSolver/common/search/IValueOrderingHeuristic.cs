using CSPSolver.common.variables;
using System;
using System.Collections.Generic;

namespace CSPSolver.common.search
{
    public interface IValueOrderingHeuristic
    {
        IEnumerable<(Action<IState> choice, Action<IState> inverse)> Order(in IModel model, in IState state, in IDecisionVariable variable);
    }
}
