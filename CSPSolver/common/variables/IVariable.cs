using System;

namespace CSPSolver.common.variables
{
    public interface IVariable<T> : IVariable
    {
        bool TryGetValue(in IState state, out T value);
    }

    public interface IVariable
    {
        void Initialise(IState state);

        bool IsInstantiated(in IState state);

        bool IsEmpty(in IState state);

        Type VariableType();

        bool SetValue(IState state, object value);

        bool RemoveValue(IState state, object value);

        string PrettyDomain(in IState state);
    }
}