﻿using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Inequality
{
    public readonly struct GreaterThanIntVar : IConstraint
    {
        private readonly IIntVar _var1;
        private readonly IIntVar _var2;

        public GreaterThanIntVar(IIntVar var1, IIntVar var2) => (_var1, _var2) = (var1, var2);

        public IEnumerable<IVariable> Variables => new IVariable[] { _var1, _var2 };

        public bool CanBeMet(IState state) =>
            _var1.GetDomainMax(state) > _var2.GetDomainMin(state);

        public bool IsMet(IState state) =>
            _var1.GetDomainMin(state) > _var2.GetDomainMax(state);

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            if (_var1.TryGetValue(state, out int val1)) return _var2.SetMin(state, val1) ? new IVariable[] { _var2 } : Enumerable.Empty<IVariable>();
            if (_var2.TryGetValue(state, out int val2)) return _var1.SetMax(state, val2) ? new IVariable[] { _var1 } : Enumerable.Empty<IVariable>();

            var changed = new List<IVariable>();

            if (_var1.SetMax(state, _var2.GetDomainMax(state))) changed.Add(_var1);
            if (_var2.SetMin(state, _var1.GetDomainMin(state))) changed.Add(_var2);

            return changed;
        }

        public IEnumerable<IVariable> Propagate(IState state)
        {
            if (_var1.TryGetValue(state, out int val1)) return _var2.SetMax(state, val1 - 1) ? new IVariable[] { _var2 } : Enumerable.Empty<IVariable>();
            if (_var2.TryGetValue(state, out int val2)) return _var1.SetMin(state, val2 + 1) ? new IVariable[] { _var1 } : Enumerable.Empty<IVariable>();

            var changed = new List<IVariable>();

            if (_var1.SetMin(state, _var2.GetDomainMin(state) + 1)) changed.Add(_var1);
            if (_var2.SetMax(state, _var1.GetDomainMax(state) - 1)) changed.Add(_var2);

            return changed;
        }
    }
}
