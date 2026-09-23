namespace CSPSolver.common.variables;

// Values match BoolVar's 2-bit domain encoding (bit0 = false possible, bit1 =
// true possible) so BoolVar.GetState can cast the raw domain instead of
// switching on it. Keep in sync with that encoding if it ever changes --
// BoolVarTests.GetStateMatchesTheIndependentIsAndCanBeChecks cross-checks
// GetState against the independent IsTrue/IsFalse/IsEmpty methods and will
// fail if the two drift apart.
public enum BoolState
{
    Empty = 0,
    False = 1,
    True = 2,
    Undecided = 3,
}
