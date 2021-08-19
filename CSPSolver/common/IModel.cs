using System.Collections.Generic;

namespace CSPSolver.common
{
    public interface IModel
    {
        bool Maximise { get; }

        IVariable Objective { get; }

        IEnumerable<IVariable> Variables { get; }

        IEnumerable<IConstraint> Constraints { get; }

        void Propagate(IState state);

        bool IsSolved(IState state);

        bool HasEmptyDomain(IState state);

        string PrettyDomains(IState state);

        void Initialise(IState state);
    }
}
