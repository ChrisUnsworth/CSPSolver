# Performance Tests

[BenchmarkDotNet](https://benchmarkdotnet.org/) benchmarks for CSPSolver. It appears in `CSPSolver.sln` under the
`Performance` folder for editing, but is excluded from that solution's build, so it does not build or run in CI.
`PerformanceTests.sln` can also be opened on its own.

Run everything (always use Release):

```sh
dotnet run -c Release --project PerformanceTests -- --filter '*'
```

Run a subset, or do a quick smoke run that executes each benchmark once:

```sh
dotnet run -c Release --project PerformanceTests -- --filter '*Sudoku*'
dotnet run -c Release --project PerformanceTests -- --filter '*' --job dry
```

Results are written to `BenchmarkDotNet.Artifacts/`.

| Benchmark | What it exercises |
|-----------|-------------------|
| `NQueensBenchmarks` | Binary not-equal propagation; first solution and full enumeration for N = 6, 8, 10 |
| `SudokuBenchmarks` | AllDiff propagation on an easy and a hard puzzle, including proving uniqueness |
| `SendMoreMoneyBenchmarks` | AllDiff combined with a wide linear equation |
| `OptimisationBenchmarks` | Objective-driven search on a small bounded knapsack |
