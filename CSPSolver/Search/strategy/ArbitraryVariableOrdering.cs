using CSPSolver.common;
using CSPSolver.common.search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPSolver.Search.strategy
{
    public readonly struct ArbitraryVariableOrdering : IVariableOrderingHeuristic
    {
        public IVariable Next(in IModel model, in IState state)
        {
            foreach (var v in model.Variables)
            {
                if (!v.isInstantiated(state)) return v;
            }

            return null;
        }
    }
}
