using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSPSolver.utils
{
    public class PowerSet<T> : IEnumerable<IList<T>>
    {
        private readonly IList<T> _list;

        private readonly int _count;

        private int N => _list.Count;

        public PowerSet(IList<T> list) => (_list, _count) = (list, 1 << list.Count);

        public IEnumerator<IList<T>> GetEnumerator()
        {
            return Enumerable.Range(0, _count)
                .Select(mask =>
                    Enumerable.Range(0, N)
                        .Where(i => (mask & (1 << i)) > 0)
                        .Select(i => _list[i])
                        .ToList())
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
