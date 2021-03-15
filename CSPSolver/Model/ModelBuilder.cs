using CSPSolver.common;
using CSPSolver.Variable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPSolver.Model
{
    public class ModelBuilder
    {
        private IStateBuilder _sb;
        private List<IVariable> _variables;
        private List<IConstraint> _constraints;
        public ModelBuilder(IStateBuilder sb)
        {
            _sb = sb;
            _variables = new List<IVariable>();
            _constraints = new List<IConstraint>();
        }

        public void AddConstraint(IConstraint con) => _constraints.Add(con);

        public ModelIntVar AddSmallIntVar(int min, int max)
        {
            var size = max - min + 1;
            var intVar = new IntSmallDomainVar(min, size, _sb.AddDomain(size));
            _variables.Add(intVar);
            return new ModelIntVar { variable = intVar };
        }

        public IList<ModelIntVar> AddIntVarArray(int min, int max, int count)
        {
            var size = max - min + 1;
            var intVars = new List<ModelIntVar>();
            foreach (var _ in Enumerable.Repeat(0, count))
            {
                var intVar = new IntSmallDomainVar(min, size, _sb.AddDomain(size));
                _variables.Add(intVar);
                intVars.Add(new ModelIntVar { variable = intVar });
            }

            return intVars;
        }

        public IModel GetModel()
        {
            var state = _sb.GetState();
            _variables.ForEach(v => v.initialise(state));
            return new Model(_constraints.ToArray(), _variables.ToArray(), state);
        }
    }
}
