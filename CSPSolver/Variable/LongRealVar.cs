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
            state.SetLong(MaxStateRef, AsFloorLong(Max));
            state.SetLong(MinStateRef, AsCeilLong(Min));
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
            // Floor, not nearest: rounding a shrinking max up onto the grid could
            // keep a point that's actually above the real bound just narrowed to.
            var maxLong = AsFloorLong(max);

            if (maxLong < state.GetLong(MaxStateRef))
            {
                state.SetLong(MaxStateRef, maxLong);
                return true;
            }

            return false;
        }

        public bool SetMin(IState state, double min)
        {
            // Ceiling, not nearest: rounding a growing min down onto the grid could
            // keep a point that's actually below the real bound just narrowed to.
            var minLong = AsCeilLong(min);

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

        // See SmallRealVar's AsFloorInt/AsCeilInt: val / Epsilon lands a tick off an
        // exact grid point often enough that a bare floor/ceiling would knock a real
        // grid point off the domain, so nudge by a tolerance tiny next to one step.
        private long AsFloorLong(double val) => (long)Floor(val / Epsilon + 1e-9);

        private long AsCeilLong(double val) => (long)Ceiling(val / Epsilon - 1e-9);
    }
}
