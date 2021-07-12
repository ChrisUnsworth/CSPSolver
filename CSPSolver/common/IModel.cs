using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IModel
    {
        IEnumerable<IVariable> Variables { get; }

        IEnumerable<IConstraint> Constraints { get; }

        void propagate(IState state);

        bool IsSolved(IState state);

        bool HasEmptyDomain(IState state);

        string PrettyDomains(IState state);
    }
}
