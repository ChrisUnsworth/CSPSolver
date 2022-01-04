using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.utils;

namespace CSPSolver.Constraint.AllDiff
{
    public readonly struct AllDiffLongDomain : IConstraint
    {
        private readonly ILongDomainVar[] _variables;
        private readonly ulong[] _domains;
        private int N => _variables.Length;
        private readonly int _minMin;

        public AllDiffLongDomain(IEnumerable<ILongDomainVar> variables)
        {
            _variables = variables.ToArray();
            _domains = new ulong[_variables.Length];
            _minMin = variables.Select(v => v.Min).Min();
        }

        public IEnumerable<IVariable> Variables => _variables;

        public IEnumerable<IVariable> Propagate(IState state)
        {
            UpdateDomains(state);
            var result = new HashSet<IVariable>();

            foreach (var subSet in BitCounter.PowerSetIndices(_variables, 1, N - 1))
            {
                var union = 0ul;
                foreach (var i in subSet)
                {
                    union |= _domains[i];
                }

                if (BitCounter.Count(union) <= subSet.Count)
                {
                    foreach (var p2 in _variables.Where((v, i) => !subSet.Contains(i)))
                    {
                        if (p2.DomainMinus(state, union >> (p2.Min - _minMin)))
                        {
                            result.Add(p2);
                        }
                    }
                }
            }

            return result;
        }

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            (int i1, int i2)? pair = null;

            foreach (int i in UpdateDomainsEnumerable(state))
            {
                for (int j = 0; j < i; j++)
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
                if (_variables[pair.Value.i1].SetDomain(state, RightShift(_domains[pair.Value.i2], pair.Value.i1))) yield return _variables[pair.Value.i1];
                if (_variables[pair.Value.i2].SetDomain(state, RightShift(_domains[pair.Value.i1], pair.Value.i2))) yield return _variables[pair.Value.i2];
            }
            else
            {
                for (int i = 0; i < N; i++)
                {
                    if (_variables[i].SetDomain(state, 0)) yield return _variables[i];
                }
            }
        }

        public bool IsMet(IState state)
        {
            foreach (int i in UpdateDomainsEnumerable(state))
            {
                for (int j = 0; j < i; ++j)
                {
                    if ((_domains[i] & _domains[j]) != 0) return false;
                }
            }

            return true;
        }

        public bool CanBeMet(IState state)
        {
            UpdateDomains(state);

            var union = _domains.Aggregate(0ul, (u, d) => u |= d);

            return BitCounter.Count(union) >= N;
        }

        private IEnumerable<int> UpdateDomainsEnumerable(IState state)
        {
            for (int i = 0; i < _variables.Length; ++i)
            {
                _domains[i] = GetLeftShifted(state, i);
                yield return i;
            }
        }

        private void UpdateDomains(IState state)
        {
            for (int i = 0; i < _variables.Length; ++i)
            {
                _domains[i] = GetLeftShifted(state, i);
            }
        }

        private ulong GetLeftShifted(IState state, int i) =>
            _variables[i].GetDomain(state).domain << (_variables[i].Min - _minMin);

        private ulong RightShift(ulong domain, int i) =>
            domain >> (_variables[i].Min - _minMin);
    }
}
