namespace CodeCube.Modulator.Tests;

public class AnalogSquareOscillatorTests
{
    [Fact]
    public void Constructor_WithRateAndAmplitude_SetsPropertiesWithDefaultTimes()
    {
        // Arrange
        double rate = 440.0; // A4 note
        double amplitude = 0.5;

        // Act
        var oscillator = new AnalogSquareOscillator(rate, amplitude);

        // Assert
        Assert.Equal(rate, oscillator.Rate);
        Assert.Equal(amplitude, oscillator.Amplitude);
        Assert.Equal(0.01, oscillator.RiseTime);
        Assert.Equal(0.01, oscillator.FallTime);
    }

    [Fact]
    public void Constructor_WithCustomTimes_SetsAllProperties()
    {
        // Arrange
        double rate = 1000.0;
        double amplitude = 2.0;
        double riseTime = 0.05;
        double fallTime = 0.03;

        // Act
        var oscillator = new AnalogSquareOscillator(rate, amplitude, riseTime, fallTime);

        // Assert
        Assert.Equal(rate, oscillator.Rate);
        Assert.Equal(amplitude, oscillator.Amplitude);
        Assert.Equal(riseTime, oscillator.RiseTime);
        Assert.Equal(fallTime, oscillator.FallTime);
    }

    [Fact]
    public void Constructor_WithExtremeRiseFallTimes_ClampsToValidRange()
    {
        // Arrange & Act
        var oscillator1 = new AnalogSquareOscillator(1.0, 1.0, -0.1, 0.6); // Negative rise, too large fall
        var oscillator2 = new AnalogSquareOscillator(1.0, 1.0, 0.0, 0.0); // Zero times

        // Assert
        Assert.True(oscillator1.RiseTime >= 0.001);
        Assert.True(oscillator1.RiseTime <= 0.4);
        Assert.True(oscillator1.FallTime >= 0.001);
        Assert.True(oscillator1.FallTime <= 0.4);

        Assert.True(oscillator2.RiseTime >= 0.001);
        Assert.True(oscillator2.FallTime >= 0.001);
    }

    [Fact]
    public void GetValue_AtTimeZero_IsNearAmplitudeButNotExact()
    {
        // Arrange
        var oscillator = new AnalogSquareOscillator(1.0, 1.0);

        // Act
        double value = oscillator.GetValue(0.0);

        // Assert
        // At time 0, we're at the start of rise time, so should be low but rising
        Assert.True(value >= 0.0);
        Assert.True(value <= oscillator.Amplitude * 0.5); // Should be in lower half during rise
    }

    [Fact]
    public void GetValue_AfterRiseTime_IsNearAmplitude()
    {
        // Arrange
        double rate = 1.0; // 1 Hz
        double amplitude = 2.0;
        var oscillator = new AnalogSquareOscillator(rate, amplitude, 0.05, 0.05); // 5% rise/fall time
        double afterRiseTime = 0.1; // Well after rise time ends

        // Act
        double value = oscillator.GetValue(afterRiseTime);

        // Assert
        // Should be close to amplitude but allow for some analog characteristics
        Assert.True(Math.Abs(value - amplitude) < amplitude * 0.2); // Within 20% of target
        Assert.True(value > amplitude * 0.8); // At least 80% of amplitude
    }

    [Fact]
    public void GetValue_DuringFallTransition_IsDecreasing()
    {
        // Arrange
        double rate = 1.0; // 1 Hz, period = 1 second
        double amplitude = 1.0;
        var oscillator = new AnalogSquareOscillator(rate, amplitude, 0.02, 0.08); // 2% rise, 8% fall
        
        double fallStart = 0.5; // Fall starts at half period
        double fallMid = 0.54;  // Middle of fall transition
        double fallEnd = 0.58;  // End of fall transition

        // Act
        double valueAtStart = oscillator.GetValue(fallStart);
        double valueAtMid = oscillator.GetValue(fallMid);
        double valueAtEnd = oscillator.GetValue(fallEnd);

        // Assert
        // Values should generally decrease during fall transition
        Assert.True(valueAtStart > valueAtMid);
        Assert.True(valueAtMid > valueAtEnd);
        Assert.True(valueAtEnd < amplitude * 0.3); // Should be quite low by end of fall
    }

    [Fact]
    public void GetValue_InLowState_IsNearZero()
    {
        // Arrange
        double rate = 1.0; // 1 Hz
        double amplitude = 3.0;
        var oscillator = new AnalogSquareOscillator(rate, amplitude, 0.02, 0.02);
        double lowStateTime = 0.8; // Well into the low state

        // Act
        double value = oscillator.GetValue(lowStateTime);

        // Assert
        // Should be close to zero but allow for analog characteristics
        Assert.True(Math.Abs(value) < amplitude * 0.1); // Within 10% of zero
    }

    [Fact]
    public void GetValue_MultiplePeriods_RepeatsPattern()
    {
        // Arrange
        double rate = 2.0; // 2 Hz - period is 0.5 seconds
        double amplitude = 1.5;
        var oscillator = new AnalogSquareOscillator(rate, amplitude);

        // Act & Assert - Test that the pattern repeats
        double value1 = oscillator.GetValue(0.1);  // Time in first period
        double value2 = oscillator.GetValue(0.6);  // Same relative time in second period (0.1 + 0.5)
        double value3 = oscillator.GetValue(1.1);  // Same relative time in third period (0.1 + 1.0)

        // Assert - Values should be approximately the same (allowing for small numeric differences)
        Assert.True(Math.Abs(value1 - value2) < 0.001);
        Assert.True(Math.Abs(value2 - value3) < 0.001);
    }

