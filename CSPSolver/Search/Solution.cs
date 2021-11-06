using CSPSolver.common;
using CSPSolver.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Search
{
    public class Solution : ISolution
    {
        private readonly IState _state;

        public Solution(IState state) => _state = state;

        public int GetInt(ModelIntVar v) => GetInt(v.Variable);

        public int GetInt(IVariable<int> v) => v.TryGetValue(_state, out int value) ? value : throw new ArgumentException("Unable to get value for variable");

        public T GetValue<T>(IVariable<T> v)
        {
            if (v.TryGetValue(_state, out T value)) return value;

            throw new ApplicationException("Value not found for given variable.");
        }

        public IList<T> GetValues<T>(IList<IVariable<T>> v)
        {
            throw new NotImplementedException();
        }
    }
}
