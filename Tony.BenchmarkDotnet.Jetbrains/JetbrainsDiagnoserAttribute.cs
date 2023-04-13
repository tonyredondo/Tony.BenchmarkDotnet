using BenchmarkDotNet.Configs;

namespace Tony.BenchmarkDotnet.Jetbrains;

/// <summary>
/// Jetbrains diagnoser attribute
/// </summary>
public class JetbrainsDiagnoserAttribute : Attribute, IConfigSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JetbrainsDiagnoserAttribute"/> class.
    /// </summary>
    public JetbrainsDiagnoserAttribute(JetbrainsProduct product, string? outputFolder = null)
    {
        Config = ManualConfig.CreateEmpty()
            .WithJetbrains(product, outputFolder);
    }

    /// <summary>
    /// Gets the configuration
    /// </summary>
    public IConfig Config { get; }
}