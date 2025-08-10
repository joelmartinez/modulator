namespace CodeCube.Modulator;

/// <summary>
/// A modulated source that combines a base source with a modulator.
/// The modulator's output is added to the base source's output.
/// </summary>
public class ModulatedSource : IModulationSource
{
    private readonly IModulationSource _baseSource;
    private readonly IModulationSource _modulator;

    /// <summary>
    /// Initializes a new instance of the ModulatedSource class.
    /// </summary>
    /// <param name="baseSource">The base modulation source.</param>
    /// <param name="modulator">The modulator to apply to the base source.</param>
    public ModulatedSource(IModulationSource baseSource, IModulationSource modulator)
    {
        _baseSource = baseSource ?? throw new ArgumentNullException(nameof(baseSource));
        _modulator = modulator ?? throw new ArgumentNullException(nameof(modulator));
    }

    /// <summary>
    /// Gets the base modulation source.
    /// </summary>
    public IModulationSource BaseSource => _baseSource;

    /// <summary>
    /// Gets the modulator being applied to the base source.
    /// </summary>
    public IModulationSource Modulator => _modulator;

    /// <summary>
    /// Gets the current value from this modulated source at the specified time.
    /// The modulator's value is added to the base source's value.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    /// <returns>The modulated value at the specified time.</returns>
    public double GetValue(double time)
    {
        double baseValue = _baseSource.GetValue(time);
        double modulatorValue = _modulator.GetValue(time);
        return baseValue + modulatorValue;
    }

    /// <summary>
    /// Applies another modulator to this already modulated source.
    /// </summary>
    /// <param name="modulator">The additional modulator to apply.</param>
    /// <returns>A new modulated source with the additional modulator.</returns>
    public IModulationSource ApplyModulator(IModulationSource modulator)
    {
        return new ModulatedSource(this, modulator);
    }
}