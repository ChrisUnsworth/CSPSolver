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
        public IVariable Next(in IModel model, IState state) => model.Variables.FirstOrDefault(v => !v.isInstantiated(state));
    }
}
