using CSPSolver.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPSolver.State
{
    public class StateBuilder : IStateBuilder
    {
        private int _size;
        private List<int> _map;

        public StateBuilder() 
        {
            _size = 0;
            _map = new List<int>();
        }

        public IStateRef AddDomain(int domainSize) => domainSize < 32 ? AddSmallDomain(domainSize) : AddLargeDomain(domainSize);

        private IStateRef AddSmallDomain(int domainSize)
        {
            var x = _map.Select((v, i) => ((int v, int i)?)(v, i)).FirstOrDefault(x => 32 - x.Value.v >= domainSize);

            if (x.HasValue)
            {
                var stateRef = new StateRef(x.Value.i, x.Value.v);
                _map[x.Value.i] += domainSize;
                return stateRef;
            }
            else
            {
                _map.Add(domainSize);
                return new StateRef(_size++, 0);
            }
        }

        private IStateRef AddLargeDomain(int domainSize)
        {
            var n = domainSize / 32;
            var r = domainSize % 32;
            var stateRef = new StateRef(_size, 0);

            _size += n;
            _map.AddRange(Enumerable.Repeat(32, n));
            if (domainSize % 32 > 0)
            {
                _map.Add(r);
                _size++;
            }

            return stateRef;
        }

        public IStateRef AddDouble()
        {
            _map.AddRange(new int[] { 32, 32 });
            return new StateRef(_size++, 0);
        }

        public IStateRef AddFloat()
        {
            _map.Add(32);
            return new StateRef(_size++, 0);
        }

        public IStateRef AddInt()
        {
            _map.Add(32);
            return new StateRef(_size++, 0);
        }

        public IStateRef AddLong()
        {
            _map.AddRange(new int[] { 32, 32 });
            return new StateRef(_size++, 0);
        }

        public IState GetState() => new IntState(_size);

    }
}
