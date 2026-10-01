using CSPSolver.common;
using CSPSolver.common.variables;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace CSPSolver.Constraint.AllDiff;

public readonly struct AllDiffSameIntDomain : IConstraint
{
    private readonly ISmallIntDomainVar[] _variables;
    private readonly uint[] _domains;
    private int N => _variables.Length;
    private ulong AllVariables => (1ul << N) - 1;

    public AllDiffSameIntDomain(IEnumerable<ISmallIntDomainVar> variables)
    {
        _variables = variables.ToArray();
        if (_variables.Length > 63) throw new System.NotSupportedException("AllDiff supports at most 63 variables.");
        _domains = new uint[_variables.Length];
    }

    public IEnumerable<IVariable> Variables => _variables;

    public IEnumerable<IVariable> NegativePropagate(IState state)
    {
        if (N == 0) return [];

        (int i1, int i2)? pair = null;

        for (int i = 0; i < N; i++)
        {
            _domains[i] = GetDomain(state, i);
            for (int j = 0; j < i; j++)
            {
                if ((_domains[i] & _domains[j]) != 0)
                {
                    if (pair.HasValue) return [];
                    pair = (i, j);
                }
            }
        }

        // No two variables can be equal, so the constraint is violated: emptying one variable is enough to fail.
        if (!pair.HasValue) return _variables[0].MakeEmpty(state) ? [_variables[0]] : [];

        // Exactly one pair can still be equal, so it must be.
        var (i1, i2) = pair.Value;
        var changed1 = _variables[i1].SetDomain(state, _domains[i2]);
        var changed2 = _variables[i2].SetDomain(state, _domains[i1]);

        return (changed1, changed2) switch
        {
            (true, true) => [_variables[i1], _variables[i2]],
            (true, false) => [_variables[i1]],
            (false, true) => [_variables[i2]],
            _ => []
        };
    }

    public IEnumerable<IVariable> Propagate(IState state)
    {
        UpdateDomains(state);
        List<IVariable> result = null;
        var changed = 0ul;

        // Bit i of a subset mask is variable i. If k variables share k or fewer values, no other variable can take them.
        for (var subset = 1ul; subset < AllVariables; subset++)
        {
            var union = 0u;
            for (var rest = subset; rest != 0; rest &= rest - 1)
            {
                union |= _domains[BitOperations.TrailingZeroCount(rest)];
            }

            if (BitOperations.PopCount(union) > BitOperations.PopCount(subset)) continue;

            for (int i = 0; i < N; i++)
            {
                var bit = 1ul << i;
                if ((subset & bit) == 0 && _variables[i].DomainMinus(state, union) && (changed & bit) == 0)
                {
                    changed |= bit;
                    (result ??= []).Add(_variables[i]);
                }
            }
        }

        return result ?? [];
    }

    public bool IsMet(IState state)
    {
        // A domain overlapping the union of the earlier ones overlaps at least one of them.
        var seen = 0u;
        for (int i = 0; i < N; i++)
        {
            var domain = GetDomain(state, i);
            if ((seen & domain) != 0) return false;
            seen |= domain;
        }

        return true;
    }

    public bool CanBeMet(IState state)
    {
        var union = 0u;
        for (int i = 0; i < N; i++)
        {
            union |= GetDomain(state, i);
        }

        return BitOperations.PopCount(union) >= N;
    }

    private void UpdateDomains(IState state)
    {
        for (int i = 0; i < _variables.Length; ++i)
        {
            _domains[i] = GetDomain(state, i);
        }
    }

    private uint GetDomain(IState state, int i) => _variables[i].GetDomain(state).domain;
}