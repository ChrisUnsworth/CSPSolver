using System;
using System.Collections.Generic;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Divide
{
    public readonly struct MixedSignDivideRealVar : IRealVar, ICompoundVariable
    {
        private readonly IRealVar _v1;
        private readonly IRealVar _v2;

        public double Min { get; }

        public double Max { get; }

        public double Epsilon => Min(_v1.Epsilon, _v2.Epsilon);

        public MixedSignDivideRealVar(IRealVar v1, IRealVar v2)
        {
            _v1 = v1;
            _v2 = v2;
            var epsilon = Min(v1.Epsilon, v2.Epsilon);
            (Min, Max) = Bounds(v1.Min, v1.Max, v2.Min, v2.Max, epsilon);
        }

        public double GetDomainMax(IState state)
            => Bounds(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state), Epsilon).max;

        public double GetDomainMin(IState state)
            => Bounds(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state), Epsilon).min;

        /// <summary>
        /// The denominator is never within epsilon of zero, so it covers at most two
        /// sign ranges: [v2Min, -epsilon] and [epsilon, v2Max]. Over either one the
        /// quotient is monotonic in the numerator and in the denominator, so its
        /// extremes sit on the corners. Both ranges absent yields an inverted range,
        /// which reads as empty.
        /// </summary>
        private static (double min, double max) Bounds(double v1Min, double v1Max, double v2Min, double v2Max, double epsilon)
        {
            var min = double.MaxValue;
            var max = double.MinValue;

            if (v2Min <= -epsilon) Corners(v1Min, v1Max, v2Min, Min(v2Max, -epsilon), ref min, ref max);
            if (v2Max >= epsilon) Corners(v1Min, v1Max, Max(v2Min, epsilon), v2Max, ref min, ref max);

            return (min, max);
        }

        private static void Corners(double v1Min, double v1Max, double lo, double hi, ref double min, ref double max)
        {
            Visit(v1Min / lo, ref min, ref max);
            Visit(v1Min / hi, ref min, ref max);
            Visit(v1Max / lo, ref min, ref max);
            Visit(v1Max / hi, ref min, ref max);
        }

        private static void Visit(double value, ref double min, ref double max)
        {
            if (value < min) min = value;
            if (value > max) max = value;
        }

        public void Initialise(IState state) { /* holds no state */ }

        public bool IsEmpty(IState state) => _v1.IsEmpty(state) | _v2.IsEmpty(state);

        public bool IsInstantiated(IState state) => _v1.IsInstantiated(state) & _v2.IsInstantiated(state);

        public bool RemoveValue(IState state, object value) =>
            (_v2.TryGetValue(state, out double v2) && _v1.RemoveValue(state, (double)value * v2))
          | (_v1.TryGetValue(state, out double v1) && (double)value != 0 && _v2.RemoveValue(state, v1 / (double)value));

        public bool SetMax(IState state, double max) => Restrict(state, double.MinValue, max);

        public bool SetMin(IState state, double min) => Restrict(state, min, double.MaxValue);

        public bool SetValue(IState state, object value) => Restrict(state, (double)value, (double)value);

        /// <summary>
        /// Narrows both operands so the quotient falls within [lo, hi]. Real division
        /// has no truncation remainder, so unlike the integer case this is exact
        /// rather than sound-but-loose: x = q * y and y = x / q hold precisely, so
        /// both directions can be corner-evaluated with no slack widening.
        /// </summary>
        private bool Restrict(IState state, double lo, double hi)
        {
            var result = ExcludeZeroDenominator(state);
            if (_v2.IsEmpty(state) || _v1.IsEmpty(state)) return result;

            var v1Min = _v1.GetDomainMin(state);
            var v1Max = _v1.GetDomainMax(state);
            var v2Min = _v2.GetDomainMin(state);
            var v2Max = _v2.GetDomainMax(state);

            // Only quotients the operands can actually produce are worth considering.
            var (qMin, qMax) = Bounds(v1Min, v1Max, v2Min, v2Max, Epsilon);
            if (lo < qMin) lo = qMin;
            if (hi > qMax) hi = qMax;

            if (lo > hi) return EmptyNumerator(state) | result;

            // x = q * y exactly, so the reachable numerator range is the corner
            // product of the quotient window and the denominator's own range.
            var (productMin, productMax) = Products(lo, hi, v2Min, v2Max, Epsilon);
            result |= _v1.SetMin(state, productMin);
            result |= _v1.SetMax(state, productMax);

            if (_v1.IsEmpty(state)) return result;

            // y = x / q exactly, the same relation as the forward bounds with the
            // quotient window standing in for the denominator. This only holds while
            // x and q can't both be zero at once -- there, x = 0 = q * y is satisfied
            // by every y, so the constraint says nothing about the denominator and
            // narrowing it would silently drop otherwise-valid solutions.
            var v1MinAfter = _v1.GetDomainMin(state);
            var v1MaxAfter = _v1.GetDomainMax(state);
            var xCanBeZero = v1MinAfter <= 0 && v1MaxAfter >= 0;
            var qCanBeZero = lo <= 0 && hi >= 0;

            if (!(xCanBeZero && qCanBeZero))
            {
                var (v2New1, v2New2) = Bounds(v1MinAfter, v1MaxAfter, lo, hi, Epsilon);
                result |= _v2.SetMin(state, v2New1);
                result |= _v2.SetMax(state, v2New2);
            }

            return result;
        }

        /// <summary>
        /// Zero is never a legal denominator. A bounds interface can only remove it
        /// at an endpoint, which is enough: a denominator narrowing towards zero
        /// reaches an endpoint within epsilon of zero before search can settle on it.
        /// </summary>
        private bool ExcludeZeroDenominator(IState state)
        {
            var result = false;

            if (_v2.GetDomainMin(state) > -Epsilon && _v2.GetDomainMin(state) < Epsilon) result |= _v2.SetMin(state, Epsilon);
            if (_v2.GetDomainMax(state) > -Epsilon && _v2.GetDomainMax(state) < Epsilon) result |= _v2.SetMax(state, -Epsilon);

            return result;
        }

        private bool EmptyNumerator(IState state) => _v1.SetMax(state, _v1.GetDomainMin(state) - Epsilon);

        private static (double min, double max) Products(double qLo, double qHi, double v2Min, double v2Max, double epsilon)
        {
            var min = double.MaxValue;
            var max = double.MinValue;

            if (v2Min <= -epsilon) ProductCorners(qLo, qHi, v2Min, Min(v2Max, -epsilon), ref min, ref max);
            if (v2Max >= epsilon) ProductCorners(qLo, qHi, Max(v2Min, epsilon), v2Max, ref min, ref max);

            return (min, max);
        }

        private static void ProductCorners(double qLo, double qHi, double lo, double hi, ref double min, ref double max)
        {
            Visit(qLo * lo, ref min, ref max);
            Visit(qLo * hi, ref min, ref max);
            Visit(qHi * lo, ref min, ref max);
            Visit(qHi * hi, ref min, ref max);
        }

        public bool TryGetValue(IState state, out double value)
        {
            if (_v1.TryGetValue(state, out double v1) & _v2.TryGetValue(state, out double v2))
            {
                value = v1 / v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(double);

        public string PrettyDomain(IState state) => $"{_v1.PrettyDomain(state)} / {_v2.PrettyDomain(state)}";

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
