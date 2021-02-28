using CSPSolver.common;
using CSPSolver.common.variables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Constraint
{
    public class AllDiffSameIntDomain : IConstraint
    {
        private List<ISmallIntVar> _variables;

        public AllDiffSameIntDomain(List<ISmallIntVar> variables)
        {
            _variables = variables;
        }

        public IEnumerable<IVariable> Variables => _variables;

        public IEnumerable<IVariable> Propagate(IState state)
        {
            throw new NotImplementedException();
        }
    }
}
