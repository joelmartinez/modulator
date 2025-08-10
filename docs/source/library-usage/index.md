---
template: Default
title: Using the Library
subtitle: Complete guide to using the Modulator library classes and methods.
---

# Using the Modulator Library

This guide covers how to use the classes and interfaces provided by the Modulator library to create and manipulate modulated signals.

## Core Interface: IModulationSource

All modulation sources in the library implement the `IModulationSource` interface, which provides a consistent API for working with any type of modulation source.

```csharp
public interface IModulationSource
{
    double GetValue(double time);
    IModulationSource ApplyModulator(IModulationSource modulator);
}
```

### Methods

#### `GetValue(double time)`
Returns the value of the modulation source at a specific time (in seconds).

```csharp
var oscillator = new SinOscillator(440, 1.0);
double value = oscillator.GetValue(0.5); // Value at 0.5 seconds
```

#### `ApplyModulator(IModulationSource modulator)`
Creates a new modulated source by applying another modulation source as a modulator.

```csharp
var carrier = new SinOscillator(440, 1.0);
var modulator = new SinOscillator(5, 0.1);
var modulated = carrier.ApplyModulator(modulator);
```

## SinOscillator Class

The `SinOscillator` class generates sine wave signals and is the primary building block for creating oscillators.

### Constructor
```csharp
public SinOscillator(double rate, double amplitude)
```

- **rate**: Frequency in Hz (cycles per second)
- **amplitude**: Peak amplitude of the sine wave

### Properties
- **Rate**: Gets the frequency of the oscillator
- **Amplitude**: Gets the amplitude of the oscillator

### Example Usage

#### Basic Sine Wave
```csharp
// Create a 440Hz sine wave with amplitude 1.0
var oscillator = new SinOscillator(440, 1.0);

// Generate values over time
for (double t = 0; t < 1.0; t += 0.01)
{
    double value = oscillator.GetValue(t);
    Console.WriteLine($"Time: {t:F2}s, Value: {value:F3}");
}
```

#### Low Frequency Oscillator (LFO)
```csharp
// Create a slow oscillator for modulation purposes
var lfo = new SinOscillator(2.5, 0.5); // 2.5Hz at 50% amplitude

// This can be used to modulate other signals
var carrier = new SinOscillator(440, 1.0);
var modulated = carrier.ApplyModulator(lfo);
```

## ModulatedSource Class

The `ModulatedSource` class combines a base source with a modulator using additive synthesis (the modulator value is added to the base source value).

### Constructor
```csharp
public ModulatedSource(IModulationSource baseSource, IModulationSource modulator)
```

- **baseSource**: The primary signal source
- **modulator**: The modulation source to apply

### Properties
- **BaseSource**: Gets the base modulation source
- **Modulator**: Gets the modulator being applied

### Example Usage

#### Simple Modulation
```csharp
var carrier = new SinOscillator(440, 1.0);     // 440Hz carrier
var modulator = new SinOscillator(5, 0.2);     // 5Hz modulator with 0.2 amplitude

var modulated = new ModulatedSource(carrier, modulator);

// Or equivalently:
var modulated2 = carrier.ApplyModulator(modulator);
```

#### Accessing Components
```csharp
var modulated = new ModulatedSource(carrier, modulator);

// Access the original components
double carrierValue = modulated.BaseSource.GetValue(1.0);
double modulatorValue = modulated.Modulator.GetValue(1.0);
double combinedValue = modulated.GetValue(1.0);

Console.WriteLine($"Carrier: {carrierValue:F3}");
Console.WriteLine($"Modulator: {modulatorValue:F3}");
Console.WriteLine($"Combined: {combinedValue:F3}");
// Note: Combined = Carrier + Modulator
```

## Advanced Usage Patterns

### Chaining Multiple Modulators

You can apply multiple modulators by chaining `ApplyModulator` calls:

```csharp
var base = new SinOscillator(440, 1.0);
var mod1 = new SinOscillator(5, 0.1);      // Slow vibrato
var mod2 = new SinOscillator(0.5, 2);      // Very slow, deep modulation

// Chain modulators
var complex = base
    .ApplyModulator(mod1)
    .ApplyModulator(mod2);

// This creates: base + mod1 + mod2
```

