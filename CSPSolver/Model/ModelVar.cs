using CSPSolver.common;
using System;

namespace CSPSolver.Model;

public abstract class ModelVar<T> : IVariable<T>
{
    public abstract IVariable<T> GetVariable();

    public void Initialise(IState state) => GetVariable().Initialise(state);

    public int Id => GetVariable().Id;

    public DomainState GetDomainState(IState state) => GetVariable().GetDomainState(state);

    public bool MakeEmpty(IState state) => GetVariable().MakeEmpty(state);

    public string PrettyDomain(IState state) => GetVariable().PrettyDomain(state);

    public bool RemoveValue(IState state, object value) => GetVariable().RemoveValue(state, value);

    public bool SetValue(IState state, object value) => GetVariable().SetValue(state, value);

    public bool TryGetValue(IState state, out T value) => GetVariable().TryGetValue(state, out value);

    public Type VariableType() => GetVariable().VariableType();
}