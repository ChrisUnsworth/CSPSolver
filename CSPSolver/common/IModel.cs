using CSPSolver.common.variables;
using System.Collections.Generic;

namespace CSPSolver.common
{
    public interface IModel
    {
        bool Maximise { get; }

        IVariable Objective { get; }

        IEnumerable<IDecisionVariable> Variables { get; }

        IEnumerable<IConstraint> Constraints { get; }

        void Propagate(IState _);

        bool IsSolved(IState _);

        bool HasEmptyDomain(IState _);

        string PrettyDomains(IState _);

        void Initialise(IState _);

        IEnumerable<IConstraint> IsConstrainedBy(IVariable _);

        IEnumerable<IVariable> IsLinkedTo(IVariable _);
    }
}
