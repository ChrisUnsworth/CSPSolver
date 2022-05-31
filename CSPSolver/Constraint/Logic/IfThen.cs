using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Logic
{
    public readonly struct IfThen : IConstraint
    {
        private readonly IConstraint _con1;
        private readonly IConstraint _con2;

        public IfThen(IConstraint con1, IConstraint con2)
        {
            _con1 = con1;
            _con2 = con2;
        }

        public IEnumerable<IVariable> Variables => _con1.Variables.Concat(_con2.Variables);

        public bool CanBeMet(IState state) => (!_con1.IsMet(state)) || _con2.CanBeMet(state);

        public bool IsMet(IState state) => (!_con1.CanBeMet(state)) || (_con1.IsMet(state) && _con2.IsMet(state));

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            if (_con1.IsMet(state)) return _con2.NegativePropagate(state);
            return Enumerable.Empty<IVariable>();
        }

        public IEnumerable<IVariable> Propagate(IState state)
        {
            if (_con1.IsMet(state)) return _con2.Propagate(state);
            return Enumerable.Empty<IVariable>();
        }
    }
}
