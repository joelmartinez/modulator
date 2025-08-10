namespace CodeCube.Modulator;

/// <summary>
/// Defines a modulation source that can generate values over time.
/// </summary>
public interface IModulationSource
{
    /// <summary>
    /// Gets the current value from this modulation source at the specified time.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    /// <returns>The modulated value at the specified time.</returns>
    double GetValue(double time);

    /// <summary>
    /// Applies a modulator to this source.
    /// </summary>
    /// <param name="modulator">The modulator to apply.</param>
    /// <returns>A new modulated source.</returns>
    IModulationSource ApplyModulator(IModulationSource modulator);
}
