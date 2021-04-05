using CSPSolver.common;
using CSPSolver.common.search;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CSPSolver.State
{
    public readonly struct IntState : IState
    {
        private readonly uint[] _data;

        public IntState(int size)
        {
            _data = new uint[size];
            for (int i = 0; i < size; i++) _data[i] = 0;
        }

        public IntState(uint[] data) => _data = data;

        public IntState Copy()
        {
            var copy = new uint[_data.Length];
            _data.CopyTo(copy, 0);
            return new IntState(copy);
        }

        IState IState.Copy() => Copy();

        public uint GetDomain(in IStateRef idx, in int size) => GetDomain((StateRef)idx , size);
        private uint GetDomain(in StateRef idx, in int size) => (_data[idx.Idx] >> idx.Offset) & ((uint)Math.Pow(2, size) - 1);

        public int GetDomainMax(in IStateRef idx, in int size) => (int)Math.Log2(GetDomain((StateRef)idx, size));

        public int GetLargeDomainMax(in IStateRef idx, in int size)
        {
            var domain = GetLargeDomain(idx, size);

            for (int i = domain.Length - 1; i >= 0; --i)
            {
                if (domain[i] != 0)
                {
                    return (int)Math.Log2(domain[i]) + 32 * i;
                }
            }

            throw new EmptyDomainException();
        }

        public int GetDomainMin(in IStateRef idx, in int size) => BitOperations.TrailingZeroCount(GetDomain((StateRef)idx, size));

        public int GetLargeDomainMin(in IStateRef idx, in int size)
        {
            var domain = GetLargeDomain(idx, size);

            for (int i = 0; i < domain.Length; i++)
            {
                if (domain[i] != 0)
                {
                    return BitOperations.TrailingZeroCount(domain[i]) + 32 * i;
                }
            }

            throw new EmptyDomainException();
        }

        public uint[] GetLargeDomain(in IStateRef idx, in int size)
        {
            var n = size / 32;
            var r = size % 32;

            var result = r == 0 ? new uint[n] : new uint[n + 1];

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

        public void SetDomain(in IStateRef idx, in int size, in uint value) => SetDomain((StateRef)idx, size, value);

        private  void SetDomain(in StateRef idx, in int size, in uint value)
        {
            _data[idx.Idx] = _data[idx.Idx] & ~(((uint)Math.Pow(2, size) - 1) << idx.Offset);
            _data[idx.Idx] = _data[idx.Idx] + (value << idx.Offset);
        }

        public void SetLargeDomain(in IStateRef idx, in int size, in uint[] value)
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

        public int GetInt(in IStateRef idx) => BitConverter.ToInt32(BitConverter.GetBytes(_data[((StateRef)idx).Idx]));

        public void SetInt(in IStateRef idx, in int value) => _data[((StateRef)idx).Idx] = BitConverter.ToUInt32(BitConverter.GetBytes(value));

        public long GetLong(in IStateRef idx) => ((long)_data[((StateRef)idx).Idx]) + ((long)_data[((StateRef)idx).Idx + 1] << 32);
        

        public void SetLong(in IStateRef idx, in long value)
        {
            _data[((StateRef)idx).Idx] = (uint)value;
            _data[((StateRef)idx).Idx + 1] = (uint)(value >> 32);
        }

        public double GetDouble(in IStateRef idx) => BitConverter.Int64BitsToDouble(GetLong(idx));

        public void SetDouble(in IStateRef idx, in double value) => SetLong(idx, BitConverter.DoubleToInt64Bits(value));

        public float GetFloat(in IStateRef idx) => BitConverter.Int32BitsToSingle(GetInt(idx));
        public void SetFloat(in IStateRef idx, in float value) => SetInt(idx, BitConverter.SingleToInt32Bits(value));
    }
}
