namespace CSPSolver.common.variables
{
    public interface IIntVar : IVariable<int>
    {
        int Min { get; }

        int Size { get; }

        int Max { get; }

        int GetDomainMax(in IState state);

        int GetDomainMin(in IState state);

        bool SetMax(IState state, int max);

        bool SetMin(IState state, int min);

    }
}
