namespace CodeCube.Modulator.Tests;

public class DigitalSquareOscillatorTests
{
    [Fact]
    public void Constructor_SetsRateAndAmplitude()
    {
        // Arrange
        double rate = 440.0; // A4 note
        double amplitude = 0.5;

        // Act
        var oscillator = new DigitalSquareOscillator(rate, amplitude);

        // Assert
        Assert.Equal(rate, oscillator.Rate);
        Assert.Equal(amplitude, oscillator.Amplitude);
    }

    [Fact]
    public void GetValue_AtTimeZero_ReturnsAmplitude()
    {
        // Arrange
        var oscillator = new DigitalSquareOscillator(1.0, 1.0);

        // Act
        double value = oscillator.GetValue(0.0);

        // Assert
        Assert.Equal(1.0, value, precision: 10);
    }

    [Fact]
    public void GetValue_AtQuarterPeriod_ReturnsAmplitude()
    {
        // Arrange
        double rate = 1.0; // 1 Hz
        double amplitude = 2.0;
        var oscillator = new DigitalSquareOscillator(rate, amplitude);
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
        var oscillator = new DigitalSquareOscillator(rate, amplitude);
        double halfPeriod = 0.5; // Half of a 1Hz period

        // Act
        double value = oscillator.GetValue(halfPeriod);

        // Assert
        Assert.Equal(0.0, value, precision: 10);
    }

    [Fact]
    public void GetValue_AtThreeQuarterPeriod_ReturnsZero()
    {
        // Arrange
        double rate = 1.0; // 1 Hz
        double amplitude = 3.0;
        var oscillator = new DigitalSquareOscillator(rate, amplitude);
        double threeQuarterPeriod = 0.75; // Three quarters of a 1Hz period

        // Act
        double value = oscillator.GetValue(threeQuarterPeriod);

        // Assert
        Assert.Equal(0.0, value, precision: 10);
    }

    [Fact]
    public void GetValue_AtFullPeriod_ReturnsAmplitude()
    {
        // Arrange
        double rate = 1.0; // 1 Hz
        double amplitude = 2.5;
        var oscillator = new DigitalSquareOscillator(rate, amplitude);
        double fullPeriod = 1.0; // Full period of a 1Hz wave

        // Act
        double value = oscillator.GetValue(fullPeriod);

        // Assert
        Assert.Equal(amplitude, value, precision: 10);
    }

    [Fact]
    public void GetValue_MultiplePeriods_RepeatsPattern()
    {
        // Arrange
        double rate = 2.0; // 2 Hz - period is 0.5 seconds
        double amplitude = 1.5;
        var oscillator = new DigitalSquareOscillator(rate, amplitude);

        // Act & Assert - Test multiple periods
        for (int period = 0; period < 3; period++)
        {
            double periodStart = period * 0.5; // Each period is 0.5s for 2Hz
            
            // First quarter of period - should be amplitude
            Assert.Equal(amplitude, oscillator.GetValue(periodStart + 0.125), precision: 10);
            
            // Third quarter of period - should be zero
            Assert.Equal(0.0, oscillator.GetValue(periodStart + 0.375), precision: 10);
        }
    }

    [Fact]
    public void GetValue_HighFrequency_ProducesCorrectPattern()
    {
        // Arrange
        double rate = 100.0; // 100 Hz
        double amplitude = 1.0;
        var oscillator = new DigitalSquareOscillator(rate, amplitude);
        double period = 1.0 / rate; // 0.01 seconds

        // Act & Assert
        // First quarter of period
        Assert.Equal(amplitude, oscillator.GetValue(period * 0.25), precision: 10);
        
        // Just before half period
        Assert.Equal(amplitude, oscillator.GetValue(period * 0.49), precision: 10);
        
        // Just after half period
        Assert.Equal(0.0, oscillator.GetValue(period * 0.51), precision: 10);
        
        // Last quarter of period
        Assert.Equal(0.0, oscillator.GetValue(period * 0.75), precision: 10);
    }

    [Fact]
    public void GetValue_NegativeAmplitude_WorksCorrectly()
    {
        // Arrange
        double rate = 1.0;
        double amplitude = -2.0;
        var oscillator = new DigitalSquareOscillator(rate, amplitude);

        // Act & Assert
        Assert.Equal(amplitude, oscillator.GetValue(0.0), precision: 10);  // High state
        Assert.Equal(0.0, oscillator.GetValue(0.5), precision: 10);       // Low state
    }

    [Fact]
    public void GetValue_ZeroAmplitude_AlwaysReturnsZero()
    {
        // Arrange
        double rate = 1.0;
        double amplitude = 0.0;
        var oscillator = new DigitalSquareOscillator(rate, amplitude);

        // Act & Assert
        Assert.Equal(0.0, oscillator.GetValue(0.0), precision: 10);
        Assert.Equal(0.0, oscillator.GetValue(0.25), precision: 10);
        Assert.Equal(0.0, oscillator.GetValue(0.5), precision: 10);
        Assert.Equal(0.0, oscillator.GetValue(0.75), precision: 10);
    }

    [Fact]
    public void GetValue_VeryLowFrequency_WorksCorrectly()
    {
        // Arrange
        double rate = 0.1; // 0.1 Hz - period is 10 seconds
        double amplitude = 1.0;
        var oscillator = new DigitalSquareOscillator(rate, amplitude);

        // Act & Assert
        Assert.Equal(amplitude, oscillator.GetValue(2.5), precision: 10);  // Quarter period (2.5s)
        Assert.Equal(0.0, oscillator.GetValue(5.0), precision: 10);       // Half period (5s)
        Assert.Equal(0.0, oscillator.GetValue(7.5), precision: 10);       // Three quarter period (7.5s)
        Assert.Equal(amplitude, oscillator.GetValue(10.0), precision: 10); // Full period (10s)
    }

    [Fact]
    public void ApplyModulator_ReturnsModulatedSource()
    {
        // Arrange
        var baseOscillator = new DigitalSquareOscillator(1.0, 1.0);
        var modulator = new SinOscillator(2.0, 0.5);

        // Act
        var modulated = baseOscillator.ApplyModulator(modulator);

        // Assert
        Assert.IsType<ModulatedSource>(modulated);
    }

    [Fact]
    public void ApplyModulator_WithAnotherSquareWave_CombinesCorrectly()
    {
        // Arrange
        var baseOscillator = new DigitalSquareOscillator(1.0, 1.0);
        var modulator = new DigitalSquareOscillator(2.0, 0.5);
        var modulated = baseOscillator.ApplyModulator(modulator);

        // Act & Assert - At time 0, both should be at amplitude
        double expectedValue = 1.0 + 0.5; // base amplitude + modulator amplitude
        Assert.Equal(expectedValue, modulated.GetValue(0.0), precision: 10);

        // At time 0.25 (quarter period of base, half period of modulator)
        // Base: still amplitude (1.0), Modulator: zero (0.0)
        expectedValue = 1.0 + 0.0;
        Assert.Equal(expectedValue, modulated.GetValue(0.25), precision: 10);
    }
}