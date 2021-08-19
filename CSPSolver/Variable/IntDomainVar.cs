using System;
using System.Linq;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.State;
using System.Numerics;

namespace CSPSolver.Variable
{
    public readonly struct IntDomainVar: IIntDomainVar
    {
        public IStateRef StateRef { get; }
        public int Min { get; }
        public int Size { get; }

        public int Max => Min + Size - 1;

        public IntDomainVar(int min, int size, IStateRef stateRef)
        {
            StateRef = stateRef;
            Min = min;
            Size = size;
        }

        public (uint[] domain, int min, int size) GetDomain(IState state) => (state.GetLargeDomain(StateRef, Size), Min, Size);

        public bool SetDomain(IState state, uint[] domain)
        {
            if (!domain.SequenceEqual(state.GetLargeDomain(StateRef, Size)))
            {
                state.SetLargeDomain(StateRef, Size, domain);
                return true;
            }

            return false;
        }

        public bool DomainMinus(IState state, uint[] domain)
        {
            var oldDomain = state.GetLargeDomain(StateRef, Size);
            var isDifferent = false;

            for (int i = 0; i < domain.Length; i++)
            {
                domain[i] = oldDomain[i] & ~domain[i];
                isDifferent = isDifferent || domain[i] != oldDomain[i];
            }

            if (isDifferent)
            {
                state.SetLargeDomain(StateRef, Size, domain);
            }

            return isDifferent;
        }

        public IEnumerable<int> EnumerateDomain(IState state)
        {
            var r = Size % 32;
            var domain = state.GetLargeDomain(StateRef, Size);

            for (int i = 0; i < domain.Length; i++)
            {
                uint mask = 0b_1;
                int s = r != 0 && i == (domain.Length - 1) ? r : 32;

                foreach (var j in Enumerable.Range(0, s))
                {
                    if ((mask & domain[i]) == mask)
                    {
                        yield return j + Min + i * 32;
                    }

                    mask <<= 1;
                }
            }

        }

        public int GetDomainMax(IState state) => state.GetLargeDomainMax(StateRef, Size) + Min;

        public int GetDomainMin(IState state) => state.GetLargeDomainMin(StateRef, Size) + Min;

        public bool SetMax(IState state, int max)
        {
            if (max >= Max) return false;
            int intVal = max - Min;
            int i = intVal < 0 ? -1 : intVal / 32;
            int r = intVal % 32;
            uint mask = (1u << (r + 1)) - 1;
            var isDifferent = false;
            var domain = state.GetLargeDomain(StateRef, Size);

            for (int j = domain.Length - 1; j >= 0; --j)
            {
                if (i == j)
                {
                    var newDom = domain[j] & mask;
                    isDifferent |= newDom != domain[j];
                    domain[j] = newDom;
                    break;
                }

                isDifferent |= 0 != domain[j];
                domain[j] = 0;
            }

            state.SetLargeDomain(StateRef, Size, domain);
            return isDifferent;
        }

        public bool SetMin(IState state, int min)
        {
            if (min <= Min) return false;
            int intVal = min - Min;
            int i = intVal < 0 ? -1 : intVal / 32;
            int r = intVal % 32;
            uint mask = ~((1u << r) - 1);
            var isDifferent = false;
            var domain = state.GetLargeDomain(StateRef, Size);

            for (int j = 0; j < domain.Length; j++)
            {
                if (i == j)
                {
                    var newDom = domain[j] & mask;
                    isDifferent |= newDom != domain[j];
                    domain[j] = newDom;
                    break;
                }

                isDifferent |= 0 != domain[j];
                domain[j] = 0;
            }

            state.SetLargeDomain(StateRef, Size, domain);
            return isDifferent;
        }

        public bool TryGetValue(IState state, out int value)
        {
            value = GetDomainMin(state);
            return value == GetDomainMax(state);
        }

        public void Initialise(IState state)
        {
            var domain = state.GetLargeDomain(StateRef, Size);

            for (int i = 0; i < domain.Length; i++)
            {
                domain[i] = 0b_1111_1111_1111_1111_1111_1111_1111_1111;
            }

            state.SetLargeDomain(StateRef, Size, domain);
        }

        public bool IsInstantiated(IState state)
        {
            int i = 0;
            var domain = state.GetLargeDomain(StateRef, Size);

            for (; i <= domain.Length; i++)
            {
                if (i == domain.Length) return false;
                if (domain[i] != 0)
                {
                    if ((domain[i] & (domain[i] - 1)) == 0) break;
                    return false;
                }
            }

            for (; i < domain.Length; i++)
            {
                if (domain[i] != 0) return false;
            }

            return true;
        }

        public bool IsEmpty(IState state)
        {
            foreach (var subdom in state.GetLargeDomain(StateRef, Size))
            {
                if (subdom != 0) return false;
            }

            return true;
        }

        public Type VariableType() => typeof(int);

        public bool SetValue(IState state, object value)
        {
            int intVal = (int)value - Min;

            int i = intVal < 0 ? -1 : intVal / 32;
            int r = intVal % 32;
            uint domVal = (uint)1 << r; 
            var isDifferent = false;

            var domain = state.GetLargeDomain(StateRef, Size);

            for (int j = 0; j < domain.Length; j++)
            {
                if (i == j)
                {
                    isDifferent |= domain[i] == domVal;
                    domain[i] = domVal;
                }
                else
                {
                    isDifferent |= domain[i] == 0;
                    domain[i] = 0;
                }
            }

            state.SetLargeDomain(StateRef, Size, domain);

            return isDifferent;
        }

        public bool RemoveValue(IState state, object value)
        {
            if ((int)value < Min || (int)value > Max) return false;
            var intVal = (int)value - Min;
            int i = intVal / 32;
            int r = intVal % 32;
            uint mask = (uint)0b_1 << r;

            var domain = state.GetLargeDomain(StateRef, Size);

            if ((domain[i] & mask) == mask)
            {
                domain[i] &= ~mask;
                state.SetLargeDomain(StateRef, Size, domain);
                return true;
            }

            return false;
        }


        public string PrettyDomain(IState state) => $"{{ {string.Join(", ", EnumerateDomain(state))} }}";
    }
}
