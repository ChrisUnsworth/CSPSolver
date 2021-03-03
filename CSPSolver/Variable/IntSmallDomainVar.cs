using CSPSolver.common;
using CSPSolver.common.variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPSolver.Variable
{
    public readonly struct IntSmallDomainVar : ISmallIntVar
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

        public (int domain, int min, int size) GetDomain(IState state) => (state.GetDomain(StateRef, Size), Min, Size);

        public void SetDomain(IState state, int domain) => state.SetDomain(StateRef, Size, domain);

        public bool TryGetValue(IState state, out int value)
        {
            value = GetDomainMin(state);
            return value == GetDomainMax(state);
        }

        public bool isInstantiated(IState state)
        {
            var dom = state.GetDomain(StateRef, Size);
            return (dom & (dom - 1)) == 0;
        }

        public bool isEmpty(IState state) => state.GetDomain(StateRef, Size) == 0;

        public Type VariableType() => typeof(int);

        public bool SetValue(IState state, object value) => SetValue(state, (int)value);

        public bool SetValue(IState state, int value)
        {
            var oldD = state.GetDomain(StateRef, Size);
            var newD = (int)Math.Pow(2, value - Min);
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
            var newD = oldD & ~(int)Math.Pow(2, value - Min);
            state.SetDomain(StateRef, Size, newD);
            if (newD != oldD)
            {
                state.SetDomain(StateRef, Size, newD);
                return true;
            }

            return false;
        }

        public bool HasSmallDomain() => true;

        public bool SetMax(IState state, int max)
        {
            if (max >= Size + Min) return false;
            if (max < Min)
            {
                if (isEmpty(state)) return false;
                SetDomain(state, 0);
                return true;
            }

            var mask = (int)Math.Pow(2, max - Min + 1) - 1;
            var oldDom = state.GetDomain(StateRef, Size);
            var newDom = oldDom & mask;
            if (oldDom != newDom)
            {
                SetDomain(state, newDom);
                return true;
            }

            return false;
        }

        public bool SetMin(IState state, int min)
        {
            if (min <= Min) return false;
            if (min >= Size + Min)
            {
                if (isEmpty(state)) return false;
                SetDomain(state, 0);
                return true;
            }

            var mask = ~(int)(Math.Pow(2, min - Min) - 1);
            var oldDom = state.GetDomain(StateRef, Size);
            var newDom = oldDom & mask;
            if (oldDom != newDom)
            {
                SetDomain(state, newDom);
                return true;
            }

            return false;
        }

        public void initialise(IState state) => SetDomain(state, (int)Math.Pow(2, Size + 1) - 1);

        public IEnumerable<int> EnumerateDomain(IState state)
        {
            var domain = state.GetDomain(StateRef, Size);
            int mask = 0b_1;

            foreach (var i in Enumerable.Range(0, Size))
            {
                if ((mask & domain) == mask)
                {
                    yield return i + Min;
                }

                mask = mask << 1;
            }
        }

        public string PrettyDomain(IState state) => $"{{ {string.Join(", ", EnumerateDomain(state))} }}";
    }
}