    [Fact]
    public void GetValue_HighFrequency_MaintainsAnalogCharacteristics()
    {
        // Arrange
        double rate = 100.0; // 100 Hz
        double amplitude = 1.0;
        var oscillator = new AnalogSquareOscillator(rate, amplitude);
        double period = 1.0 / rate; // 0.01 seconds

        // Act & Assert
        double highStateValue = oscillator.GetValue(period * 0.3); // Middle of high state
        double lowStateValue = oscillator.GetValue(period * 0.8);  // Middle of low state

        // Should maintain the general square wave shape even at high frequency
        Assert.True(highStateValue > lowStateValue);
        Assert.True(highStateValue > amplitude * 0.5);
        Assert.True(Math.Abs(lowStateValue) < amplitude * 0.2);
    }

    [Fact]
    public void GetValue_NegativeAmplitude_WorksCorrectly()
    {
        // Arrange
        double rate = 1.0;
        double amplitude = -2.0;
        var oscillator = new AnalogSquareOscillator(rate, amplitude);

        // Act
        double highStateValue = oscillator.GetValue(0.2); // During high state
        double lowStateValue = oscillator.GetValue(0.7);  // During low state

        // Assert
        // With negative amplitude, "high" state should be more negative
        Assert.True(highStateValue < lowStateValue); // "High" is more negative
        Assert.True(highStateValue < amplitude * 0.5); // Should approach negative amplitude
    }

    [Fact]
    public void GetValue_ZeroAmplitude_ReturnsNearZero()
    {
        // Arrange
        double rate = 1.0;
        double amplitude = 0.0;
        var oscillator = new AnalogSquareOscillator(rate, amplitude);

        // Act & Assert
        Assert.True(Math.Abs(oscillator.GetValue(0.0)) < 0.01);
        Assert.True(Math.Abs(oscillator.GetValue(0.25)) < 0.01);
        Assert.True(Math.Abs(oscillator.GetValue(0.5)) < 0.01);
        Assert.True(Math.Abs(oscillator.GetValue(0.75)) < 0.01);
    }

    [Fact]
    public void GetValue_VeryLowFrequency_WorksCorrectly()
    {
        // Arrange
        double rate = 0.1; // 0.1 Hz - period is 10 seconds
        double amplitude = 1.0;
        var oscillator = new AnalogSquareOscillator(rate, amplitude);

        // Act & Assert
        double earlyValue = oscillator.GetValue(1.0);    // Early in high state
        double midValue = oscillator.GetValue(3.0);      // Middle of high state  
        double lateHighValue = oscillator.GetValue(4.5); // Late in high state
        double lowValue = oscillator.GetValue(7.0);      // In low state

        // Verify general square wave behavior over long period
        Assert.True(earlyValue > lowValue);
        Assert.True(midValue > lowValue);
        Assert.True(lateHighValue > lowValue);
    }

    [Fact]
    public void ApplyModulator_ReturnsModulatedSource()
    {
        // Arrange
        var baseOscillator = new AnalogSquareOscillator(1.0, 1.0);
        var modulator = new SinOscillator(2.0, 0.5);

        // Act
        var modulated = baseOscillator.ApplyModulator(modulator);

        // Assert
        Assert.IsType<ModulatedSource>(modulated);
    }

    [Fact]
    public void ApplyModulator_WithSinusoidalModulator_CombinesSignals()
    {
        // Arrange
        var baseOscillator = new AnalogSquareOscillator(1.0, 1.0);
        var modulator = new SinOscillator(10.0, 0.1); // High freq, low amplitude modulation
        var modulated = baseOscillator.ApplyModulator(modulator);

        // Act
        double baseValue = baseOscillator.GetValue(0.2);
        double modulatorValue = modulator.GetValue(0.2);
        double combinedValue = modulated.GetValue(0.2);

        // Assert
        // Combined value should be approximately base + modulator
        Assert.True(Math.Abs(combinedValue - (baseValue + modulatorValue)) < 0.001);
    }

    [Fact]
    public void AnalogVsDigital_ShowsDifferentCharacteristics()
    {
        // Arrange
        double rate = 10.0;
        double amplitude = 1.0;
        var digitalOsc = new DigitalSquareOscillator(rate, amplitude);
        var analogOsc = new AnalogSquareOscillator(rate, amplitude, 0.1, 0.1); // Longer rise/fall times

        // Act - Sample during the middle of transition periods to catch intermediate values
        double period = 1.0 / rate; // 0.1 seconds
        double[] transitionTimes = { 
            period * 0.02,  // 20% through rise period (should be intermediate)
            period * 0.03,  // 30% through rise period (should be intermediate)
            period * 0.52,  // 20% through fall period
            period * 0.53   // 30% through fall period
        };
        
        double[] digitalSamples = new double[transitionTimes.Length];
        double[] analogSamples = new double[transitionTimes.Length];

        for (int i = 0; i < transitionTimes.Length; i++)
        {
            digitalSamples[i] = digitalOsc.GetValue(transitionTimes[i]);
            analogSamples[i] = analogOsc.GetValue(transitionTimes[i]);
        }

        // Assert - Digital oscillator should have exact 0 or amplitude values
        Assert.True(digitalSamples.All(v => Math.Abs(v) < 0.001 || Math.Abs(v - amplitude) < 0.001));
        
        // Analog oscillator should have intermediate values during transitions
        Assert.True(analogSamples.Any(v => v > 0.2 && v < amplitude - 0.2));
    }
}