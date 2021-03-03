using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IVariable<T> : IVariable
    {
        bool TryGetValue(IState state, out T value);
    }

    public interface IVariable
    {
        void initialise(IState state);

        bool isInstantiated(IState state);

        bool isEmpty(IState state);

        Type VariableType();

        bool SetValue(IState state, object value);

        bool RemoveValue(IState state, object value);

        string PrettyDomain(IState state);
    }
}
