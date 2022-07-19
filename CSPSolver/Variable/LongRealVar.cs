using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CSPSolver.common;
using CSPSolver.common.variables;

using static System.Math;

namespace CSPSolver.Variable
{
    public readonly struct LongRealVar : IRealVar, IDecisionVariable<double>
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

        public double GetDomainMax(in IState state) => Round(state.GetLong(MaxStateRef) * Epsilon, (int)Abs(Log10(Epsilon)));

        public double GetDomainMin(in IState state) => Round(state.GetLong(MinStateRef) * Epsilon, (int)Abs(Log10(Epsilon)));

        public void Initialise(IState state)
        {
            state.SetLong(MaxStateRef, (int)Round(Max / Epsilon));
            state.SetLong(MinStateRef, (int)Round(Min / Epsilon));
        }

        public bool IsEmpty(in IState state) => state.GetLong(MaxStateRef) < state.GetLong(MinStateRef);

        public bool IsInstantiated(in IState state) => state.GetLong(MaxStateRef) == state.GetLong(MinStateRef);

        public string PrettyDomain(in IState state) => $"{GetDomainMin(state)} ... {GetDomainMax(state)}";

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

        public bool TryGetValue(in IState state, out double value)
        {
            value = GetDomainMax(state);
            return value == GetDomainMin(state);
        }

        public Type VariableType() => typeof(double);

        private long AsLong(double val) => (long)Round((double)val / Epsilon);

        public int DomainSize(in IState state) =>
            (int)((GetDomainMax(state) - GetDomainMin(state)) / Epsilon);

        public int DomainSize() => (int)((Max - Min) / Epsilon);

        public override int GetHashCode() => MinStateRef.UniqueIdentifier;

        public override bool Equals([NotNullWhen(true)] object obj) => obj is LongRealVar longOther && Equals(longOther);

        public bool Equals(IDecisionVariable other) => other is LongRealVar longOther && Equals(longOther);

        private bool Equals(LongRealVar other) => MinStateRef.UniqueIdentifier == other.MinStateRef.UniqueIdentifier;

        public IEnumerable<double> Domain(in IState _)
        {
            var min = GetDomainMin(_);
            var max = GetDomainMax(_);
            var e = Epsilon;

            return Enumerable.Range(0, (int)((max - min) / Epsilon)).Select(i => min + i * e);
        }

        public static bool operator ==(LongRealVar left, LongRealVar right) => left.Equals(right);

        public static bool operator !=(LongRealVar left, LongRealVar right) => !(left == right);
    }
}
