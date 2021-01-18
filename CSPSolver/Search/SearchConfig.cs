using CSPSolver.common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Search
{
    public class SearchConfig
    {
        public Func<IModel, IVariable> VariableOrderingHeuristic { get; set; } = DefaultVariableOrderingHeuristic;
        public Func<IModel, IVariable, object> ValueOrderingHeuristic { get; set; } = DefaultValueOrderingHeuristic;

        private static IVariable DefaultVariableOrderingHeuristic(IModel model) =>
            model.Variables.FirstOrDefault(v => !v.isInstantiated(model.State) && !v.isEmpty(model.State));

        private static object DefaultValueOrderingHeuristic(IModel model, IVariable variable) => 
            (variable as IIntVar).GetDomainMin(model.State);
    }
}
