using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.common.variables;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Search.strategy
{
    public readonly struct MinValueOrdering : IValueOrderingHeuristic
    {
        public IEnumerable<object> Order(in IModel model, in IVariable variable) => variable switch
            {
                ISmallIntVar siv => siv.EnumerateDomain(model.State).Cast<object>(),
                _ => throw new NotSupportedException($"Variable type {variable.GetType().Name} not currently supported in MinValueOrdering."),
            };
    }
}
