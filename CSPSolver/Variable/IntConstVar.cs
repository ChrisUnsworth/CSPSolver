using CSPSolver.common;
using CSPSolver.common.variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPSolver.Variable;

public readonly struct IntConstVar : ISmallIntDomainVar
{
    public int Min => _value;

    public int Size => 1;

    public int Max => _value;

    private readonly int _value;

    public IntConstVar(int value) => _value = value;


    public bool DomainMinus(IState state, uint domain) => false;

    public IEnumerable<int> EnumerateDomain(IState state) => Enumerable.Repeat(_value, 1);

    public (uint domain, int min, int size) GetDomain(IState state) => (1, _value, 1);

    public int GetDomainMax(IState state) => Max;

    public int GetDomainMin(IState state) => Min;

    public IntBounds GetBounds(IState state) => new(_value, _value, DomainState.Instantiated);

    public void Initialise(IState state) { }

    public int Id => -1;

    public DomainState GetDomainState(IState state) => DomainState.Instantiated;

    public string PrettyDomain(IState state) => $"{_value}";

    public bool RemoveValue(IState state, object value) => false;

    public bool SetDomain(IState state, uint domain) => false;

    public bool SetMax(IState state, int max) => false;

    public bool SetMin(IState state, int min) => false;

    public bool SetValue(IState state, object value) => false;

    public bool TryGetValue(IState state, out int value)
    {
        value = _value;
        return true;
    }

    public Type VariableType() => typeof(int);
}