using System;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.Search.VariableOrdering;

namespace CSPSolver.Model
{
    public static class VariableOrdering
    {
        public static IVariableOrderingHeuristic Default(IModel _) => new Arbitrary();

        public static IVariableOrderingHeuristic LargestDomain(IModel _) => new LargestDomain();

        public static IVariableOrderingHeuristic SmallestDomain(IModel _) => new SmallestDomain();
    }
}
