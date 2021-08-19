using CSPSolver.common;
using System;

namespace CSPSolver.Model
{
    public abstract class ModelVar<T>: IVariable<T>
    {
        public abstract IVariable<T> GetVariable();

        public void Initialise(IState state) => throw new NotImplementedException();

        public bool IsEmpty(IState state) => throw new NotImplementedException();

        public bool IsInstantiated(IState state) => throw new NotImplementedException();

        public string PrettyDomain(IState state) => throw new NotImplementedException();

        public bool RemoveValue(IState state, object value) => throw new NotImplementedException();

        public bool SetValue(IState state, object value) => throw new NotImplementedException();

        public bool TryGetValue(IState state, out T value) => GetVariable().TryGetValue(state, out value);

        public Type VariableType() => GetVariable().VariableType();
    }
}
