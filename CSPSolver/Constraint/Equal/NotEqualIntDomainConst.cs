using System.Linq;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Equal
{
    public readonly struct NotEqualIntDomainConst : IConstraint
    {
        private readonly IIntVar _var;
        private readonly int _con;

        public NotEqualIntDomainConst(IIntVar var, int con) => 
            (_var, _con) = (var, con);

        public IEnumerable<IVariable> Variables => 
            new List<IVariable>() { _var };

        public bool CanBeMet(IState state) =>
            !(_var.TryGetValue(state, out int val) && _con == val);

        public bool IsMet(IState state) =>
            _var.GetDomainMax(state) < _con || _var.GetDomainMin(state) > _con;

        public IEnumerable<IVariable> NegativePropagate(IState state) =>
            _var.SetValue(state, _con) ? new IVariable[] { _var } : Enumerable.Empty<IVariable>();

        public IEnumerable<IVariable> Propagate(IState state) => 
            _var.RemoveValue(state, _con) ? new IVariable[] { _var } : Enumerable.Empty<IVariable>();
    }
}
