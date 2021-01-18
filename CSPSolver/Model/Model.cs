using CSPSolver.common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Model
{
    public class Model : IModel
    {
        public IState State { get; set; }

        public IVariable[] Variables { get; }

        public IConstraint[] Constraints { get; }

        IEnumerable<IVariable> IModel.Variables => Variables;

        IEnumerable<IConstraint> IModel.Constraints => Constraints;

        private IDictionary<IVariable, List<IConstraint>> _constraintLookup;

        public Model(IConstraint[] constraints, IVariable[] variables, IState state)
        {
            Variables = variables;
            Constraints = constraints;
            State = state;
            _constraintLookup = constraints
                .SelectMany(c => c.Variables.Select(v => (v, c)))
                .GroupBy(x => x.v)
                .ToDictionary(g => g.Key, g => g.Select(y => y.c).ToList());
        }

        public void propagate()
        {
            var todo = Constraints.ToList();

            while (todo.Any())
            {
                var changedVariables = todo.SelectMany(c => c.Propagate(State)).Distinct();
                todo = changedVariables.SelectMany(v => _constraintLookup[v]).Distinct().ToList();
            }
        }

        public bool IsSolved() => Variables.All(v => v.isInstantiated(State));

        public bool HasEmptyDomain() => Variables.Any(v => v.isEmpty(State));
    }
}
