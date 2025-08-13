namespace CodeCube.Modulator;

/// <summary>
/// An analog square wave oscillator that simulates the characteristics of real analog square wave signals.
/// Unlike the digital version, this oscillator has configurable rise/fall times and smoother transitions
/// that more closely resemble the behavior of analog electronic circuits.
/// </summary>
public class AnalogSquareOscillator : IModulationSource
{
    private readonly double _rate;
    private readonly double _amplitude;
    private readonly double _riseTime;
    private readonly double _fallTime;

    /// <summary>
    /// Initializes a new instance of the AnalogSquareOscillator class with default rise/fall times.
    /// </summary>
    /// <param name="rate">The frequency of the square wave in Hz (cycles per second).</param>
    /// <param name="amplitude">The amplitude (peak value) of the square wave.</param>
    public AnalogSquareOscillator(double rate, double amplitude) : this(rate, amplitude, 0.01, 0.01)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AnalogSquareOscillator class with custom rise/fall times.
    /// </summary>
    /// <param name="rate">The frequency of the square wave in Hz (cycles per second).</param>
    /// <param name="amplitude">The amplitude (peak value) of the square wave.</param>
    /// <param name="riseTime">The time it takes for the signal to rise from 10% to 90% of amplitude (as fraction of period).</param>
    /// <param name="fallTime">The time it takes for the signal to fall from 90% to 10% of amplitude (as fraction of period).</param>
    public AnalogSquareOscillator(double rate, double amplitude, double riseTime, double fallTime)
    {
        _rate = rate;
        _amplitude = amplitude;
        _riseTime = Math.Max(0.001, Math.Min(0.4, riseTime)); // Clamp between 0.1% and 40% of period
        _fallTime = Math.Max(0.001, Math.Min(0.4, fallTime)); // Clamp between 0.1% and 40% of period
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
    /// Gets the rise time as a fraction of the period.
    /// </summary>
    public double RiseTime => _riseTime;

    /// <summary>
    /// Gets the fall time as a fraction of the period.
    /// </summary>
    public double FallTime => _fallTime;

    /// <summary>
    /// Gets the current value from this analog square oscillator at the specified time.
    /// The output includes smooth transitions during rise and fall times, simulating analog behavior.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    /// <returns>The analog square wave value at the specified time.</returns>
    public double GetValue(double time)
    {
        // Calculate the phase within one period (0 to 1)
        double period = 1.0 / _rate;
        double normalizedTime = (time % period) / period;
        
        // Ensure normalizedTime is always positive
        if (normalizedTime < 0) normalizedTime += 1.0;

        // Define the transition points
        double riseStart = 0.0;
        double riseEnd = _riseTime;
        double fallStart = 0.5;
        double fallEnd = 0.5 + _fallTime;

        if (normalizedTime >= riseStart && normalizedTime < riseEnd)
        {
            // Rising edge - exponential rise with slight overshoot
            double riseProgress = (normalizedTime - riseStart) / (riseEnd - riseStart);
            double baseRise = 1.0 - Math.Exp(-5.0 * riseProgress); // Exponential rise
            double overshoot = 0.05 * Math.Sin(Math.PI * riseProgress) * Math.Exp(-3.0 * riseProgress); // Small overshoot
            return _amplitude * Math.Min(1.1, baseRise + overshoot); // Cap at 110% for overshoot
        }
        else if (normalizedTime >= riseEnd && normalizedTime < fallStart)
        {
            // High state - steady at amplitude with tiny noise-like variation
            double noise = 0.01 * Math.Sin(normalizedTime * 50 * Math.PI) * Math.Exp(-10 * (normalizedTime - riseEnd));
            return _amplitude * (1.0 + noise);
        }
        else if (normalizedTime >= fallStart && normalizedTime < fallEnd)
        {
            // Falling edge - exponential fall with slight undershoot
            double fallProgress = (normalizedTime - fallStart) / (fallEnd - fallStart);
            double baseFall = Math.Exp(-5.0 * fallProgress); // Exponential fall
            double undershoot = -0.03 * Math.Sin(Math.PI * fallProgress) * Math.Exp(-3.0 * fallProgress); // Small undershoot
            return _amplitude * Math.Max(-0.05, baseFall + undershoot); // Allow small negative undershoot
        }
        else
        {
            // Low state - close to zero with small settling behavior
            double settlingNoise = normalizedTime > fallEnd ? 
                0.005 * Math.Exp(-20 * (normalizedTime - fallEnd)) * Math.Sin(normalizedTime * 30 * Math.PI) : 0;
            return _amplitude * settlingNoise;
        }
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