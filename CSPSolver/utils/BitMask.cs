namespace CSPSolver.utils;

/// <summary>
/// Low bit masks, used to select the live portion of a packed domain.
/// </summary>
public static class BitMask
{
    /// <summary>The low <paramref name="bits"/> bits set, for 0 to 32.</summary>
    /// <remarks>
    /// Shifting a uint by 32 wraps to a shift of 0 rather than clearing it, so the
    /// full width mask is returned directly.
    /// </remarks>
    public static uint Small(int bits) => bits >= 32 ? uint.MaxValue : (1u << bits) - 1;

    /// <summary>The low <paramref name="bits"/> bits set, for 0 to 64.</summary>
    /// <remarks>
    /// Shifting a ulong by 64 wraps to a shift of 0 rather than clearing it, so the
    /// full width mask is returned directly.
    /// </remarks>
    public static ulong Long(int bits) => bits >= 64 ? ulong.MaxValue : (1ul << bits) - 1;
}