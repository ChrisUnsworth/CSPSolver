using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Variable;

public readonly struct FalseVar : IBoolVar
{
    public IStateRef StateRef { get; }

    public int Min => 0;

    public int Size => 1;

    public int Max => 0;

    public FalseVar(IStateRef stateRef) => StateRef = stateRef;

    public bool IsTrue(IState state) => false;

    public bool CanBeTrue(IState state) => false;

    public bool IsFalse(IState state) => GetDomainState(state) != DomainState.Empty;

    public bool CanBeFalse(IState state) => GetDomainState(state) != DomainState.Empty;

    public BoolState GetState(IState state) => GetDomainState(state) == DomainState.Empty ? BoolState.Empty : BoolState.False;

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
    }

    public int GetDomainMax(IState state) => state.GetDomainMax(StateRef, Size);

    public int GetDomainMin(IState state) => state.GetDomainMin(StateRef, Size);

    public IntBounds GetBounds(IState state) =>
        state.GetDomain(StateRef, Size) == 0
            ? new IntBounds(GetDomainMin(state), GetDomainMax(state), DomainState.Empty)
            : new IntBounds(0, 0, DomainState.Instantiated);

    public bool SetMax(IState state, int max)
    {
        if (max >= 0) return false;
        return SetDomain(state, 0);
    }

    public bool SetMin(IState state, int min)
    {
        if (min <= 0) return false;
        return SetDomain(state, 0);
    }

    public bool TryGetValue(IState state, out int value)
    {
        value = 0;
        return GetDomainState(state) != DomainState.Empty;
    }

    public bool TryGetValue(IState state, out bool value)
    {
        value = false;
        return IsFalse(state);
    }

    public void Initialise(IState state) => state.SetDomain(StateRef, Size, 1);

    public DomainState GetDomainState(IState state) =>
        state.GetDomain(StateRef, Size) == 0 ? DomainState.Empty : DomainState.Instantiated;

    public int Id => StateRef.Id;

    public Type VariableType() => typeof(bool);

    public bool SetValue(IState state, object value) =>
        Convert.ToInt32(value) switch
        {
            0 => false,
            _ => SetDomain(state, 0),
        };

    public bool RemoveValue(IState state, object value) =>
        Convert.ToInt32(value) switch
        {
            0 => SetDomain(state, 0),
            _ => false,
        };

    public string PrettyDomain(IState state) => $"{{ {string.Join(", ", EnumerateDomain(state))} }}";
}