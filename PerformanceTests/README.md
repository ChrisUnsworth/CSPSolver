# Performance Tests

[BenchmarkDotNet](https://benchmarkdotnet.org/) benchmarks for CSPSolver. This project has its own
solution (`PerformanceTests.sln`) so it is not built or run as part of the main solution or CI.

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
