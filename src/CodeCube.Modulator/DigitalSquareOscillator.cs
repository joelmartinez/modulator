namespace CodeCube.Modulator;

/// <summary>
/// A digital square wave oscillator that generates clean square wave values with configurable rate and amplitude.
/// This oscillator produces exact 0 and amplitude values with instantaneous transitions.
/// </summary>
public class DigitalSquareOscillator : IModulationSource
{
    private readonly double _rate;
    private readonly double _amplitude;

    /// <summary>
    /// Initializes a new instance of the DigitalSquareOscillator class.
    /// </summary>
    /// <param name="rate">The frequency of the square wave in Hz (cycles per second).</param>
    /// <param name="amplitude">The amplitude (peak value) of the square wave.</param>
    public DigitalSquareOscillator(double rate, double amplitude)
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
    /// Gets the current value from this digital square oscillator at the specified time.
    /// Returns either 0 or the amplitude value with instantaneous transitions.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    /// <returns>The square wave value at the specified time (either 0 or amplitude).</returns>
    public double GetValue(double time)
    {
        // Calculate the phase within one period (0 to 2π)
        double phase = (2 * Math.PI * _rate * time) % (2 * Math.PI);
        
        // Square wave is high for first half of period (0 to π), low for second half (π to 2π)
        return phase < Math.PI ? _amplitude : 0.0;
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