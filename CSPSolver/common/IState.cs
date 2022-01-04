namespace CSPSolver.common
{
    public interface IState
    {
        int GetInt(in IStateRef idx);
        void SetInt(in IStateRef idx, in int value);

        long GetLong(in IStateRef idx);
        void SetLong(in IStateRef idx, in long value);

        double GetDouble(in IStateRef idx);
        void SetDouble(in IStateRef idx, in double value);

        float GetFloat(in IStateRef idx);
        void SetFloat(in IStateRef idx, in float value);

        uint GetDomain(in IStateRef idx, in int size);
        ulong GetDomainLong(in IStateRef idx, in int size);
        int GetDomainMax(in IStateRef idx, in int size);
        int GetDomainMaxLong(in IStateRef idx, in int size);
        int GetLargeDomainMax(in IStateRef idx, in int size);
        int GetDomainMin(in IStateRef idx, in int size);
        int GetDomainMinLong(in IStateRef idx, in int size);
        int GetLargeDomainMin(in IStateRef idx, in int size);
        void SetDomain(in IStateRef idx, in int size, in uint value);
        void SetDomainLong(in IStateRef idx, in int size, in ulong value);

        uint[] GetLargeDomain(in IStateRef idx, in int size);
        void SetLargeDomain(in IStateRef idx, in int size, in uint[] value);

        bool IsSameAs(IState other);
    }
}
