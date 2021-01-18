using CSPSolver.common;
using CSPSolver.Variable;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Constraint.Equal
{
    public readonly struct EqualIntDomainConst : IConstraint
    {
        private readonly IntDomainVar _var;
        private readonly int _con;

        public EqualIntDomainConst(IntDomainVar var, int con) => (_var, _con) = (var, con);

        public IEnumerable<IVariable> Variables => new List<IVariable>() { _var };

        public IEnumerable<IVariable> Propagate(IState state)
        {

            var d = _var.GetDomain(state);
            var newD = d & (int)Math.Pow(2, _con - _var.Min);

            if (d != newD)
            {
                _var.SetDomain(state, newD);
                return Enumerable.Repeat<IVariable>(_var, 1);
            }

            return Enumerable.Empty<IVariable>();
        }
    }
}
