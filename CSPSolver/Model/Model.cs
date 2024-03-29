﻿using CSPSolver.common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CSPSolver.common.variables;
using System.Collections.Immutable;

namespace CSPSolver.Model
{
    public readonly struct Model : IModel
    {
        public bool Maximise { get; }

        public IVariable Objective { get; }

        public IVariable[] Variables { get; }

        private IVariable[] NonDecisionVariables { get; }

        public IConstraint[] Constraints { get; }

        IEnumerable<IVariable> IModel.Variables => Variables;

        IEnumerable<IConstraint> IModel.Constraints => Constraints;

        private readonly ImmutableDictionary<IVariable, List<IConstraint>> _constraintLookup;

        public Model(IConstraint[] constraints, IVariable[] variables, IVariable[] nonDecisionVariables, IVariable objective = null, bool? maximise = null)
        {
            Maximise = maximise ?? true;
            Objective = objective;
            Variables = variables;
            NonDecisionVariables = nonDecisionVariables;
            Constraints = constraints;
            var keyValuePairs = constraints
                .SelectMany(c => c.Variables.Select(v => (v, c)))
                .SelectMany(x => x.v is ICompoundVariable cv ? cv.GetChildren().Select(v => (v, x.c)) : Enumerable.Repeat(x, 1))
                .GroupBy(x => x.v)
                .ToDictionary(g => g.Key, g => g.Select(y => y.c).ToList());
            _constraintLookup = ImmutableDictionary<IVariable, List<IConstraint>>.Empty.AddRange(keyValuePairs);
        }

        public void Propagate(IState state)
        {
            var todo = Constraints.ToList();

            while (todo.Any())
            {
                var changedVariables = 
                    todo.SelectMany(c => c.Propagate(state))
                        .SelectMany(v => v is ICompoundVariable cv ? cv.GetChildren() : Enumerable.Repeat(v, 1))
                        .Distinct();
                todo = GetAssociatedConstraints(changedVariables);
            }
        }

        private List<IConstraint> GetAssociatedConstraints(IEnumerable<IVariable> variables)
        {
            var set = new HashSet<IConstraint>();

            foreach (var v in variables)
            {
                foreach (var c in _constraintLookup[v])
                {
                    set.Add(c);
                }
            }

            return set.ToList();
        }

        public string PrettyDomains(IState state)
        {
            var sb = new StringBuilder();

            foreach (var v in Variables)
            {
                sb.AppendLine(v.PrettyDomain(state));
            }

            return sb.ToString();
        }

        public bool IsSolved(IState state) => Variables.All(v => v.IsInstantiated(state));

        public bool HasEmptyDomain(IState state) => Variables.Concat(NonDecisionVariables).Any(v => v.IsEmpty(state));

        public void Initialise(IState state)
        {
            foreach (var v in Variables) v.Initialise(state);
            foreach (var v in NonDecisionVariables) v.Initialise(state);
        }
    }
}
