using System.Linq;
using System.Collections.Generic;

using CSPSolver.common;

namespace CSPSolver.Constraint.Equal
{
    public readonly struct EqualIntDomainConst : IConstraint
    {
        private readonly IIntVar _var;
        private readonly int _con;

        public EqualIntDomainConst(IIntVar var, int con) => (_var, _con) = (var, con);

        public IEnumerable<IVariable> Variables => new List<IVariable>() { _var };

        public IEnumerable<IVariable> Propagate(IState state) => _var.SetValue(state, _con) ? new IVariable[] { _var } : Enumerable.Empty<IVariable>();
    }
}
