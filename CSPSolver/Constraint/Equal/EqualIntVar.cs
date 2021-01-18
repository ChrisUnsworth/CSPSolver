using CSPSolver.common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Constraint.Equal
{
    public class EqualIntVar : IConstraint
    {

        public EqualIntVar(IIntVar var1, IIntVar var2)
        {

        }

        public IEnumerable<IVariable> Variables => throw new NotImplementedException();

        public IEnumerable<IVariable> Propagate(IState state)
        {
            throw new NotImplementedException();
        }
    }
}
