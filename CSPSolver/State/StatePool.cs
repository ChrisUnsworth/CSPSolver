using CSPSolver.common;
using System;
using System.Buffers;

namespace CSPSolver.State;

public class StatePool : IStatePool
{
    private readonly int _size;
    private readonly ArrayPool<uint> _arrayPool;
    public StatePool(int size)
    {
        _size = size;
        // maxArrayLength must be positive even though Rent(0) itself is fine -- a
        // model with no state at all (e.g. built entirely from constants) would
        // otherwise fail to even construct a pool.
        _arrayPool = ArrayPool<uint>.Create(System.Math.Max(_size, 1), 10);
    }

    public IState Copy(IState state)
    {
        switch (state)
        {
            case IntState intState:
                var array = _arrayPool.Rent(_size);
                return intState.Copy(array);
            default:
                throw new NotImplementedException();
        }
    }

    public void Return(IState state)
    {
        switch (state)
        {
            case IntState intState:
                _arrayPool.Return(intState._data);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public IState Empty() => new IntState(_arrayPool.Rent(_size));
}