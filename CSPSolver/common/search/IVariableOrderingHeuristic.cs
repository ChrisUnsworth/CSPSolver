using CSPSolver.common.variables;

namespace CSPSolver.common.search
{
    public interface IVariableOrderingHeuristic
    {
        IDecisionVariable Next(in IModel model, in IState state);
    }
}
