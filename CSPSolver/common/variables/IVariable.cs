using System;

namespace CSPSolver.common;

public interface IVariable<T> : IVariable
{
    bool TryGetValue(IState state, out T value);
}

public interface IVariable
{
    /// <summary>The Id of the variable state, or -1 if no state.</summary>
    int Id { get; }

    void Initialise(IState state);

    DomainState GetDomainState(IState state);

    bool MakeEmpty(IState state);

    Type VariableType();

    bool SetValue(IState state, object value);

    bool RemoveValue(IState state, object value);

    string PrettyDomain(IState state);
}