using System;

using CSPSolver.common;
using CSPSolver.common.variables;

using static System.Math;

namespace CSPSolver.Variable
{
    public readonly struct LongRealVar : IRealVar
    {
        public double Min { get; }

        public double Max { get; }

        public double Epsilon { get; }

        public IStateRef MinStateRef { get; }
        public IStateRef MaxStateRef { get; }

        public LongRealVar(double min, IStateRef minStateRef, double max, IStateRef maxStateRef, int decimalPlaces)
        {
            Min = min;
            MinStateRef = minStateRef;
            Max = max;
            MaxStateRef = maxStateRef;
            Epsilon = Pow(10, -decimalPlaces); ;
        }

        public double GetDomainMax(IState state) => Round(state.GetLong(MaxStateRef) * Epsilon, (int)Abs(Log10(Epsilon)));

        public double GetDomainMin(IState state) => Round(state.GetLong(MinStateRef) * Epsilon, (int)Abs(Log10(Epsilon)));

        public void Initialise(IState state)
        {
            state.SetLong(MaxStateRef, (int)Round(Max / Epsilon));
            state.SetLong(MinStateRef, (int)Round(Min / Epsilon));
        }

        public bool IsEmpty(IState state) => state.GetLong(MaxStateRef) < state.GetLong(MinStateRef);

        public bool IsInstantiated(IState state) => state.GetLong(MaxStateRef) == state.GetLong(MinStateRef);

        public string PrettyDomain(IState state) => $"{GetDomainMin(state)} ... {GetDomainMax(state)}";

        public bool RemoveValue(IState state, object value)
        {
            long val = AsLong((double)value);
            var result = false;
            var max = state.GetLong(MaxStateRef);
            var min = state.GetLong(MinStateRef);

            if (val == max)
            {
                state.SetLong(MaxStateRef, max - 1);
                result = true;
            }

            if (val == min)
            {
                state.SetLong(MinStateRef, min + 1);
                result = true;
            }

            return result;
        }

        public bool SetMax(IState state, double max)
        {
            var maxLong = AsLong(max);

            if (maxLong < state.GetLong(MaxStateRef))
            {
                state.SetLong(MaxStateRef, maxLong);
                return true;
            }

            return false;
        }

        public bool SetMin(IState state, double min)
        {
            var minLong = AsLong(min);

            if (minLong > state.GetLong(MinStateRef))
            {
                state.SetLong(MinStateRef, minLong);
                return true;
            }

            return false;
        }

        public bool SetValue(IState state, object value) =>
            SetMin(state, (double)value) | SetMax(state, (double)value);

        public bool TryGetValue(IState state, out double value)
        {
            value = GetDomainMax(state);
            return value == GetDomainMin(state);
        }

        public Type VariableType() => typeof(double);

        private long AsLong(double val) => (long)Round((double)val / Epsilon);
    }
}
