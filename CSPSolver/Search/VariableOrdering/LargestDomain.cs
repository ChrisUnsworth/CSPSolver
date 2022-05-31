using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.common.variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPSolver.Search.VariableOrdering
{
    public readonly struct LargestDomain : IVariableOrderingHeuristic
    {
        public IVariable Next(in IModel model, in IState state)
        {
            var s = state;
            foreach (var v in model.Variables.Where(_ => !_.IsInstantiated(s)))
            {
                //TODO: make right
                if (!v.IsInstantiated(state)) return v;
            }

            return null;
        }
    }
}
