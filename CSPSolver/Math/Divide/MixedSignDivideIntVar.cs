using System;
using System.Collections.Generic;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Divide
{
    public readonly struct MixedSignDivideIntVar : IIntVar, ICompoundVariable
    {
        private readonly IIntVar _v1;
        private readonly IIntVar _v2;

        public int Min { get; }

        public int Size { get; }

        public int Max { get; }

        public MixedSignDivideIntVar(IIntVar v1, IIntVar v2)
        {
            _v1 = v1;
            _v2 = v2;
            (Min, Max) = Bounds(v1.Min, v1.Max, v2.Min, v2.Max);
            Size = Max - Min + 1;
        }

        public int GetDomainMax(IState state)
            => Bounds(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state)).max;

        public int GetDomainMin(IState state)
            => Bounds(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state)).min;

        /// <summary>
        /// The denominator is never zero, so it covers at most two sign constant
        /// ranges: [v2Min, -1] and [1, v2Max]. Over either one the quotient is
        /// monotonic in the numerator and in the magnitude of the denominator, so
        /// its extremes sit on the corners. A denominator holding only zero yields
        /// an inverted range, which reads as empty.
        /// </summary>
        private static (int min, int max) Bounds(int v1Min, int v1Max, int v2Min, int v2Max)
        {
            var min = int.MaxValue;
            var max = int.MinValue;

            if (v2Min <= -1) Corners(v1Min, v1Max, v2Min, Min(v2Max, -1), ref min, ref max);
            if (v2Max >= 1) Corners(v1Min, v1Max, Max(v2Min, 1), v2Max, ref min, ref max);

            return (min, max);
        }

        private static void Corners(int v1Min, int v1Max, int lo, int hi, ref int min, ref int max)
        {
            Visit(v1Min / lo, ref min, ref max);
            Visit(v1Min / hi, ref min, ref max);
            Visit(v1Max / lo, ref min, ref max);
            Visit(v1Max / hi, ref min, ref max);
        }

        private static void Visit(int quotient, ref int min, ref int max)
        {
            if (quotient < min) min = quotient;
            if (quotient > max) max = quotient;
        }

        public void Initialise(IState state) { /* holds no state */ }

        public bool IsEmpty(IState state) => _v1.IsEmpty(state) | _v2.IsEmpty(state);

        public bool IsInstantiated(IState state) => _v1.IsInstantiated(state) & _v2.IsInstantiated(state);

        public bool RemoveValue(IState state, object value) =>
            (_v2.TryGetValue(state, out int v2) && _v1.RemoveValue(state, (int)value * v2))
          | (_v1.TryGetValue(state, out int v1) && (int)value != 0 && _v2.RemoveValue(state, v1 / (int)value));

        public bool SetMax(IState state, int max) => Restrict(state, int.MinValue, max);

        public bool SetMin(IState state, int min) => Restrict(state, min, int.MaxValue);

        public bool SetValue(IState state, object value) => Restrict(state, (int)value, (int)value);

        /// <summary>
        /// Narrows the operands so the quotient falls within [lo, hi].
        /// <para>
        /// Deliberately sound rather than tight. Everything it removes is genuinely
        /// infeasible, but it leaves some infeasible values in place. Pruning too
        /// little costs search nodes; pruning too much loses solutions, which is the
        /// fault this class carried. #26 covers tightening it now that a completeness
        /// sweep guards the result.
        /// </para>
        /// </summary>
        private bool Restrict(IState state, int lo, int hi)
        {
            var result = ExcludeZeroDenominator(state);
            if (_v2.IsEmpty(state) || _v1.IsEmpty(state)) return result;

            var v1Min = _v1.GetDomainMin(state);
            var v1Max = _v1.GetDomainMax(state);
            var v2Min = _v2.GetDomainMin(state);
            var v2Max = _v2.GetDomainMax(state);

            // Only quotients the operands can actually produce are worth considering,
            // and clamping here keeps the unbounded end of SetMin and SetMax finite.
            var (qMin, qMax) = Bounds(v1Min, v1Max, v2Min, v2Max);
            if (lo < qMin) lo = qMin;
            if (hi > qMax) hi = qMax;

            if (lo > hi) return EmptyNumerator(state) | result;

            // x = q * y + r, where r carries the sign of x and |r| < |y|. So every
            // reachable numerator lies within the product range widened by the
            // largest denominator magnitude.
            var slack = Max(Abs((long)v2Min), Abs((long)v2Max)) - 1;
            var (productMin, productMax) = Products(lo, hi, v2Min, v2Max);

            return _v1.SetMin(state, Saturate(productMin - slack))
                 | _v1.SetMax(state, Saturate(productMax + slack))
                 | result;
        }

        /// <summary>
        /// Zero is never a legal denominator. A bounds interface can only remove it
        /// at an endpoint, which is enough: a denominator narrowing towards zero
        /// reaches an endpoint of zero before search can settle on it.
        /// </summary>
        private bool ExcludeZeroDenominator(IState state)
        {
            var result = false;

            if (_v2.GetDomainMin(state) == 0) result |= _v2.SetMin(state, 1);
            if (_v2.GetDomainMax(state) == 0) result |= _v2.SetMax(state, -1);

            return result;
        }

        private bool EmptyNumerator(IState state) => _v1.SetMax(state, Saturate((long)_v1.Min - 1));

        private static (long min, long max) Products(int qLo, int qHi, int v2Min, int v2Max)
        {
            var min = long.MaxValue;
            var max = long.MinValue;

            if (v2Min <= -1) ProductCorners(qLo, qHi, v2Min, Min(v2Max, -1), ref min, ref max);
            if (v2Max >= 1) ProductCorners(qLo, qHi, Max(v2Min, 1), v2Max, ref min, ref max);

            return (min, max);
        }

        private static void ProductCorners(long qLo, long qHi, long lo, long hi, ref long min, ref long max)
        {
            Visit(qLo * lo, ref min, ref max);
            Visit(qLo * hi, ref min, ref max);
            Visit(qHi * lo, ref min, ref max);
            Visit(qHi * hi, ref min, ref max);
        }

        private static void Visit(long product, ref long min, ref long max)
        {
            if (product < min) min = product;
            if (product > max) max = product;
        }

        private static int Saturate(long value) =>
            value < int.MinValue 
                ? int.MinValue
                : value > int.MaxValue 
                    ? int.MaxValue
                    : (int)value;

        public bool TryGetValue(IState state, out int value)
        {
            if (_v1.TryGetValue(state, out int v1) & _v2.TryGetValue(state, out int v2))
            {
                value = v1 / v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(int);

        public string PrettyDomain(IState state) => $"{_v1.PrettyDomain(state)} / {_v2.PrettyDomain(state)}";

        public IEnumerable<IVariable> GetChildren() => [_v1, _v2];
    }
}
