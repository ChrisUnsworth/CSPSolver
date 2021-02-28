using System;
using System.Linq;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Equal
{
    public readonly struct EqualIntDomainConst : IConstraint
    {
        private readonly ISmallIntVar _var;
        private readonly int _con;

        public EqualIntDomainConst(ISmallIntVar var, int con) => (_var, _con) = (var, con);

        public IEnumerable<IVariable> Variables => new List<IVariable>() { _var };

        public IEnumerable<IVariable> Propagate(IState state)
        {

            var (d, min, _) = _var.GetDomain(state);
            var newD = d & (int)Math.Pow(2, _con - min);

            if (d != newD)
            {
                _var.SetDomain(state, newD);
                return Enumerable.Repeat<IVariable>(_var, 1);
            }

            return Enumerable.Empty<IVariable>();
        }
    }
}
