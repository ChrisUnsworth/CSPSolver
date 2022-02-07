using System;
using System.Linq;
using System.Collections.Generic;

using static System.Math;

using CSPSolver.common.variables;
using CSPSolver.common;

namespace CSPSolver.Constraint.Equal
{
    public readonly struct NotEqualRealVar : IConstraint
    {
        private readonly IRealVar _var1;
        private readonly IRealVar _var2;

        private double Epsilon => Min(_var1.Epsilon, _var2.Epsilon);

        public NotEqualRealVar(IRealVar var1, IRealVar var2) => (_var1, _var2) = (var1, var2);

        public IEnumerable<IVariable> Variables => new IVariable[] { _var1, _var2 };

        private bool IsEqual(double v1, double v2) => Abs(v1 - v2) < Epsilon;

        private static bool AreTrue(double v1, double v2, params Func<double, double, bool>[] func) => func.All(f => f(v1, v2));

        public bool CanBeMet(IState state) =>
            !(_var1.TryGetValue(state, out double val1) &&
              _var2.TryGetValue(state, out double val2) &&
              IsEqual(val1, val2));

        public bool IsMet(IState state) =>
            (_var1.GetDomainMax(state) + Epsilon) < _var2.GetDomainMin(state) ||
            _var1.GetDomainMin(state) > (_var2.GetDomainMax(state) + Epsilon);

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            if (_var1.TryGetValue(state, out double val1)) return _var2.SetValue(state, val1) ? new IVariable[] { _var2 } : Enumerable.Empty<IVariable>();
            if (_var2.TryGetValue(state, out double val2)) return _var1.SetValue(state, val2) ? new IVariable[] { _var1 } : Enumerable.Empty<IVariable>();

            var changed = new List<IVariable>();

            if (_var1.SetMax(state, _var2.GetDomainMax(state)) | _var1.SetMin(state, _var2.GetDomainMin(state))) changed.Add(_var1);
            if (_var2.SetMax(state, _var1.GetDomainMax(state)) | _var2.SetMin(state, _var1.GetDomainMin(state))) changed.Add(_var2);

            return changed;
        }

        public IEnumerable<IVariable> Propagate(IState state)
        {
            if (_var1.TryGetValue(state, out double val1)) return _var2.RemoveValue(state, val1) ? new IVariable[] { _var2 } : Enumerable.Empty<IVariable>();
            if (_var2.TryGetValue(state, out double val2)) return _var1.RemoveValue(state, val2) ? new IVariable[] { _var1 } : Enumerable.Empty<IVariable>();

            return Enumerable.Empty<IVariable>();
        }
    }
}
