using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPSolver.common.variables
{
    public interface IBoolVar: ISmallIntDomainVar, IVariable<bool>
    {
        public bool IsTrue(IState state);

        public bool CanBeTrue(IState state);

        public bool IsFalse(IState state);

        public bool CanBeFalse(IState state);
    }
}
