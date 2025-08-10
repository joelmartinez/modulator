namespace CodeCube.Modulator.Tests;

public class SinOscillatorTests
{
    [Fact]
    public void Constructor_SetsRateAndAmplitude()
    {
        // Arrange
        double rate = 440.0; // A4 note
        double amplitude = 0.5;

        // Act
        var oscillator = new SinOscillator(rate, amplitude);

        // Assert
        Assert.Equal(rate, oscillator.Rate);
        Assert.Equal(amplitude, oscillator.Amplitude);
    }

    [Fact]
    public void GetValue_AtTimeZero_ReturnsZero()
    {
        // Arrange
        var oscillator = new SinOscillator(1.0, 1.0);

        // Act
        double value = oscillator.GetValue(0.0);

        // Assert
        Assert.Equal(0.0, value, precision: 10);
    }

    [Fact]
    public void GetValue_AtQuarterPeriod_ReturnsAmplitude()
    {
        // Arrange
        double rate = 1.0; // 1 Hz
        double amplitude = 2.0;
        var oscillator = new SinOscillator(rate, amplitude);
        double quarterPeriod = 0.25; // Quarter of a 1Hz period

        // Act
        double value = oscillator.GetValue(quarterPeriod);

        // Assert
        Assert.Equal(amplitude, value, precision: 10);
    }

    [Fact]
    public void GetValue_AtHalfPeriod_ReturnsZero()
    {
        // Arrange
        double rate = 1.0; // 1 Hz
        double amplitude = 1.0;
        var oscillator = new SinOscillator(rate, amplitude);
        double halfPeriod = 0.5; // Half of a 1Hz period

        // Act
        double value = oscillator.GetValue(halfPeriod);

        // Assert
        Assert.Equal(0.0, value, precision: 10);
    }

    [Fact]
    public void GetValue_AtThreeQuarterPeriod_ReturnsNegativeAmplitude()
    {
        // Arrange
        double rate = 1.0; // 1 Hz
        double amplitude = 3.0;
        var oscillator = new SinOscillator(rate, amplitude);
        double threeQuarterPeriod = 0.75; // Three quarters of a 1Hz period

        // Act
        double value = oscillator.GetValue(threeQuarterPeriod);

        // Assert
        Assert.Equal(-amplitude, value, precision: 10);
    }

    [Fact]
    public void ApplyModulator_ReturnsModulatedSource()
    {
        // Arrange
        var baseOscillator = new SinOscillator(1.0, 1.0);
        var modulator = new SinOscillator(2.0, 0.5);

        // Act
        var modulated = baseOscillator.ApplyModulator(modulator);

        // Assert
        Assert.IsType<ModulatedSource>(modulated);
    }
}