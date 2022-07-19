using System;
using System.Linq;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.common.variables;

namespace CSPSolver.Search.ValueOrdering
{
    public readonly struct MinValueOrdering : IValueOrderingHeuristic
    {

        public IEnumerable<(Action<IState> choice, Action<IState> inverse)> Order(in IModel model, in IState state, in IDecisionVariable variable) => variable switch
        {
            IDecisionVariable<int> iv => Order(state, iv),
            IDecisionVariable<double> rv => Order(state, rv),
            _ => throw new NotSupportedException($"Variable type {variable.GetType().Name} not currently supported in MinValueOrdering."),
        };

        public static IEnumerable<(Action<IState> choice, Action<IState> inverse)> Order(IState state, IDecisionVariable<int> variable) =>
            variable.Domain(state)
                    .OrderBy(_ => _)
                    .Select(i => ((Action<IState> choice, Action<IState> inverse))(s => variable.SetMax(s, i), s => variable.SetMin(s, i + 1)));

        public static IEnumerable<(Action<IState> choice, Action<IState> inverse)> Order(IState state, IDecisionVariable<double> variable) =>
            variable.Domain(state)
                    .OrderBy(_ => _)
                    .Select(d => ((Action<IState> choice, Action<IState> inverse))(s => variable.SetMax(s, d), s => variable.SetMin(s, d + ((IRealVar)variable).Epsilon)));
    }
}
