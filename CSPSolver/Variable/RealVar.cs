using System;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Variable
{
    public readonly struct RealVar : IRealVar
    {
        public double Min { get; }

        public double Max { get; }

        public double Epsilon { get; }

        public IStateRef MinStateRef { get; }
        public IStateRef MaxStateRef { get; }

        public RealVar(double min, IStateRef minStateRef, double max, IStateRef maxStateRef, double epsilon = 0)
        {
            Min = min;
            MinStateRef = minStateRef;
            Max = max;
            MaxStateRef = maxStateRef;
            Epsilon = epsilon == 0
                ? (max - min) / 100
                : epsilon;
        }

        public double GetDomainMax(IState state) => state.GetDouble(MaxStateRef);

        public double GetDomainMin(IState state) => state.GetDouble(MinStateRef);

        public void Initialise(IState state)
        {
            state.SetDouble(MaxStateRef, Max);
            state.SetDouble(MinStateRef, Min);
        }

        public bool IsEmpty(IState state) => GetDomainMax(state) < GetDomainMin(state);

        public bool IsInstantiated(IState state) => GetDomainMax(state) - GetDomainMin(state) <= Epsilon;

        public string PrettyDomain(IState state) => $"{GetDomainMin(state)} ... {GetDomainMax(state)}";

        public bool RemoveValue(IState state, object value)
        {
            var val = (double)value;
            var result = false;
            var max = GetDomainMax(state);
            var min = GetDomainMin(state);

            if (val <= max && val > max - Epsilon)
            {
                state.SetDouble(MaxStateRef, max - Epsilon);
                result = true;
            }

            if (val >= min && val < min + Epsilon)
            {
                state.SetDouble(MinStateRef, min + Epsilon);
                result = true;
            }

            return result;
            
        }

        public bool SetMax(IState state, double max)
        {
            if (max < GetDomainMax(state))
            {
                state.SetDouble(MaxStateRef, max);
                return true;
            }

            return false;
        }

        public bool SetMin(IState state, double min)
        {
            if (min > GetDomainMin(state))
            {
                state.SetDouble(MinStateRef, min);
                return true;
            }

            return false;
        }

        public bool SetValue(IState state, object value) => SetValue(state, (double)value);

        public bool SetValue(IState state, double value)
        {
            var result = false;

            if ((value - Epsilon) >= GetDomainMin(state))
            {
                result = true;
                state.SetDouble(MinStateRef, value);
            }

            if ((value + Epsilon) <= GetDomainMax(state))
            {
                result = true;
                state.SetDouble(MaxStateRef, value);
            }

            return result;
        }

        public bool TryGetValue(IState state, out double value)
        {
            value = GetDomainMax(state);
            return (value - Epsilon) < GetDomainMin(state);
        }

        public Type VariableType() => typeof(double);
    }
}
