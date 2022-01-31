using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Round
{
    public readonly struct RealRound : IRealVar, ICompoundVariable
    {
        private readonly IRealVar _realVar;
        private readonly int _dp;

        public double Epsilon => _realVar.Epsilon;

        public RealRound(IRealVar realVar, int dp) => (_realVar, _dp) = (realVar, dp);

        public double Min => System.Math.Round(_realVar.Min, _dp);

        public double Max => System.Math.Round(_realVar.Max, _dp);

        public IEnumerable<IVariable> GetChildren() { yield return _realVar; }

        public double GetDomainMax(IState state) => System.Math.Round(_realVar.GetDomainMax(state), _dp);

        public double GetDomainMin(IState state) => System.Math.Round(_realVar.GetDomainMin(state), _dp);

        public void Initialise(IState state) => _realVar.Initialise(state);

        public bool IsEmpty(IState state) => _realVar.IsEmpty(state);

        public bool IsInstantiated(IState state) => GetDomainMax(state) == GetDomainMin(state);

        public string PrettyDomain(IState state)
        {
            var min = GetDomainMin(state);
            var max = GetDomainMax(state);

            return min == max
                ? $"{{ {max} }}"
                : $"{{ {min} .. {max} }}";
        }

        public bool RemoveValue(IState state, object value) => _realVar.RemoveValue(state, value);

        public bool SetMax(IState state, double max) => _realVar.SetMax(state, max + (0.5 * System.Math.Pow(10, -_dp)) - Epsilon);

        public bool SetMin(IState state, double min) => _realVar.SetMin(state, min - (0.5 * System.Math.Pow(10, -_dp)) + Epsilon);

        public bool SetValue(IState state, object value) =>
            SetMax(state, (int)value) |
            SetMin(state, (int)value);

        public bool TryGetValue(IState state, out double value)
        {
            value = GetDomainMin(state);
            return value == GetDomainMax(state);
        }

        public Type VariableType() => typeof(double);
    }
}
