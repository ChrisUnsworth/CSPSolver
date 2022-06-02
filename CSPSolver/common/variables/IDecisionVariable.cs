using System;

namespace CSPSolver.common.variables
{
    public interface IDecisionVariable : IVariable, IEquatable<IDecisionVariable>
    {
        public int DomainSize(in IState state);

        public int DomainSize();
    }
}
