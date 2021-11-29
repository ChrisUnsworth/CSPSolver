using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Bool
{
    public readonly struct BoolConstraint : IConstraint
    {
        private readonly IBoolVar _var;

        public IEnumerable<IVariable> Variables => new[] { _var };

        public BoolConstraint(IBoolVar var) => _var = var;

        public bool CanBeMet(IState state) => _var.CanBeTrue(state);

        public bool IsMet(IState state) => _var.IsTrue(state);

        public IEnumerable<IVariable> Propagate(IState state)
        {
            if (_var.SetValue(state, true)) yield return _var;
        }

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            if (_var.SetValue(state, false)) yield return _var;
        }
    }
}
