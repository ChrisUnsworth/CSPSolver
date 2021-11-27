using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Variable
{
    public readonly struct BoolVar: IBoolVar
    {
        public IStateRef StateRef { get; }

        public int Min => 0;

        public int Size => 2;

        public int Max => 1;

        public BoolVar(IStateRef stateRef) => StateRef = stateRef;

        public bool IsTrue(IState state) => GetDomainMin(state) == 1;

        public bool CanBeTrue(IState state) => GetDomainMax(state) == 1;

        public bool IsFalse(IState state) => GetDomainMax(state) == 0;

        public bool CanBeFalse(IState state) => GetDomainMin(state) == 0;

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

        public bool DomainMinus(IState state, uint domain) => SetDomain(state, state.GetDomain(StateRef, Size) & ~domain);

        public IEnumerable<int> EnumerateDomain(IState state)
        {
            if (CanBeFalse(state)) yield return 0;
            if (CanBeTrue(state)) yield return 1;
        }

        public int GetDomainMax(IState state) => state.GetDomainMax(StateRef, Size);

        public int GetDomainMin(IState state) => state.GetDomainMin(StateRef, Size);

        public bool SetMax(IState state, int max)
        {
            if (max >= 1) return false;
            if (max < 0) return SetDomain(state, 0);

            var newDom = state.GetDomain(StateRef, Size) & 1;
            return SetDomain(state, newDom);
        }

        public bool SetMin(IState state, int min)
        {
            if (min <= 0) return false;
            if (min > 1) return SetDomain(state, 0);

            var newDom = state.GetDomain(StateRef, Size) & 2;
            return SetDomain(state, newDom);
        }

        public bool TryGetValue(IState state, out int value)
        {
            value = GetDomainMin(state);
            return value == GetDomainMax(state);
        }

        public void Initialise(IState state) => state.SetDomain(StateRef, Size, 3);

        public bool IsInstantiated(IState state) => IsTrue(state) || IsFalse(state);

        public bool IsEmpty(IState state) => state.GetDomain(StateRef, Size) == 0;

        public Type VariableType() => typeof(bool);

        public bool SetValue(IState state, object value) =>
            Convert.ToInt32(value) switch
            {
                0 => SetDomain(state, 1),
                1 => SetDomain(state, 2),
                _ => SetDomain(state, 0),
            };

        public bool RemoveValue(IState state, object value) => 
            Convert.ToInt32(value) switch
            {
                0 => SetMin(state, 1),
                1 => SetMax(state, 0),
                _ => false,
            };

        public string PrettyDomain(IState state) => $"{{ {string.Join(", ", EnumerateDomain(state))} }}";
    }
}
