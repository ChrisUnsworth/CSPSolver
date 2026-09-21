namespace CSPSolver.common.search;

public interface IVariableOrderingHeuristic
{
    IVariable Next(in IModel model, in IState state);
}