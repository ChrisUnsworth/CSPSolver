namespace CSPSolver.common;

public interface IStatePool
{
    IState Copy(IState state);

    void Return(IState state);
}