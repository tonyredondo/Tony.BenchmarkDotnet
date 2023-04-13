using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using JetBrains.Profiler.SelfApi;

namespace Tony.BenchmarkDotnet.Jetbrains;

/// <summary>
/// Jetbrains diagnoser
/// </summary>
internal class JetbrainsDiagnoser : IDiagnoser
{
    public IEnumerable<string> Ids { get; } = new[] { "Jetbrains" };
    public IEnumerable<IExporter> Exporters { get; } = Array.Empty<IExporter>();
    public IEnumerable<IAnalyser> Analysers { get; } = Array.Empty<IAnalyser>();

    private readonly JetbrainsProduct _product;
    private string _filePath;
    private string? _outputFolder;

    public JetbrainsDiagnoser(JetbrainsProduct product, string? outputFolder = null)
    {
        _product = product;
        _filePath = string.Empty;
        _outputFolder = outputFolder;
        switch (_product)
        {
            case JetbrainsProduct.Memory:
                DotMemory.EnsurePrerequisite();
                break;
            case JetbrainsProduct.Trace:
                DotTrace.EnsurePrerequisite();
                break;
        }
    }

    public RunMode GetRunMode(BenchmarkCase benchmarkCase)
    {
        return RunMode.ExtraRun;
    }

    public bool RequiresBlockingAcknowledgments(BenchmarkCase benchmarkCase)
    {
        return false;
    }

    public void Handle(HostSignal signal, DiagnoserActionParameters parameters)
    {
        if (signal == HostSignal.BeforeActualRun)
        {
            var outputFolder = _outputFolder ?? Environment.CurrentDirectory;
            switch (_product)
            {
                case JetbrainsProduct.Memory:
                    DotMemory.Attach(new DotMemory.Config().SaveToDir(outputFolder));
                    DotMemory.GetSnapshot("Start");
                    break;
                case JetbrainsProduct.Trace:
                    DotTrace.Attach(new DotTrace.Config().SaveToDir(outputFolder));
                    DotTrace.StartCollectingData();
                    break;
            }
        }
        else if (signal == HostSignal.AfterActualRun)
        {
            switch (_product)
            {
                case JetbrainsProduct.Memory:
                    DotMemory.GetSnapshot("End");
                    _filePath = DotMemory.Detach();
                    break;
                case JetbrainsProduct.Trace:
                    DotTrace.StopCollectingData();
                    DotTrace.SaveData();
                    _filePath = DotTrace.GetCollectedSnapshotFilesArchive(true);
                    break;
            }
        }
    }

    public IEnumerable<Metric> ProcessResults(DiagnoserResults results)
    {
        return Enumerable.Empty<Metric>();
    }

    public void DisplayResults(ILogger logger)
    {
        logger.WriteLine(LogKind.Statistic, $"Jetbrains workspace filepath: {_filePath}");
    }

    public IEnumerable<ValidationError> Validate(ValidationParameters validationParameters)
    {
        return Enumerable.Empty<ValidationError>();
    }
}