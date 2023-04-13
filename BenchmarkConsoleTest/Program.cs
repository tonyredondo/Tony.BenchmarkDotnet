using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Tony.BenchmarkDotnet.Jetbrains;

BenchmarkRunner.Run<BenchmarkTest>(DefaultConfig.Instance
    .WithJetbrains(JetbrainsProduct.Memory));

public class BenchmarkTest
{
    [Benchmark]
    public string CreateString()
    {
        return Guid.NewGuid().ToString();
    }
}