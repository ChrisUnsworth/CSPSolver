namespace CSPSolverTests.Solve.Queens;

/// <summary>
/// Concrete Queens boards. Each instance's region layout and expected solution
/// are generated and checked for uniqueness independently of this solver before
/// being added here.
/// </summary>
public static class Instances
{
    public record Instance(string[] Regions, int[] Solution);

    public static readonly Instance SevenBySeven = new(
        Regions:
        [
            "AAABBBB",
            "AAABBBB",
            "CACCBBB",
            "CCCCDBD",
            "ECCDDDD",
            "EEGDDDF",
            "EGGGGGF",
        ],
        Solution: [1, 5, 2, 4, 0, 6, 3]);
}