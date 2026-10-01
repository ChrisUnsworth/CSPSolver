using System;

namespace CSPSolver.common;

public interface IVariable<T> : IVariable
{
    bool TryGetValue(IState state, out T value);
}

public interface IVariable
{
    /// <summary>The Id of the variable's own state, or -1 for constants and expressions, which have none.</summary>
    int Id { get; }

    void Initialise(IState state);

    bool IsInstantiated(IState state);

    bool IsEmpty(IState state);

    bool MakeEmpty(IState state);

    Type VariableType();

    bool SetValue(IState state, object value);

    bool RemoveValue(IState state, object value);

    string PrettyDomain(IState state);
}