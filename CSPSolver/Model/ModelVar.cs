using CSPSolver.common;
using CSPSolver.common.variables;
using System;

namespace CSPSolver.Model
{
    public abstract class ModelVar<T>: IVariable<T>
    {
        public abstract IVariable<T> GetVariable();

        public void Initialise(IState state) => throw new NotImplementedException();

        public bool IsEmpty(in IState state) => throw new NotImplementedException();

        public bool IsInstantiated(in IState state) => throw new NotImplementedException();

        public string PrettyDomain(in IState state) => throw new NotImplementedException();

        public bool RemoveValue(IState state, object value) => throw new NotImplementedException();

        public bool SetValue(IState state, object value) => throw new NotImplementedException();

        public bool TryGetValue(in IState state, out T value) => GetVariable().TryGetValue(state, out value);

        public Type VariableType() => GetVariable().VariableType();
    }
}
