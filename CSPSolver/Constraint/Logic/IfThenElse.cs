using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Logic
{
    public readonly struct IfThenElse : IConstraint
    {
        private readonly IConstraint _conIf;
        private readonly IConstraint _conThen;
        private readonly IConstraint _conElse;

        public IfThenElse(IConstraint conIf, IConstraint conThen, IConstraint conElse)
        {
            _conIf = conIf;
            _conThen = conThen;
            _conElse = conElse;
        }

        public IEnumerable<IVariable> Variables => _conIf.Variables.Concat(_conThen.Variables).Concat(_conElse.Variables);

        public bool CanBeMet(IState state) =>
            (_conIf.CanBeMet(state) && _conThen.CanBeMet(state)) ||
            ((!_conIf.IsMet(state)) && _conElse.CanBeMet(state));

        public bool IsMet(IState state) =>
            (_conIf.IsMet(state) && _conThen.CanBeMet(state)) ||
            ((!_conIf.CanBeMet(state)) && _conElse.CanBeMet(state));

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            if (_conIf.IsMet(state)) return _conThen.NegativePropagate(state);
            if (!_conIf.CanBeMet(state)) return _conElse.NegativePropagate(state);
            return Enumerable.Empty<IVariable>();
        }

        public IEnumerable<IVariable> Propagate(IState state)
        {
            if (_conIf.IsMet(state)) return _conThen.Propagate(state);
            if (!_conIf.CanBeMet(state)) return _conElse.Propagate(state);
            return Enumerable.Empty<IVariable>();
        }
    }
}
