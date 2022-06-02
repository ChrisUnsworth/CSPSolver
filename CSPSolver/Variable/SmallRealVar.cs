using System;
using System.Diagnostics.CodeAnalysis;
using CSPSolver.common;
using CSPSolver.common.variables;

using static System.Math;

namespace CSPSolver.Variable
{
    public readonly struct SmallRealVar : IRealVar, IDecisionVariable
    {
        public double Min { get; }

        public double Max { get; }

        public double Epsilon { get; }

        public IStateRef MinStateRef { get; }
        public IStateRef MaxStateRef { get; }

        public SmallRealVar(double min, IStateRef minStateRef, double max, IStateRef maxStateRef, int decimalPlaces)
        {
            Min = min;
            MinStateRef = minStateRef;
            Max = max;
            MaxStateRef = maxStateRef;
            Epsilon = Pow(10, -decimalPlaces); ;
        }

        public double GetDomainMax(in IState state) => Round(state.GetInt(MaxStateRef) * Epsilon, (int)Abs(Log10(Epsilon)));

        public double GetDomainMin(in IState state) => Round(state.GetInt(MinStateRef) * Epsilon, (int)Abs(Log10(Epsilon)));

        public void Initialise(IState state)
        {
            state.SetInt(MaxStateRef, (int)Round(Max / Epsilon));
            state.SetInt(MinStateRef, (int)Round(Min / Epsilon));
        }

        public bool IsEmpty(in IState state) => state.GetInt(MaxStateRef) < state.GetInt(MinStateRef);

        public bool IsInstantiated(in IState state) => state.GetInt(MaxStateRef) == state.GetInt(MinStateRef);

        public string PrettyDomain(in IState state) => $"{GetDomainMin(state)} ... {GetDomainMax(state)}";

        public bool RemoveValue(IState state, object value)
        {
            int val = AsInt((double)value);
            var result = false;
            var max = state.GetInt(MaxStateRef);
            var min = state.GetInt(MinStateRef);

            if (val == max)
            {
                state.SetInt(MaxStateRef, max - 1);
                result = true;
            }

            if (val == min)
            {
                state.SetInt(MinStateRef, min + 1);
                result = true;
            }

            return result;
        }

        public bool SetMax(IState state, double max)
        {
            var maxInt = AsInt(max);

            if (maxInt < state.GetInt(MaxStateRef))
            {
                state.SetInt(MaxStateRef, maxInt);
                return true;
            }

            return false;
        }

        public bool SetMin(IState state, double min)
        {
            var minInt = AsInt(min);

            if (minInt > state.GetInt(MinStateRef))
            {
                state.SetInt(MinStateRef, minInt);
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

        private int AsInt(double val) => (int)Round((double)val / Epsilon);

        public int DomainSize(in IState state) =>
            (int)((GetDomainMax(state) - GetDomainMin(state)) / Epsilon);

        public int DomainSize() => (int)((Max - Min) / Epsilon);

        public override int GetHashCode() => MinStateRef.UniqueIdentifier;

        public override bool Equals([NotNullWhen(true)] object obj) => obj is SmallRealVar smallOther && Equals(smallOther);

        public bool Equals(IDecisionVariable other) => other is SmallRealVar smallOther && Equals(smallOther);

        private bool Equals(SmallRealVar other) => MinStateRef.UniqueIdentifier == other.MinStateRef.UniqueIdentifier;

        public static bool operator ==(SmallRealVar left, SmallRealVar right) => left.Equals(right);

        public static bool operator !=(SmallRealVar left, SmallRealVar right) => !(left == right);
    }
}
