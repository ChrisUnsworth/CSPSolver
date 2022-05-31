using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

using static System.Math;

namespace CSPSolver.Math.Minus
{
    public readonly struct MinusRealVar : IRealVar, ICompoundVariable
    {
        private readonly IRealVar _v1;
        private readonly IRealVar _v2;

        public double Min { get; }

        public double Max { get; }

        public double Epsilon => Min(_v1.Epsilon, _v2.Epsilon);

        public MinusRealVar(IRealVar v1, IRealVar v2)
        {
            _v1 = v1;
            _v2 = v2;
            Min = v1.Min - v2.Max;
            Max = v1.Max - v2.Min;
        }

        public double GetDomainMax(in IState state) => _v1.GetDomainMax(state) - _v2.GetDomainMin(state);

        public double GetDomainMin(in IState state) => _v1.GetDomainMin(state) - _v2.GetDomainMax(state);

        public void Initialise(IState state) { /* holds no state */ }

        public bool IsEmpty(in IState state) => _v1.IsEmpty(state) || _v2.IsEmpty(state);

        public bool IsInstantiated(in IState state) => _v1.IsInstantiated(state) && _v2.IsInstantiated(state);

        public bool RemoveValue(IState state, object value)
        {
            var result = false;
            var doubleVal = (double)value;
            if (_v2.TryGetValue(state, out double v2))
            {
                result = _v1.RemoveValue(state, v2 + doubleVal);
            }

            if (_v1.TryGetValue(state, out double v1))
            {
                result |= _v2.RemoveValue(state, v1 - doubleVal);
            }

            return result;
        }

        public bool SetMax(IState state, double max) =>
            _v1.SetMax(state, max + _v2.GetDomainMax(state))
          | _v2.SetMin(state, _v1.GetDomainMin(state) - max);

        public bool SetMin(IState state, double min) =>
            _v1.SetMin(state, min + _v2.GetDomainMin(state))
          | _v2.SetMax(state, _v1.GetDomainMax(state) - min);

        public bool SetValue(IState state, object value) => SetMax(state, (double)value) | SetMin(state, (double)value);

        public bool TryGetValue(in IState state, out double value)
        {
            if (_v1.TryGetValue(state, out double v1) & _v2.TryGetValue(state, out double v2))
            {
                value = v1 - v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(double);

        public string PrettyDomain(in IState state) => $"{_v1.PrettyDomain(state)} - {_v2.PrettyDomain(state)}";

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
