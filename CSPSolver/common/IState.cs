using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IState
    {
        int GetInt(IStateRef idx);
        void SetInt(IStateRef idx, int value);

        long GetLong(IStateRef idx);
        void SetLong(IStateRef idx, long value);

        double GetDouble(IStateRef idx);
        void SetDouble(IStateRef idx, double value);

        float GetFloat(IStateRef idx);
        void SetFloat(IStateRef idx, float value);

        int GetDomain(IStateRef idx, int size);
        int GetDomainMax(IStateRef idx, int size);
        int GetDomainMin(IStateRef idx, int size);
        void SetDomain(IStateRef idx, int size, int value);

        int[] GetLargeDomain(IStateRef idx, int size);
        void SetLargeDomain(IStateRef idx, int size, int[] value);

        IState Copy();
    }
}
