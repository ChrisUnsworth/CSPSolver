using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IModelBuilder
    {
        IModel GetModel();

        IVariable AddIntVar(int min, int max);

        IList<IVariable> AddIntVarArray(int min, int max, int count);

        void AddConstraint(IConstraint con);
    }
}
