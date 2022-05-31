using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Variable
{
    public readonly struct TrueVar : IBoolVar
    {
        public IStateRef StateRef { get; }

        public int Min => 1;

        public int Size => 1;

        public int Max => 1;

        public TrueVar(IStateRef stateRef) => StateRef = stateRef;

        public bool IsTrue(in IState state) => !IsEmpty(state);

        public bool CanBeTrue(in IState state) => !IsEmpty(state);

        public bool IsFalse(in IState state) => false;

        public bool CanBeFalse(in IState state) => false;

        public (uint domain, int min, int size) GetDomain(in IState state) => (state.GetDomain(StateRef, Size), Min, Size);

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
            if (CanBeTrue(state)) yield return 1;
        }

        public int GetDomainMax(in IState state) => state.GetDomainMax(StateRef, Size) + 1;

        public int GetDomainMin(in IState state) => state.GetDomainMin(StateRef, Size) + 1;

        public bool SetMax(IState state, int max)
        {
            if (max >= 1) return false;
            return SetDomain(state, 0);
        }

        public bool SetMin(IState state, int min)
        {
            if (min <= 1) return false;
            return SetDomain(state, 0);
        }

        public bool TryGetValue(in IState state, out int value)
        {
            value = 1;
            return !IsEmpty(state);
        }

        public bool TryGetValue(in IState state, out bool value)
        {
            value = true;
            return IsTrue(state);
        }

        public void Initialise(IState state) => state.SetDomain(StateRef, Size, 1);

        public bool IsInstantiated(in IState state) => IsTrue(state);

        public bool IsEmpty(in IState state) => state.GetDomain(StateRef, Size) == 0;

        public Type VariableType() => typeof(bool);

        public bool SetValue(IState state, object value) =>
            Convert.ToInt32(value) switch
            {
                1 => false,
                _ => SetDomain(state, 0),
            };

        public bool RemoveValue(IState state, object value) =>
            Convert.ToInt32(value) switch
            {
                1 => SetDomain(state, 0),
                _ => false,
            };

        public string PrettyDomain(in IState state) => $"{{ {string.Join(", ", EnumerateDomain(state))} }}";
    }
}
