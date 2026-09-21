using System.Linq;

namespace CSPSolver.common.variables;

public interface IIntVar : IVariable<int>
{
    int Min { get; }

    int Size { get; }

    int Max { get; }

    int GetDomainMax(IState state);

    int GetDomainMin(IState state);

    bool SetMax(IState state, int max);

    bool SetMin(IState state, int min);

    // A compound var holds no state of its own, so emptying it means emptying a
    // child instead -- IsEmpty on every one of them is already an OR over
    // children, so one is enough. Otherwise, asking for a max below the current
    // min is unsatisfiable regardless of backing representation, so SetMax
    // already knows how to turn this into a genuine empty domain.
    bool IVariable.MakeEmpty(IState state) =>
        this is ICompoundVariable compound
            ? compound.GetChildren().Any(c => c.MakeEmpty(state))
            : SetMax(state, GetDomainMin(state) - 1);
}