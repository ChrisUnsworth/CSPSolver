namespace CSPSolver.common.variables
{
    public interface IBoolVar: ISmallIntDomainVar, IVariable<bool>
    {
        public bool IsTrue(in IState state);

        public bool CanBeTrue(in IState state);

        public bool IsFalse(in IState state);

        public bool CanBeFalse(in IState state);
    }
}
