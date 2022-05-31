namespace CSPSolver.common.variables
{
    public interface IRealVar : IVariable<double>
    {
        double Min { get; }

        double Max { get; }

        double GetDomainMax(in IState state);

        double GetDomainMin(in IState state);

        double Epsilon { get; }

        bool SetMax(IState state, double max);

        bool SetMin(IState state, double min);
    }
}
