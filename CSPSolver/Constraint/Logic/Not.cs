using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Logic
{
    public readonly struct Not : IConstraint
    {
        private readonly IConstraint _con;

        public Not(IConstraint con) => _con = con;

        public IEnumerable<IVariable> Variables => _con.Variables;

        public bool CanBeMet(IState state) => !_con.IsMet(state);

        public bool IsMet(IState state) => !_con.CanBeMet(state);

        public IEnumerable<IVariable> NegativePropagate(IState state) => _con.Propagate(state);

        public IEnumerable<IVariable> Propagate(IState state) => _con.NegativePropagate(state);
    }
}
