using CSPSolver.common;
using System;

namespace CSPSolver.Variable
{
    public readonly struct IntDomainVar : IIntVar
    {
        public IStateRef StateRef { get; }
        public int Min { get; }
        public int Size { get; }

        public IntDomainVar(int min, int size, IStateRef stateRef)
        {
            StateRef = stateRef;
            Min = min;
            Size = size;
        }

        public int GetDomainMax(IState state) => state.GetDomainMax(StateRef, Size) + Min;

        public int GetDomainMin(IState state) => state.GetDomainMin(StateRef, Size) + Min;

        public int GetDomain(IState state) => state.GetDomain(StateRef, Size);

        public void SetDomain(IState state, int domain) => state.SetDomain(StateRef, Size, domain);

        public bool TryGetValue(IState state, out int value)
        {
            value = GetDomainMin(state);
            return value == GetDomainMax(state);
        }

        public bool isInstantiated(IState state)
        {
            var dom = GetDomain(state);
            return (dom & (dom - 1)) == 0;
        }

        public bool isEmpty(IState state) => GetDomain(state) == 0;

        public Type VariableType() => typeof(int);

        public bool SetValue(IState state, object value) => SetValue(state, (int)value);

        public bool SetValue(IState state, int value)
        {
            var oldD = GetDomain(state);
            var newD = (int)Math.Pow(2, value - Min);
            state.SetDomain(StateRef, Size, newD);
            if (newD != oldD)
            {
                state.SetDomain(StateRef, Size, newD);
                return true;
            }

            return false;
        }

        public bool RemoveValue(IState state, object value) => RemoveValue(state, value);

        public bool RemoveValue(IState state, int value)
        {
            var oldD = GetDomain(state);
            var newD = oldD & ~(int)Math.Pow(2, value - Min);
            state.SetDomain(StateRef, Size, newD);
            if (newD != oldD)
            {
                state.SetDomain(StateRef, Size, newD);
                return true;
            }

            return false;
        }
    }
}
