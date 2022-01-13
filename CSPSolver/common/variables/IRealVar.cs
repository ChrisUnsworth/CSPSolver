namespace CSPSolver.common.variables
{
    public interface IRealVar : IVariable<double>
    {
        double Min { get; }

        double Max { get; }

        double GetDomainMax(IState state);

        double GetDomainMin(IState state);

        bool SetMax(IState state, double max);

        bool SetMin(IState state, double min);
    }
}
