using System;
using System.Collections.Generic;
using System.Linq;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Variable
{
    public readonly struct IntSmallDomainVar : ISmallIntDomainVar
    {
        public IStateRef StateRef { get; }
        public int Min { get; }
        public int Size { get; }

        public int Max => Min + Size - 1;

        public IntSmallDomainVar(int min, int size, IStateRef stateRef)
        {
            StateRef = stateRef;
            Min = min;
            Size = size;
        }

        public int GetDomainMax(IState state) => state.GetDomainMax(StateRef, Size) + Min;

        public int GetDomainMin(IState state) => state.GetDomainMin(StateRef, Size) + Min;

        public (uint domain, int min, int size) GetDomain(IState state) => (state.GetDomain(StateRef, Size), Min, Size);

        public bool SetDomain(IState state, uint domain)
        {
            var oldDom = state.GetDomain(StateRef, Size);
            domain &= oldDom;
            if (domain != oldDom)
            {
                state.SetDomain(StateRef, Size, domain);
                return true;
            }

            return false;
        }

        public bool DomainMinus(IState state, uint domain) => SetDomain(state, ~domain);

        public bool TryGetValue(IState state, out int value)
        {
            value = GetDomainMin(state);
            return value == GetDomainMax(state);
        }

        public bool IsInstantiated(IState state)
        {
            var dom = state.GetDomain(StateRef, Size);
            return dom != 0 && (dom & (dom - 1)) == 0;
        }

        public bool IsEmpty(IState state) => state.GetDomain(StateRef, Size) == 0;

        public Type VariableType() => typeof(int);

        public bool SetValue(IState state, object value) => SetValue(state, (int)value);

        public bool SetValue(IState state, int value)
        {
            var oldD = state.GetDomain(StateRef, Size);
            if (oldD == 0) return false;
            var newD = (uint)Pow(2, value - Min);
            if (newD != oldD)
            {
                state.SetDomain(StateRef, Size, newD & oldD);
                return true;
            }

            return false;
        }

        public bool RemoveValue(IState state, object value) => RemoveValue(state, (int)value);

        public bool RemoveValue(IState state, int value)
        {
            var oldD = state.GetDomain(StateRef, Size);
            var newD = oldD & ~(uint)Pow(2, value - Min);
            if (newD != oldD)
            {
                state.SetDomain(StateRef, Size, newD);
                return true;
            }

            return false;
        }

        public bool SetMax(IState state, int max)
        {
            if (max >= Size + Min) return false;
            if (max < Min)
            {
                return SetDomain(state, 0);
            }

            var mask = (uint)Pow(2, max - Min + 1) - 1;
            var oldDom = state.GetDomain(StateRef, Size);
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

            var mask = ~(uint)(Pow(2, min - Min) - 1);
            var oldDom = state.GetDomain(StateRef, Size);
            var newDom = oldDom & mask;
            return SetDomain(state, newDom);
        }

        public void Initialise(IState state) => state.SetDomain(StateRef, Size, (uint)(Pow(2, Size) - 1));

        public IEnumerable<int> EnumerateDomain(IState state)
        {
            var domain = state.GetDomain(StateRef, Size);
            uint mask = 0b_1;

            foreach (var i in Enumerable.Range(0, Size))
            {
                if ((mask & domain) == mask)
                {
                    yield return i + Min;
                }

                mask <<= 1;
            }
        }

        public string PrettyDomain(IState state) => $"{{ {string.Join(", ", EnumerateDomain(state))} }}";
    }
}
