using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.utils;
using System.Linq;
using System.Collections.Generic;

namespace CSPSolver.Constraint.AllDiff
{
    public readonly struct AllDiffSameIntDomain : IConstraint
    {
        private readonly ISmallIntDomainVar[] _variables;
        private int _n => _variables.Length;

        public AllDiffSameIntDomain(IEnumerable<ISmallIntDomainVar> variables) => _variables = variables.ToArray();

        public IEnumerable<IVariable> Variables => _variables;

        public IEnumerable<IVariable> Propagate(IState state)
        {
            var result = new HashSet<IVariable>();

            foreach (var subSet in BitCounter.PowerSet(_variables, 1, _n - 1))
            {
                var union = subSet.Aggregate(0u, (u, v) => u |= v.GetDomain(state).domain);

                if (BitCounter.Count(union) <= subSet.Count)
                {
                    foreach (var p2 in _variables.Where(p => !subSet.Contains(p)))
                    {
                        if (p2.DomainMinus(state, union))
                        {
                            result.Add(p2);
                        }
                    }
                }
            }

            return result;
        }

        public bool IsMet(IState state)
        {
            throw new System.NotImplementedException();
        }

        public bool CanBeMet(IState state)
        {
            throw new System.NotImplementedException();
        }
    }
}