### Creating Modulation Networks

For more complex modulation scenarios, you can create networks of modulators:

```csharp
// Create a vibrato that itself is modulated
var vibratoRate = new SinOscillator(6, 1.0);          // 6Hz vibrato
var vibratoDepth = new SinOscillator(0.2, 0.05);      // Varying depth
var vibrato = vibratoRate.ApplyModulator(vibratoDepth);

// Apply the complex vibrato to a carrier
var note = new SinOscillator(440, 1.0);
var expressiveNote = note.ApplyModulator(vibrato);
```

### Generating Sample Data

For audio applications, you'll often need to generate arrays of sample data:

```csharp
public static double[] GenerateSamples(IModulationSource source, 
                                     double duration, 
                                     int sampleRate)
{
    int sampleCount = (int)(duration * sampleRate);
    double[] samples = new double[sampleCount];
    
    for (int i = 0; i < sampleCount; i++)
    {
        double time = (double)i / sampleRate;
        samples[i] = source.GetValue(time);
    }
    
    return samples;
}

// Usage
var oscillator = new SinOscillator(440, 1.0);
double[] audioSamples = GenerateSamples(oscillator, 2.0, 44100); // 2 seconds at 44.1kHz
```

### Real-time Processing

For real-time applications, you can process samples continuously:

```csharp
public class RealTimeProcessor
{
    private readonly IModulationSource _source;
    private double _currentTime = 0;
    private readonly double _sampleRate;
    
    public RealTimeProcessor(IModulationSource source, double sampleRate)
    {
        _source = source;
        _sampleRate = sampleRate;
    }
    
    public double GetNextSample()
    {
        double sample = _source.GetValue(_currentTime);
        _currentTime += 1.0 / _sampleRate;
        return sample;
    }
}

// Usage
var processor = new RealTimeProcessor(
    new SinOscillator(440, 1.0).ApplyModulator(new SinOscillator(5, 0.1)), 
    44100
);

// In your audio callback
double nextSample = processor.GetNextSample();
```

## Best Practices

### Performance Considerations

1. **Cache Calculated Values**: If you're using the same time values repeatedly, consider caching results
2. **Minimize Object Creation**: Create your modulation sources once and reuse them
3. **Use Appropriate Data Types**: For high-performance scenarios, consider using `float` instead of `double`

### Mathematical Considerations

1. **Frequency Ranges**: Be mindful of the frequencies you're generating relative to your sample rate
2. **Amplitude Management**: Ensure your combined signals don't exceed desired amplitude ranges
3. **Phase Relationships**: Consider the phase relationships between multiple oscillators

### Design Patterns

1. **Factory Pattern**: Create factory methods for common modulation setups
2. **Builder Pattern**: Use builders for complex modulation networks
3. **Strategy Pattern**: Abstract different modulation algorithms behind the `IModulationSource` interface

## Example Applications

### Simple Synthesizer Voice
```csharp
public class SimpleVoice
{
    private readonly IModulationSource _voice;
    
    public SimpleVoice(double frequency)
    {
        var carrier = new SinOscillator(frequency, 0.8);
        var vibrato = new SinOscillator(5, frequency * 0.01); // 1% vibrato
        var tremolo = new SinOscillator(3, 0.1);               // 10% tremolo
        
        _voice = carrier.ApplyModulator(vibrato).ApplyModulator(tremolo);
    }
    
    public double GetSample(double time) => _voice.GetValue(time);
}
```

### Signal Generator for Testing
```csharp
public class TestSignalGenerator
{
    public static IModulationSource CreateSweep(double startFreq, double endFreq, double duration)
    {
        double sweepRate = (endFreq - startFreq) / duration;
        var baseOsc = new SinOscillator(startFreq, 1.0);
        var sweepMod = new SinOscillator(1.0 / duration, sweepRate);
        
        return baseOsc.ApplyModulator(sweepMod);
    }
}
```

This library provides a solid foundation for building more complex modulation systems while maintaining simplicity and performance.