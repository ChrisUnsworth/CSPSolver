using System.Linq;

namespace CSPSolver.common.variables;

public interface IRealVar : IVariable<double>
{
    double Min { get; }

    double Max { get; }

    double GetDomainMax(IState state);

    double GetDomainMin(IState state);

    /// <summary>Min, max and state in one pass, for callers that need more than one of them.</summary>
    RealBounds GetBounds(IState state);

    double Epsilon { get; }

    bool SetMax(IState state, double max);

    bool SetMin(IState state, double min);

    bool IVariable.MakeEmpty(IState state) =>
        this is ICompoundVariable compound
            ? compound.GetChildren().Any(c => c.MakeEmpty(state))
            : SetMax(state, GetDomainMin(state) - Epsilon);
}