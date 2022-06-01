using System;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.Search.VariableOrdering;

namespace CSPSolver.Model
{
    public static class VariableOrdering
    {
        public static Func<IModel, IVariableOrderingHeuristic> Default => _ => new Arbitrary();
    }
}
