using CSPSolver.common;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPSolver.State
{
    public class StatePool: IStatePool
    {
        private readonly int _size;
        private readonly ArrayPool<uint> _arrayPool;
        public StatePool(int size)
        {
            _size = size;
            _arrayPool = ArrayPool<uint>.Create(_size, 10);
        }

        public IState Copy(IState state)
        {
            if (state is IntState intState)
            {
                var array = _arrayPool.Rent(_size);
                return intState.Copy(array);
            }

            throw new NotImplementedException();
        }

        public void Return(IState state)
        {
            if (state is IntState intState)
            {
                _arrayPool.Return(intState._data);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal IState Empty() => new IntState(_arrayPool.Rent(_size));
    }
}
