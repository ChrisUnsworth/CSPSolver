using CSPSolver.common;
using CSPSolver.Variable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPSolver.Model
{
    public class ModelBuilder : IModelBuilder
    {
        private IStateBuilder _sb;
        private IList<IVariable> _variables;
        private IList<IConstraint> _constraints;
        public ModelBuilder(IStateBuilder sb)
        {
            _sb = sb;
            _variables = new List<IVariable>();
            _constraints = new List<IConstraint>();
        }

        public void AddConstraint(IConstraint con) => _constraints.Add(con);

        public IVariable AddIntVar(int min, int max)
        {
            var size = max - min;
            var intVar = new IntSmallDomainVar(min, size, _sb.AddDomain(size));
            _variables.Add(intVar);
            return intVar;
        }

        public IList<IVariable> AddIntVarArray(int min, int max, int count)
        {
            var size = max - min;
            var intVars = new List<IVariable>();
            foreach (var _ in Enumerable.Repeat(0, count))
            {
                var intVar = new IntSmallDomainVar(min, size, _sb.AddDomain(size));
                _variables.Add(intVar);
                intVars.Add(intVar);
            }

            return intVars;
        }

        public IModel GetModel() => new Model(_constraints.ToArray(), _variables.ToArray(), _sb.GetState());
    }
}
