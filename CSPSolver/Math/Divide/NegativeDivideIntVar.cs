using System;
using System.Collections.Generic;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Divide
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

        public void Initialise(IState state) { /* holds no state */ }

        public bool IsEmpty(IState state) => _v1.IsEmpty(state) || _v2.IsEmpty(state);

        public bool IsInstantiated(IState state) => _v1.IsInstantiated(state) && _v2.IsInstantiated(state);

        public bool RemoveValue(IState state, object value) =>
            (_v2.TryGetValue(state, out int v2) && _v1.RemoveValue(state, (int)value * v2))
          | (_v1.TryGetValue(state, out int v1) && (int)value != 0 && _v2.RemoveValue(state, v1 / (int)value));

        public bool SetMax(IState state, int max) =>
            max > 0
                && _v1.SetMin(state, (max + 1) * _v2.GetDomainMin(state) + 1)
                  | _v2.SetMax(state, _v1.GetDomainMax(state) / (max + 1) - 1);

        public bool SetMin(IState state, int min) =>
            min > 0
         && _v1.SetMax(state, _v2.GetDomainMax(state) * min)
           | _v2.SetMin(state, (int)Ceiling(_v1.GetDomainMin(state) / (double)min));

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

        public string PrettyDomain(IState state) => $"{_v1.PrettyDomain(state)} / {_v2.PrettyDomain(state)}";

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
