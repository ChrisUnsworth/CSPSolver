using System;
using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.AllDiff;
using CSPSolver.State;
using CSPSolver.Variable;
using CSPSolver;

namespace CSPSolver.Model
{
    public class ModelBuilder : IModelBuilder
    {
        private readonly IStateBuilder _sb;
        private IIntVar _objective;
        private bool _maximise;
        private readonly List<IVariable> _variables;
        private readonly List<IConstraint> _constraints;

        public ModelBuilder() : this(new StateBuilder()) { }

        public ModelBuilder(IStateBuilder sb)
        {
            _sb = sb;
            _variables = new List<IVariable>();
            _constraints = new List<IConstraint>();
        }

        public IEnumerable<ISolution> Search() => new Search.Search(this);

        public void AddObjective(ModelIntVar objective, bool maximise) => AddObjective((IIntVar)objective.GetVariable(), maximise);
        public void AddObjective(IIntVar objective, bool maximise)
        {
            _objective = objective;
            _maximise = maximise;
        }

        public void AddConstraint(ModelConstraint con) => AddConstraint(con.Constraint);

        public void AddConstraint(IConstraint con) => _constraints.Add(con);

        public ModelBoolVar AddBoolVar()
        {
            var boolVar = new BoolVar(_sb.AddDomain(2));
            _variables.Add(boolVar);
            return new ModelBoolVar { Variable = boolVar };
        }

        public ModelBoolVar[] AddBoolVarArray(int count)
        {
            var boolVars = new ModelBoolVar[count];

            for (int i = 0; i < count; i++)
            {
                boolVars[i] = AddBoolVar();
            }

            return boolVars;
        }

        public ModelIntVar AddIntDomainVar(int min, int max)
        {
            var size = max - min + 1;
            IIntVar intVar = size switch
            {
                <= 32 => new IntSmallDomainVar(min, size, _sb.AddDomain(size)),
                <= 64 => new LongDomainVar(min, size, _sb.AddDomain(size)),
                _     => new IntDomainVar(min, size, _sb.AddDomain(size))
            };
            _variables.Add(intVar);
            return new ModelIntVar { Variable = intVar };
        }

        public ModelIntVar[] AddIntVarArray(int min, int max, int count)
        {
            var intVars = new ModelIntVar[count];

            for (int i = 0; i < count; i++)
            {
                intVars[i] = AddIntDomainVar(min, max);
            }

            return intVars;
        }

        public IModel GetModel() => new Model(_constraints.ToArray(), _variables.ToArray(), _objective, _maximise);        

        public int GetStateSize() =>_sb.GetSize();
    }
}
