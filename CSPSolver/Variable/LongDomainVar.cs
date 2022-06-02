﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.utils;

namespace CSPSolver.Variable
{
    public readonly struct LongDomainVar: ILongDomainVar, IDecisionVariable
    {
        public IStateRef StateRef { get; }
        public int Min { get; }
        public int Size { get; }

        public int Max => Min + Size - 1;

        public LongDomainVar(int min, int size, IStateRef stateRef)
        {
            StateRef = stateRef;
            Min = min;
            Size = size;
        }

        public (ulong domain, int min, int size) GetDomain(in IState state) => (state.GetDomainLong(StateRef, Size), Min, Size);

        public bool SetDomain(IState state, ulong domain)
        {
            var oldDom = state.GetDomainLong(StateRef, Size);
            domain &= oldDom;
            if (domain != oldDom)
            {
                state.SetDomainLong(StateRef, Size, domain);
                return true;
            }

            return false;
        }

        public bool DomainMinus(IState state, ulong domain) => SetDomain(state, ~domain);

        public IEnumerable<int> EnumerateDomain(IState state)
        {
            var domain = state.GetDomainLong(StateRef, Size);
            ulong mask = 0b_1;

            foreach (var i in Enumerable.Range(0, Size))
            {
                if ((mask & domain) == mask)
                {
                    yield return i + Min;
                }

                mask <<= 1;
            }
        }

        public int GetDomainMax(in IState state) => state.GetDomainMaxLong(StateRef, Size) + Min;

        public int GetDomainMin(in IState state) => state.GetDomainMinLong(StateRef, Size) + Min;

        public bool SetMax(IState state, int max)
        {
            if (max >= Size + Min) return false;
            if (max < Min)
            {
                return SetDomain(state, 0);
            }

            var mask = (ulong)(Pow(2, max - Min + 1) - 1);
            var oldDom = state.GetDomainLong(StateRef, Size);
            var newDom = oldDom & mask;
            return SetDomain(state, newDom);
        }

        public bool SetMin(IState state, int min)
        {
            if (min <= Min) return false;
            if (min >= Size + Min)
            {
                return SetDomain(state, 0);
            }

            var mask = ~(ulong)(Pow(2, min - Min) - 1);
            var oldDom = state.GetDomainLong(StateRef, Size);
            var newDom = oldDom & mask;
            return SetDomain(state, newDom);
        }

        public bool TryGetValue(in IState state, out int value)
        {
            value = GetDomainMin(state);
            return value == GetDomainMax(state);
        }

        public void Initialise(IState state) => state.SetDomainLong(StateRef, Size, (ulong)(Pow(2, Size) - 1));

        public bool IsInstantiated(in IState state)
        {
            var dom = state.GetDomainLong(StateRef, Size);
            return dom != 0 && (dom & (dom - 1)) == 0;
        }

        public bool IsEmpty(in IState state) => state.GetDomainLong(StateRef, Size) == 0;

        public Type VariableType() => typeof(int);

        public bool SetValue(IState state, object value) => SetValue(state, (int)value);

        public bool SetValue(IState state, int value)
        {
            var oldD = state.GetDomainLong(StateRef, Size);
            if (oldD == 0) return false;
            var newD = (ulong)Pow(2, value - Min);
            if (newD != oldD)
            {
                state.SetDomainLong(StateRef, Size, newD & oldD);
                return true;
            }

            return false;
        }

        public bool RemoveValue(IState state, object value) => RemoveValue(state, (int)value);

        public bool RemoveValue(IState state, int value)
        {
            var oldD = state.GetDomainLong(StateRef, Size);
            var newD = oldD & ~(ulong)Pow(2, value - Min);
            if (newD != oldD)
            {
                state.SetDomainLong(StateRef, Size, newD);
                return true;
            }

            return false;
        }

        public string PrettyDomain(in IState state) => $"{{ {string.Join(", ", EnumerateDomain(state))} }}";

        int IDecisionVariable.DomainSize(in IState state) => BitCounter.Count(GetDomain(state).domain);

        int IDecisionVariable.DomainSize() => Size;

        public override int GetHashCode() => StateRef.UniqueIdentifier;

        public override bool Equals([NotNullWhen(true)] object obj) => obj is LongDomainVar intOther && Equals(intOther);

        public bool Equals(IDecisionVariable other) => other is LongDomainVar boolOther && Equals(boolOther);

        private bool Equals(LongDomainVar other) => StateRef.UniqueIdentifier == other.StateRef.UniqueIdentifier;

        public static bool operator ==(LongDomainVar left, LongDomainVar right) => left.Equals(right);

        public static bool operator !=(LongDomainVar left, LongDomainVar right) => !(left == right);
    }
}
