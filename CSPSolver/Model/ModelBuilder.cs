using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.AllDiff;
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

        public void AddAllDiff(IEnumerable<ModelIntVar> vars)
        {
            var intVars = vars.Select(v => v.variable as ISmallIntDomainVar).Where(s => s != null);
            if (intVars.Any() && intVars.Min(v => v.Min) == intVars.Max(v => v.Min))
            {
                _constraints.Add(new AllDiffSameIntDomain(intVars));
            }
            else
            {
                throw new NotImplementedException();
            }            
        }

        public ModelIntVar AddIntDomainVar(int min, int max)
        {
            var size = max - min + 1;
            IIntVar intVar = size > 32 
                    ? new IntDomainVar(min, size, _sb.AddDomain(size))
                    : new IntSmallDomainVar(min, size, _sb.AddDomain(size));
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

        public (IModel, IState) GetModel()
        {
            var state = _sb.GetState();
            _variables.ForEach(v => v.initialise(state));
            return (new Model(_constraints.ToArray(), _variables.ToArray()), state);
        }
    }
}
