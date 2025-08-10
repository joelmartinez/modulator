namespace CodeCube.Modulator.Tests;

public class IntegrationTests
{
    [Fact]
    public void ComplexModulation_WithMultipleOscillators_WorksCorrectly()
    {
        // Arrange
        // Create a base 440Hz oscillator (A4 note) with amplitude 0.8
        var baseOscillator = new SinOscillator(440.0, 0.8);
        
        // Create a 6Hz vibrato modulator with small amplitude
        var vibratoModulator = new SinOscillator(6.0, 0.05);
        
        // Create a 2Hz tremolo modulator
        var tremoloModulator = new SinOscillator(2.0, 0.1);

        // Act
        // Apply vibrato first, then tremolo
        var vibratoModulated = baseOscillator.ApplyModulator(vibratoModulator);
        var fullyModulated = vibratoModulated.ApplyModulator(tremoloModulator);

        // Assert
        // Test at various time points to ensure the modulation is working
        double[] testTimes = { 0.0, 0.1, 0.25, 0.5, 1.0 };
        
        foreach (double time in testTimes)
        {
            double modulatedValue = fullyModulated.GetValue(time);
            double baseValue = baseOscillator.GetValue(time);
            double vibratoValue = vibratoModulator.GetValue(time);
            double tremoloValue = tremoloModulator.GetValue(time);
            double expectedValue = baseValue + vibratoValue + tremoloValue;
            
            Assert.Equal(expectedValue, modulatedValue, precision: 12);
        }
    }

    [Fact]
    public void OscillatorModulatingOscillator_WorksCorrectly()
    {
        // Arrange
        // Create two oscillators where one modulates the other
        var carrier = new SinOscillator(100.0, 1.0);    // 100Hz carrier
        var modulator = new SinOscillator(10.0, 0.2);   // 10Hz modulator

        // Act
        var modulatedCarrier = carrier.ApplyModulator(modulator);

        // Assert
        // Test that the modulated signal has characteristics of both frequencies
        double time = 0.025; // 1/4 period of the 10Hz modulator
        
        double carrierValue = carrier.GetValue(time);
        double modulatorValue = modulator.GetValue(time);
        double modulatedValue = modulatedCarrier.GetValue(time);
        
        Assert.Equal(carrierValue + modulatorValue, modulatedValue, precision: 12);
        
        // The modulator should be at its peak at this time (0.2)
        Assert.Equal(0.2, modulatorValue, precision: 10);
    }

    [Fact]
    public void ChainedModulation_PreservesIndividualComponents()
    {
        // Arrange
        var osc1 = new SinOscillator(1.0, 1.0);
        var osc2 = new SinOscillator(2.0, 0.5);
        var osc3 = new SinOscillator(3.0, 0.25);

        // Act
        var step1 = osc1.ApplyModulator(osc2);
        var step2 = step1.ApplyModulator(osc3);

        // Assert
        // At any given time, the final value should be the sum of all three oscillators
        double[] testTimes = { 0.0, 0.1, 0.2, 0.33, 0.5 };
        
        foreach (double time in testTimes)
        {
            double finalValue = step2.GetValue(time);
            double expectedValue = osc1.GetValue(time) + osc2.GetValue(time) + osc3.GetValue(time);
            
            Assert.Equal(expectedValue, finalValue, precision: 12);
        }
    }

    [Fact]
    public void ModulationWithZeroAmplitude_DoesNotAffectBase()
    {
        // Arrange
        var baseOscillator = new SinOscillator(440.0, 1.0);
        var zeroModulator = new SinOscillator(10.0, 0.0); // Zero amplitude

        // Act
        var modulated = baseOscillator.ApplyModulator(zeroModulator);

        // Assert
        // The modulated signal should be identical to the base signal
        double[] testTimes = { 0.0, 0.1, 0.25, 0.5, 1.0 };
        
        foreach (double time in testTimes)
        {
            double baseValue = baseOscillator.GetValue(time);
            double modulatedValue = modulated.GetValue(time);
            
            Assert.Equal(baseValue, modulatedValue, precision: 12);
        }
    }

    [Fact]
    public void HighFrequencyModulation_ProducesExpectedResults()
    {
        // Arrange
        var baseOscillator = new SinOscillator(1000.0, 0.5);  // 1kHz base
        var highFreqModulator = new SinOscillator(5000.0, 0.1); // 5kHz modulator

        // Act
        var modulated = baseOscillator.ApplyModulator(highFreqModulator);

        // Assert
        // Test at a specific time where we can calculate expected values
        double time = 0.0001; // 0.1ms
        
        double baseValue = baseOscillator.GetValue(time);
        double modulatorValue = highFreqModulator.GetValue(time);
        double modulatedValue = modulated.GetValue(time);
        
        Assert.Equal(baseValue + modulatorValue, modulatedValue, precision: 12);
    }
}