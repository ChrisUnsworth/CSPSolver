using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IModel
    {
        IState State { get; set; }

        IEnumerable<IVariable> Variables { get; }

        IEnumerable<IConstraint> Constraints { get; }

        void propagate();

        bool IsSolved();

        bool HasEmptyDomain();

        string PrettyDomains();
    }
}
