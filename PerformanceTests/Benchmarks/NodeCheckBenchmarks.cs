using BenchmarkDotNet.Attributes;

using CSPSolver.common;
using CSPSolver.Model;
using CSPSolver.State;

namespace PerformanceTests.Benchmarks;

/// <summary>
/// The per-node check Search.Solve makes after propagating, isolated from search and propagation costs.
/// SudokuEasy is solved by propagation alone, so it measures the solution case; the others are undecided.
/// </summary>
[MemoryDiagnoser]
public class NodeCheckBenchmarks
{
    [Params("SudokuHard", "SudokuEasy", "Queens10")]
    public string Problem { get; set; }

    private IModel _model;
    private IState _state;

    [GlobalSetup]
    public void Setup()
    {
        var mb = Problem switch
        {
            "SudokuHard" => SudokuBenchmarks.BuildModel("Hard"),
            "SudokuEasy" => SudokuBenchmarks.BuildModel("Easy"),
            _ => NQueensBenchmarks.BuildModel(10),
        };

        _model = mb.GetModel();
        _state = new StatePool(mb.GetStateSize()).Empty();
        _model.Initialise(_state);
        _model.Propagate(_state);
    }

    [Benchmark]
    public DomainState Check() => _model.GetDomainState(_state);
}
