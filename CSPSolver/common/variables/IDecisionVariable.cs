using System;

namespace CSPSolver.common.variables
{
    public interface IDecisionVariable : IVariable, IEquatable<IDecisionVariable>
    {
        public int Size(in IState state);
    }
}
