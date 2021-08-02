using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPSolver.common
{
    public interface IModelBuilder
    {
        IModel GetModel();

        int GetStateSize();
    }
}
