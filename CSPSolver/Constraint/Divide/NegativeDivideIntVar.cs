using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Divide
{
    public readonly struct NegativeDivideIntVar : IIntVar, ICompoundVariable
    {
        private readonly IIntVar _v1;
        private readonly IIntVar _v2;

        public int Min { get; }

        public int Size { get; }

        public int Max { get; }

        public NegativeDivideIntVar(IIntVar v1, IIntVar v2)
        {
            if (v1.Max >= 0 || v2.Max >= 0) throw new ArgumentOutOfRangeException($"{nameof(NegativeDivideIntVar)} only acts over negative domains.");
            _v1 = v1;
            _v2 = v2;
            Min = v1.Max / v2.Min;
            Max = v1.Min / v2.Max;
            Size = Max - Min + 1;
        }

        public int GetDomainMax(IState state) => _v1.GetDomainMin(state) / _v2.GetDomainMax(state);

        public int GetDomainMin(IState state) => _v1.GetDomainMax(state) / _v2.GetDomainMin(state);

        public void initialise(IState state) { /* holds no state */ }

        public bool isEmpty(IState state) => _v1.isEmpty(state) || _v2.isEmpty(state);

        public bool isInstantiated(IState state) => _v1.isInstantiated(state) && _v2.isInstantiated(state);

        public bool RemoveValue(IState state, object value) =>
            (_v2.TryGetValue(state, out int v2) && _v1.RemoveValue(state, (int)value * v2))
          | (_v1.TryGetValue(state, out int v1) && (int)value != 0 && _v2.RemoveValue(state, v1 / (int)value));

        public bool SetMax(IState state, int max) =>
            max > 0
                && (_v1.SetMin(state, ((max + 1) * _v2.GetDomainMin(state)) + 1)
                  | _v2.SetMax(state, (_v1.GetDomainMax(state) / (max + 1)) - 1));

        public bool SetMin(IState state, int min) =>
            min > 0
         && (_v1.SetMax(state, _v2.GetDomainMax(state) * min)
           | _v2.SetMin(state, (int)Math.Ceiling(_v1.GetDomainMin(state) / (double)min)));

        public bool SetValue(IState state, object value) => SetMax(state, (int)value) | SetMin(state, (int)value);

        public bool TryGetValue(IState state, out int value)
        {
            if (_v1.TryGetValue(state, out int v1) & _v2.TryGetValue(state, out int v2))
            {
                value = v1 / v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(int);

        public string PrettyDomain(IState state)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
