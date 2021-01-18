using CSPSolver.common;
using CSPSolver.Variable;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Constraint
{
    public class AllDiffSameIntDomain : IConstraint
    {
        private List<IntDomainVar> _variables;

        public AllDiffSameIntDomain(List<IntDomainVar> variables)
        {
            _variables = variables;
        }

        public IEnumerable<IVariable> Variables => (IEnumerable<IVariable>)_variables;

        public IEnumerable<IVariable> Propagate(IState state)
        {
            throw new NotImplementedException();
        }
    }
}
