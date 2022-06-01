using System;

using CSPSolver.common.search;
using CSPSolver.Search.strategy;

namespace CSPSolver.Model
{
    public static class BranchStrategy
    {
        public static Func<IVariableOrderingHeuristic, IValueOrderingHeuristic, IBranchStrategy> Default =>
            (var, val) => new BinaryBranching(var, val);
    }
}
