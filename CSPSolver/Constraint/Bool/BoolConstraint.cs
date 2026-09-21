using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Bool;

public readonly struct BoolConstraint(IBoolVar var) : IConstraint
{
    public IEnumerable<IVariable> Variables => [var];

    public bool CanBeMet(IState state) => var.CanBeTrue(state);

    public bool IsMet(IState state) => var.IsTrue(state);

    public IEnumerable<IVariable> Propagate(IState state) =>
        var.SetValue(state, true) ? [var] : [];

    public IEnumerable<IVariable> NegativePropagate(IState state) =>
        var.SetValue(state, false) ? [var] : [];
}