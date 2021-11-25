using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.AllDiff;
using CSPSolver.Variable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPSolver.Model
{
    public class ModelBuilder : IModelBuilder
    {
        private readonly IStateBuilder _sb;
        private IIntVar _objective;
        private bool _maximise;
        private readonly List<IVariable> _variables;
        private readonly List<IConstraint> _constraints;
        public ModelBuilder(IStateBuilder sb)
        {
            _sb = sb;
            _variables = new List<IVariable>();
            _constraints = new List<IConstraint>();
        }


        public void AddObjective(ModelIntVar objective, bool maximise) => AddObjective((IIntVar)objective.GetVariable(), maximise);
        public void AddObjective(IIntVar objective, bool maximise)
        {
            _objective = objective;
            _maximise = maximise;
        }

        public void AddConstraint(ModelConstraint con) => AddConstraint(con.Constraint);

        public void AddConstraint(IConstraint con) => _constraints.Add(con);

        public void AddAllDiff(IEnumerable<ModelIntVar> vars)
        {
            var intVars = vars.Select(v => v.Variable as ISmallIntDomainVar).Where(s => s != null);
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
            return new ModelIntVar { Variable = intVar };
        }

        public IList<ModelIntVar> AddIntVarArray(int min, int max, int count)
        {
            var intVars = new List<ModelIntVar>();
            foreach (var _ in Enumerable.Repeat(0, count))
            {
                intVars.Add(AddIntDomainVar(min, max));
            }

            return intVars;
        }

        public IModel GetModel() => new Model(_constraints.ToArray(), _variables.ToArray(), _objective, _maximise);        

        public int GetStateSize() =>_sb.GetSize();
    }
}
