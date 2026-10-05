using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Variable;

public readonly struct BoolVar(IStateRef stateRef) : IBoolVar
{
    public int Min => 0;

    public int Size => 2;

    public int Max => 1;

    public bool IsTrue(IState state) => GetDomainMin(state) == 1;

    public bool CanBeTrue(IState state) => GetDomainMax(state) == 1;

    public bool IsFalse(IState state) => GetDomainMax(state) == 0;

    public bool CanBeFalse(IState state) => GetDomainMin(state) == 0;

    public BoolState GetState(IState state) => (BoolState)state.GetDomain(stateRef, Size);

    public (uint domain, int min, int size) GetDomain(IState state) => (state.GetDomain(stateRef, Size), Min, Size);

    public bool SetDomain(IState state, uint domain)
    {
        var oldDom = state.GetDomain(stateRef, Size);
        domain &= oldDom;
        if (domain != oldDom)
        {
            state.SetDomain(stateRef, Size, domain);
            return true;
        }

        return false;
    }

    public bool DomainMinus(IState state, uint domain) => SetDomain(state, state.GetDomain(stateRef, Size) & ~domain);

    public IEnumerable<int> EnumerateDomain(IState state)
    {
        if (CanBeFalse(state)) yield return 0;
        if (CanBeTrue(state)) yield return 1;
    }

    public int GetDomainMax(IState state) => state.GetDomainMax(stateRef, Size);

    public int GetDomainMin(IState state) => state.GetDomainMin(stateRef, Size);

    public bool SetMax(IState state, int max)
    {
        if (max >= 1) return false;
        if (max < 0) return SetDomain(state, 0);

        var newDom = state.GetDomain(stateRef, Size) & 1;
        return SetDomain(state, newDom);
    }

    public bool SetMin(IState state, int min)
    {
        if (min <= 0) return false;
        if (min > 1) return SetDomain(state, 0);

        var newDom = state.GetDomain(stateRef, Size) & 2;
        return SetDomain(state, newDom);
    }

    public bool TryGetValue(IState state, out int value)
    {
        var s = GetState(state);
        value = s == BoolState.True ? 1 : 0;
        return s is BoolState.True or BoolState.False;
    }

    public bool TryGetValue(IState state, out bool value)
    {
        var s = GetState(state);
        value = s == BoolState.True;
        return s is BoolState.True or BoolState.False;
    }

    public void Initialise(IState state) => state.SetDomain(stateRef, Size, 3);

    public DomainState GetDomainState(IState state) => GetState(state) switch
    {
        BoolState.Empty => DomainState.Empty,
        BoolState.Undecided => DomainState.Undecided,
        _ => DomainState.Instantiated
    };

    public int Id => stateRef.Id;

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