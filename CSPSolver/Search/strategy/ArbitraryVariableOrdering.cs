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
        public IVariable Next(IModel model) => model.Variables.FirstOrDefault(v => !v.isInstantiated(model.State));
    }
}
