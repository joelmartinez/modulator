namespace CodeCube.Modulator;

/// <summary>
/// A sine wave oscillator that generates sine wave values with configurable rate and amplitude.
/// </summary>
public class SinOscillator : IModulationSource
{
    private readonly double _rate;
    private readonly double _amplitude;

    /// <summary>
    /// Initializes a new instance of the SinOscillator class.
    /// </summary>
    /// <param name="rate">The frequency of the sine wave in Hz (cycles per second).</param>
    /// <param name="amplitude">The amplitude (peak value) of the sine wave.</param>
    public SinOscillator(double rate, double amplitude)
    {
        _rate = rate;
        _amplitude = amplitude;
    }

    /// <summary>
    /// Gets the rate (frequency) of the oscillator in Hz.
    /// </summary>
    public double Rate => _rate;

    /// <summary>
    /// Gets the amplitude of the oscillator.
    /// </summary>
    public double Amplitude => _amplitude;

    /// <summary>
    /// Gets the current value from this sine oscillator at the specified time.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    /// <returns>The sine wave value at the specified time.</returns>
    public double GetValue(double time)
    {
        return _amplitude * Math.Sin(2 * Math.PI * _rate * time);
    }

    /// <summary>
    /// Applies a modulator to this oscillator, creating a modulated version.
    /// </summary>
    /// <param name="modulator">The modulator to apply.</param>
    /// <returns>A new modulated oscillator.</returns>
    public IModulationSource ApplyModulator(IModulationSource modulator)
    {
        return new ModulatedSource(this, modulator);
    }
}