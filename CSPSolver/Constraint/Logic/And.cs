using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;

namespace CSPSolver.Constraint.Logic;

public readonly struct And(IConstraint con1, IConstraint con2) : IConstraint
{
    public IEnumerable<IVariable> Variables => con1.Variables.Concat(con2.Variables);

    public bool CanBeMet(IState state) => con1.CanBeMet(state) && con2.CanBeMet(state);

    public bool IsMet(IState state) => con1.IsMet(state) && con2.IsMet(state);

    public IEnumerable<IVariable> NegativePropagate(IState state) =>
        con1.IsMet(state)
            ? con2.NegativePropagate(state)
            : con2.IsMet(state)
                ? con1.NegativePropagate(state)
                : [];

    public IEnumerable<IVariable> Propagate(IState state) => con1.Propagate(state).Concat(con2.Propagate(state));
}