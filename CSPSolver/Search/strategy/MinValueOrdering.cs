using System;
using System.Linq;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.common.variables;

namespace CSPSolver.Search.strategy
{
    public readonly struct MinValueOrdering : IValueOrderingHeuristic
    {
        public IEnumerable<Action<IState>> Order(in IModel model, in IState state, in IVariable variable) => variable switch
        {
            IIntVar iv => Order(state, iv),
            IRealVar rv => Order(state, rv),
            _ => throw new NotSupportedException($"Variable type {variable.GetType().Name} not currently supported in MinValueOrdering."),
        };

        public static IEnumerable<Action<IState>> Order(in IState state, IIntVar variable)
        {
            var min = variable.GetDomainMin(state);

            return new Action<IState>[]
            {
                s => variable.SetMax(s, min),
                s => variable.SetMin(s, min + 1) };
        }

        public static IEnumerable<Action<IState>> Order(in IState state, IRealVar variable)
        {
            var min = variable.GetDomainMin(state);

            return new Action<IState>[]
            {
                s => variable.SetMax(s, min),
                s => variable.SetMin(s, min + variable.Epsilon) };
        }
    }
}
