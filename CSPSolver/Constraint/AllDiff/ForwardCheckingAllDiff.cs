using System;
using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.AllDiff
{
    public readonly struct ForwardCheckingAllDiff : IConstraint
    {
        private readonly IIntVar[] _variables;
        private int N => _variables.Length;

        private readonly int[] _max;
        private readonly int[] _min;
        private readonly bool[] _flag;

        public ForwardCheckingAllDiff(IEnumerable<IIntVar> variables)
        {
            _variables = variables.ToArray();
            _max = new int[_variables.Length];
            _min = new int[_variables.Length];
            _flag = new bool[_variables.Length];
        }

        public IEnumerable<IVariable> Variables => _variables;

        public IEnumerable<IVariable> Propagate(IState state)
        {
            Array.Clear(_flag, 0, _flag.Length);

            foreach (var i in UpdateMinMax(state))
            {
                if (_min[i] == _max[i])
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j && _variables[j].RemoveValue(state, _min[i]))
                        {
                            _flag[j] = true;
                        }
                    }
                }
            }

            for (int j = 0; j < N; j++)
            {
                if (_flag[j]) yield return _variables[j];
            }
        }

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            (int i1, int i2)? pair = null;

            foreach (var i in UpdateMinMax(state))
            {
                for (int j = 0; j < i; j++)
                {
                    if ((_min[i] <= _max[j]) && (_max[i] >= _min[j]))
                    {
                        if (pair.HasValue) yield break;
                        pair = (i, j);
                    }
                }
            }

            if (pair.HasValue)
            {
                if (_variables[pair.Value.i1].SetMax(state, _max[pair.Value.i2])
                  | _variables[pair.Value.i1].SetMin(state, _min[pair.Value.i2])) yield return _variables[pair.Value.i1];
                if (_variables[pair.Value.i2].SetMax(state, _max[pair.Value.i1])
                  | _variables[pair.Value.i2].SetMin(state, _min[pair.Value.i1])) yield return _variables[pair.Value.i2];
            }
            else
            {
                for (int i = 0; i < N; i++)
                {
                    if (_variables[i].SetMax(state, _min[i] - 1)) yield return _variables[i];
                }
            }
        }

        public bool IsMet(IState state)
        {
            foreach (var i in UpdateMinMax(state))
            {
                for (int j = 0; j < i; j++)
                {
                    if (!(_min[i] > _max[j] || _max[i] < _min[j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanBeMet(IState state)
        {
            foreach (var i in UpdateMinMax(state))
            {
                if (_min[i] == _max[i])
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (!(_min[i] > _min[j] || _max[i] < _max[j]))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private IEnumerable<int> UpdateMinMax(IState state)
        {
            for (int i = 0; i < N; i++)
            {
                _max[i] = _variables[i].GetDomainMax(state);
                _min[i] = _variables[i].GetDomainMin(state);
                yield return i;
            }
        }
    }
}
