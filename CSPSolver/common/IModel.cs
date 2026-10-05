using System.Collections.Generic;

namespace CSPSolver.common;

public interface IModel
{
    bool Maximise { get; }

    IVariable Objective { get; }

    IEnumerable<IVariable> Variables { get; }

    IEnumerable<IConstraint> Constraints { get; }

    void Propagate(IState state);

    /// <summary>
    /// Empty if any variable is empty, Instantiated if every decision variable is fixed (a solution), otherwise Undecided.
    /// </summary>
    DomainState GetDomainState(IState state);

    string PrettyDomains(IState state);

    void Initialise(IState state);
}