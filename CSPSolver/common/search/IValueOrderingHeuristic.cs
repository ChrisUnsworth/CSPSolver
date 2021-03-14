using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common.search
{
    public interface IValueOrderingHeuristic
    {
        IEnumerable<object> Order(in IModel model, in IVariable variable);
    }
}
