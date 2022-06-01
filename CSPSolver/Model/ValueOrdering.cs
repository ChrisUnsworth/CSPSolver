using System;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.Search.strategy;

namespace CSPSolver.Model
{
    public static class ValueOrdering
    {
        public static Func<IModel, IValueOrderingHeuristic> Default => _ => new MinValueOrdering();
    }
}
