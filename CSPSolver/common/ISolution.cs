using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface ISolution
    {
        T GetValue<T>(IVariable<T> v);
        IList<T> GetValues<T>(IList<IVariable<T>> v);

        int GetInt(IVariable<int> v);

        (double min, double max) GetValueRange(IVariable<double> v);
    }
}
