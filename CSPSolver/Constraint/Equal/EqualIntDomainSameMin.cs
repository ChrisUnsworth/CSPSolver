using System;
using System.Collections.Generic;
using System.Text;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Equal
{
    public class EqualIntDomainSameMin : IConstraint
    {
        private ISmallIntDomainVar _var1;
        private ISmallIntDomainVar _var2;

        public EqualIntDomainSameMin(ISmallIntDomainVar var1, ISmallIntDomainVar var2) => (_var1, _var2) = (var1, var2);

        public IEnumerable<IVariable> Variables => new List<IVariable>() { _var1, _var2};

        public bool CanBeMet(IState state)
        {
            var (d1, _, _) = _var1.GetDomain(state);
            var (d2, _, _) = _var2.GetDomain(state);
            return  (d1 & d2) != 0;
        }

        public bool IsMet(IState state) =>
            _var1.TryGetValue(state, out int val1) &&
            _var2.TryGetValue(state, out int val2) &&
            val1 == val2;

        public IEnumerable<IVariable> Propagate(IState state)
        {
            var (d1, _, _) = _var1.GetDomain(state);
            var (d2, _, _) = _var2.GetDomain(state);
            if (_var1.SetDomain(state, d2)) yield return _var1;
            if (_var2.SetDomain(state, d1)) yield return _var2;
        }
    }
}
