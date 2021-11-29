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
        private readonly uint[] _domains;
        private int N => _variables.Length;

        public AllDiffSameIntDomain(IEnumerable<ISmallIntDomainVar> variables)
        {
            _variables = variables.ToArray();
            _domains = new uint[_variables.Length];
        }

        public IEnumerable<IVariable> Variables => _variables;

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            UpdateDomains(state);

            (int i1, int i2)? pair = null;

            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    if ((_domains[i] & _domains[j]) != 0)
                    {
                        if (pair.HasValue) yield break;
                        pair = (i, j);
                    }
                }
            }

            if (pair.HasValue)
            {
                if (_variables[pair.Value.i1].SetDomain(state, _domains[pair.Value.i2])) yield return _variables[pair.Value.i1];
                if (_variables[pair.Value.i2].SetDomain(state, _domains[pair.Value.i1])) yield return _variables[pair.Value.i2];
            }
            else
            {
                for (int i = 0; i < N; i++)
                {
                    if (_variables[i].SetDomain(state, 0)) yield return _variables[i];
                }
            }
        }

        public IEnumerable<IVariable> Propagate(IState state)
        {
            var result = new HashSet<IVariable>();

            foreach (var subSet in BitCounter.PowerSet(_variables, 1, N - 1))
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
            UpdateDomains(state);

            for (int i = 0; i < N; ++i)
            {
                for (int j = i+1; j < N; ++j)
                {
                    if ((_domains[i] & _domains[j]) != 0) return false;
                }
            }

            return true;
        }

        public bool CanBeMet(IState state)
        {
            UpdateDomains(state);

            var union = _domains.Aggregate(0u, (u, d) => u |= d);

            return BitCounter.Count(union) >= N;
        }

        private void UpdateDomains(IState state)
        {
            for (int i = 0; i < _variables.Length; ++i)
            {
                _domains[i] = _variables[i].GetDomain(state).domain;
            }
        }
    }
}
