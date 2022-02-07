using System;
using System.Collections.Generic;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Plus
{
    public readonly struct PlusRealVar : IRealVar, ICompoundVariable
    {
        private readonly IRealVar _v1;
        private readonly IRealVar _v2;

        public double Min { get; }

        public double Max { get; }

        public double Epsilon => Min(_v1.Epsilon, _v2.Epsilon);

        public PlusRealVar(IRealVar v1, IRealVar v2)
        {
            _v1 = v1;
            _v2 = v2;
            Min = v1.Min + v2.Min;
            Max = v1.Max + v2.Max;
        }

        public double GetDomainMax(IState state) => _v1.GetDomainMax(state) + _v2.GetDomainMax(state);

        public double GetDomainMin(IState state) => _v1.GetDomainMin(state) + _v2.GetDomainMin(state);

        public void Initialise(IState state) { /* holds no state */ }

        public bool IsEmpty(IState state) => _v1.IsEmpty(state) || _v2.IsEmpty(state);

        public bool IsInstantiated(IState state) => _v1.IsInstantiated(state) && _v2.IsInstantiated(state);

        public bool RemoveValue(IState state, object value)
        {
            var result = false;
            if (_v2.TryGetValue(state, out double v2))
            {
                result = _v1.RemoveValue(state, (double)value - v2);
            }

            if (_v1.TryGetValue(state, out double v1))
            {
                result |= _v2.RemoveValue(state, (double)value - v1);
            }

            return result;
        }

        public bool SetMax(IState state, double max) =>
            _v1.SetMax(state, max - _v2.GetDomainMin(state))
          | _v2.SetMax(state, max - _v1.GetDomainMin(state));

        public bool SetMin(IState state, double min) =>
            _v1.SetMin(state, min - _v2.GetDomainMax(state))
          | _v2.SetMin(state, min - _v1.GetDomainMax(state));

        public bool SetValue(IState state, object value) => SetMax(state, (double)value) | SetMin(state, (double)value);

        public bool TryGetValue(IState state, out double value)
        {
            if (_v1.TryGetValue(state, out double v1) && _v2.TryGetValue(state, out double v2))
            {                
                value = v1 + v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(double);

        public string PrettyDomain(IState state) => throw new NotImplementedException();

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
