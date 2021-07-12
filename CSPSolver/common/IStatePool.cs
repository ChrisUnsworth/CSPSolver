using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPSolver.common
{
    public interface IStatePool
    {
        IState Copy(IState state);

        void Return(IState state);
    }
}
