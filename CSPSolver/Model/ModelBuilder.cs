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
        private readonly List<IVariable> _nonDecisionVariables;
        private readonly List<IConstraint> _constraints;

        public ModelBuilder() : this(new StateBuilder()) { }

        public ModelBuilder(IStateBuilder sb)
        {
            _sb = sb;
            _variables = new List<IVariable>();
            _nonDecisionVariables = new List<IVariable>();
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

        public ModelBoolVar AddBoolVar(bool isDecisionVar = true)
        {
            var boolVar = new BoolVar(_sb.AddDomain(2));
            if (isDecisionVar) _variables.Add(boolVar);
            else _nonDecisionVariables.Add(boolVar);
            return new ModelBoolVar { Variable = boolVar };
        }

        public ModelBoolVar[] AddBoolVarArray(int count, bool isDecisionVar = true)
        {
            var boolVars = new ModelBoolVar[count];

            for (int i = 0; i < count; i++)
            {
                boolVars[i] = AddBoolVar(isDecisionVar);
            }

            return boolVars;
        }

        public ModelIntVar AddIntDomainVar(int min, int max, bool isDecisionVar = true)
        {
            var size = max - min + 1;
            IIntVar intVar = size switch
            {
                <= 32 => new IntSmallDomainVar(min, size, _sb.AddDomain(size)),
                <= 64 => new LongDomainVar(min, size, _sb.AddDomain(size)),
                _     => new IntDomainVar(min, size, _sb.AddDomain(size))
            };

            if (isDecisionVar) _variables.Add(intVar);
            else _nonDecisionVariables.Add(intVar);
            return new ModelIntVar { Variable = intVar };
        }

        public ModelIntVar[] AddIntVarArray(int min, int max, int count, bool isDecisionVar = true)
        {
            var intVars = new ModelIntVar[count];

            for (int i = 0; i < count; i++)
            {
                intVars[i] = AddIntDomainVar(min, max, isDecisionVar);
            }

            return intVars;
        }

        public ModelRealVar AddRealVar(double min, double max, double epsilon = 0, bool isDecisionVar = false)
        {
            var realVar = epsilon == 0
                ? new RealVar(min, _sb.AddDouble(), max, _sb.AddDouble())
                : new RealVar(min, _sb.AddDouble(), max, _sb.AddDouble(), epsilon);

            if (isDecisionVar) _variables.Add(realVar);
            else _nonDecisionVariables.Add(realVar);
            return new ModelRealVar { Variable = realVar };
        }

        public ModelRealVar[] AddRealVarArray(double min, double max, int count, double epsilon = 0, bool isDecisionVar = false)
        {
            var realVars = new ModelRealVar[count];

            for (int i = 0; i < count; i++)
            {
                realVars[i] = AddRealVar(min, max, epsilon, isDecisionVar);
            }

            return realVars;
        }

        public IModel GetModel() => new Model(_constraints.ToArray(), _variables.ToArray(), _nonDecisionVariables.ToArray(), _objective, _maximise);        

        public int GetStateSize() =>_sb.GetSize();
    }
}
