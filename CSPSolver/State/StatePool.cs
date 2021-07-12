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
            _arrayPool = ArrayPool<uint>.Create();
            _size = size;
        }

        public StatePool(IState state)
        {
            if (state is IntState intState)
            {
                _arrayPool = ArrayPool<uint>.Create();
                _size = intState._data.Length;
            }
            else
            {
                throw new NotImplementedException();
            }
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

            throw new NotImplementedException();
        }
    }
}
