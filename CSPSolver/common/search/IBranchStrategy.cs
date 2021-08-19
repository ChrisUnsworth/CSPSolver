using System.Collections.Generic;

namespace CSPSolver.common.search
{
    public interface IBranchStrategy
    {
        IEnumerable<IState> Branch(in IModel model, in IState state, IStatePool statePool);
    }
}
