using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Multiply
{
    public readonly struct PositiveMultiplyIntVar : IIntVar, ICompoundVariable
    {
        private readonly IIntVar _v1;
        private readonly IIntVar _v2;

        public int Min { get; }

        public int Size { get; }

        public int Max { get; }

        public PositiveMultiplyIntVar(IIntVar v1, IIntVar v2)
        {
            if (v1.Min < 0 || v2.Min < 0) throw new ArgumentOutOfRangeException($"{nameof(NegativeMultiplyIntVar)} only acts over posative domains.");
            _v1 = v1;
            _v2 = v2;
            Min = v1.Min * v2.Min;
            Max = v1.Max * v2.Max;
            Size = Max - Min + 1;
        }

        public int GetDomainMax(IState state) => _v1.GetDomainMax(state) * _v2.GetDomainMax(state);

        public int GetDomainMin(IState state) => _v1.GetDomainMin(state) * _v2.GetDomainMin(state);

        public void initialise(IState state) { /* holds no state */ }

        public bool isEmpty(IState state) => _v1.isEmpty(state) | _v2.isEmpty(state);

        public bool isInstantiated(IState state) => _v1.isInstantiated(state) & _v2.isInstantiated(state);

        public bool RemoveValue(IState state, object value) =>
            _v2.TryGetValue(state, out int v2) & _v1.RemoveValue(state, (int)value / v2)
          | _v1.TryGetValue(state, out int v1) & _v2.RemoveValue(state, (int)value / v1);

        public bool SetMax(IState state, int max) =>
            _v1.SetMax(state, max / _v2.GetDomainMin(state))
          | _v2.SetMax(state, max / _v1.GetDomainMin(state));

        public bool SetMin(IState state, int min) =>
            min > 0
         && (_v1.SetMin(state, (int)Math.Ceiling((double)min / _v2.GetDomainMax(state)))
           | _v2.SetMin(state, (int)Math.Ceiling((double)min / _v1.GetDomainMax(state))));

        public bool SetValue(IState state, object value) => SetMax(state, (int)value) | SetMin(state, (int)value);

        public bool TryGetValue(IState state, out int value)
        {
            if (_v1.TryGetValue(state, out int v1) & _v2.TryGetValue(state, out int v2))
            {
                value = v1 * v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(int);

        public string PrettyDomain(IState state) => $"{_v1.PrettyDomain(state)} * {_v2.PrettyDomain(state)}";

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
