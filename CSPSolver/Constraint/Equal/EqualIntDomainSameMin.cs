using CSPSolver.common;
using CSPSolver.Variable;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Constraint.Equal
{
    public class EqualIntDomainSameMin : IConstraint
    {
        private IntDomainVar _var1;
        private IntDomainVar _var2;

        public EqualIntDomainSameMin(IntDomainVar var1, IntDomainVar var2) => (_var1, _var2) = (var1, var2);

        public IEnumerable<IVariable> Variables => new List<IVariable>() { _var1, _var2};

        public IEnumerable<IVariable> Propagate(IState state)
        {
            var d1 = _var1.GetDomain(state);
            var d2 = _var2.GetDomain(state);
            var newD = d1 & d2;
            var result = new List<IVariable>();

            if (d1 != newD)
            {
                result.Add(_var1);
                _var1.SetDomain(state, newD);
            }

            if (d2 != newD)
            {
                result.Add(_var2);
                _var2.SetDomain(state, newD);
            }

            return result;
        }
    }
}
