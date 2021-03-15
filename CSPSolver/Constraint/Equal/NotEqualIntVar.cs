using CSPSolver.common;
using CSPSolver.Variable;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Constraint.Equal
{
    public class NotEqualIntVar : IConstraint
    {
        private readonly IIntVar _var1;
        private readonly IIntVar _var2;

        public NotEqualIntVar(IIntVar var1, IIntVar var2) => (_var1, _var2) = (var1, var2);

        public IEnumerable<IVariable> Variables => new IVariable[] { _var1, _var2 };

        public IEnumerable<IVariable> Propagate(IState state)
        {
            if (_var1.TryGetValue(state, out int val1)) return _var2.RemoveValue(state, val1) ? new IVariable[] { _var2 } : Enumerable.Empty<IVariable>();
            if (_var2.TryGetValue(state, out int val2)) return _var1.RemoveValue(state, val2) ? new IVariable[] { _var1 } : Enumerable.Empty<IVariable>();

            return Enumerable.Empty<IVariable>();
        }
    }
}
