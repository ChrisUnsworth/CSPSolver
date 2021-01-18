using CSPSolver.common;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CSPSolver.State
{
    public class IntState : IState
    {
        private int[] _data;

        public IntState(int size)
        {
            _data = new int[size];
            for (int i = 0; i < size; i++) _data[i] = 0;
        }

        public IntState(int[] data) => _data = data;

        public IntState Copy()
        {
            var copy = new int[_data.Length];
            _data.CopyTo(copy, 0);
            return new IntState(copy);
        }

        IState IState.Copy() => Copy();

        public int GetDomain(IStateRef idx, int size) => GetDomain((StateRef)idx , size);
        private int GetDomain(StateRef idx, int size) => (_data[idx.Idx] >> idx.Offset) & ( (int)Math.Pow(2, size) - 1);

        public int GetDomainMax(IStateRef idx, int size) => (int)Math.Log2(GetDomain((StateRef)idx, size));

        public int GetDomainMin(IStateRef idx, int size) => BitOperations.TrailingZeroCount(GetDomain((StateRef)idx, size));

        public int[] GetLargeDomain(IStateRef idx, int size)
        {
            var n = size / 32;
            var r = size % 32;

            var result = r == 0 ? new int[n] : new int[n + 1];

            if (((StateRef)idx).Offset == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    result[i] = _data[((StateRef)idx).Idx + i];
                }

                if (r != 0)
                {
                    result[n] = GetDomain(new StateRef(((StateRef)idx).Idx + n, 0), r);
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return result;
        }

        public void SetDomain(IStateRef idx, int size, int value) => SetDomain((StateRef)idx, size, value);

        private  void SetDomain(StateRef idx, int size, int value)
        {
            _data[idx.Idx] = _data[idx.Idx] & ~(((int)Math.Pow(2, size) - 1) << idx.Offset);
            _data[idx.Idx] = _data[idx.Idx] + (value << idx.Offset);
        }

        public void SetLargeDomain(IStateRef idx, int size, int[] value)
        {
            var n = size / 32;
            var r = size % 32;

            if (((StateRef)idx).Offset == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    _data[((StateRef)idx).Idx + i] = value[i];
                }

                if (r != 0)
                {
                    SetDomain(new StateRef(((StateRef)idx).Idx + n, 0), r, value[n]);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public int GetInt(IStateRef idx) => _data[((StateRef)idx).Idx];

        public void SetInt(IStateRef idx, int value) => _data[((StateRef)idx).Idx] = value;

        public long GetLong(IStateRef idx) => ((long)_data[((StateRef)idx).Idx]) + ((long)_data[((StateRef)idx).Idx + 1] << 32);
        

        public void SetLong(IStateRef idx, long value)
        {
            _data[((StateRef)idx).Idx] = (int)value;
            _data[((StateRef)idx).Idx + 1] = (int)(value >> 32);
        }

        public double GetDouble(IStateRef idx) => BitConverter.Int64BitsToDouble(GetLong(idx));

        public void SetDouble(IStateRef idx, double value) => SetLong(idx, BitConverter.DoubleToInt64Bits(value));

        public float GetFloat(IStateRef idx) => BitConverter.Int32BitsToSingle(GetInt(idx));
        public void SetFloat(IStateRef idx, float value) => SetInt(idx, BitConverter.SingleToInt32Bits(value));
    }
}
