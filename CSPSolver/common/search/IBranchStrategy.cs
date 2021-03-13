using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common.search
{
    public interface IBranchStrategy
    {
        IEnumerable<IState> Branch(IModel model);
    }
}
