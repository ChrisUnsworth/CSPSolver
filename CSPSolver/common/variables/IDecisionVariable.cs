namespace CSPSolver.common.variables
{
    public interface IDecisionVariable : IVariable
    {
        public int Size(in IState state);
    }
}
