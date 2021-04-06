using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPSolver.utils
{
    public static class BitCounter
    {
        private static readonly int[] _lookup = Enumerable.Range(0, byte.MaxValue + 1).Select(CountBits).ToArray();

        public static int Count(uint value)
        {
            if (value < _lookup.Length) return _lookup[(int)value];

            return BitConverter.GetBytes(value).Sum(b => _lookup[b]);
        }

        public static int Count(int value)
        {
            if (value < _lookup.Length) return _lookup[value];

            return BitConverter.GetBytes(value).Sum(b => _lookup[b]);
        }

        public static int CountBits(int value)
        {
            int count = 0;
            while (value != 0)
            {
                count++;
                value &= value - 1;
            }
            return count;
        }

        public static IEnumerable<List<T>> PowerSet<T>(IList<T> list, int min, int max)
        {
            var count = 1 << list.Count;
            return Enumerable.Range(0, count)
                .Where(mask => { var count = Count(mask); return count >= min && count <= max; } )
                .Select(mask =>
                    Enumerable.Range(0, list.Count)
                        .Where(i => (mask & (1 << i)) > 0)
                        .Select(i => list[i])
                        .ToList());
        }
    }
}
