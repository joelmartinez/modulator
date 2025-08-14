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

The `SinOscillator` class generates sine wave signals and is the primary building block for creating smooth oscillators.

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

<div class="waveform-visualization">
    <canvas id="basicSineWaveCanvas" width="800" height="300"></canvas>
</div>

#### Low Frequency Oscillator (LFO)
```csharp
// Create a slow oscillator for modulation purposes
var lfo = new SinOscillator(2.5, 0.5); // 2.5Hz at 50% amplitude

// This can be used to modulate other signals
var carrier = new SinOscillator(440, 1.0);
var modulated = carrier.ApplyModulator(lfo);
```

<div class="waveform-visualization">
    <canvas id="lfoCanvas" width="800" height="300"></canvas>
</div>

## DigitalSquareOscillator Class

The `DigitalSquareOscillator` class generates clean, digital square wave signals with instantaneous transitions between 0 and amplitude values.

### Constructor
```csharp
public DigitalSquareOscillator(double rate, double amplitude)
```

- **rate**: Frequency in Hz (cycles per second)
- **amplitude**: Peak amplitude of the square wave

### Properties
- **Rate**: Gets the frequency of the oscillator
- **Amplitude**: Gets the amplitude of the oscillator

### Characteristics
- **Clean transitions**: Instantaneous switching between 0 and amplitude
- **Perfect timing**: Exactly 50% duty cycle
- **Digital precision**: No overshoot, undershoot, or settling time

### Example Usage

#### Basic Digital Square Wave
```csharp
// Create a 1kHz digital square wave
var digitalSquare = new DigitalSquareOscillator(1000, 1.0);

// The output will be exactly 0 or 1.0 at any point in time
for (double t = 0; t < 0.002; t += 0.0001) // 2ms worth of samples
{
    double value = digitalSquare.GetValue(t);
    Console.WriteLine($"Time: {t:F4}s, Value: {value:F1}"); // Will print 0.0 or 1.0
}
```

<div class="waveform-visualization">
    <canvas id="digitalSquareCanvas" width="800" height="300"></canvas>
</div>

#### Clock Signal Generation
```csharp
// Generate a precise clock signal
var clockSignal = new DigitalSquareOscillator(100, 3.3); // 100Hz, 3.3V logic level

// Perfect for timing applications
bool isHigh = clockSignal.GetValue(someTime) > 1.5; // TTL logic threshold
```

## AnalogSquareOscillator Class

The `AnalogSquareOscillator` class simulates realistic analog square wave behavior with configurable rise/fall times and smooth transitions.

### Constructors
```csharp
public AnalogSquareOscillator(double rate, double amplitude)
public AnalogSquareOscillator(double rate, double amplitude, double riseTime, double fallTime)
```

- **rate**: Frequency in Hz (cycles per second)
- **amplitude**: Peak amplitude of the square wave
- **riseTime**: Time to rise from 10% to 90% amplitude (as fraction of period, default: 0.01)
- **fallTime**: Time to fall from 90% to 10% amplitude (as fraction of period, default: 0.01)

### Properties
- **Rate**: Gets the frequency of the oscillator
- **Amplitude**: Gets the amplitude of the oscillator
- **RiseTime**: Gets the rise time as a fraction of the period
- **FallTime**: Gets the fall time as a fraction of the period

### Characteristics
- **Smooth transitions**: Exponential rise and fall curves
- **Overshoot/undershoot**: Slight overshoot on rising edge, undershoot on falling edge
- **Settling behavior**: Small oscillations after transitions
- **Analog realism**: Mimics real analog circuit behavior

### Example Usage

#### Basic Analog Square Wave
```csharp
// Create an analog square wave with default rise/fall times
var analogSquare = new AnalogSquareOscillator(440, 1.0);

// The output will show smooth transitions and analog characteristics
for (double t = 0; t < 0.01; t += 0.0001)
{
    double value = analogSquare.GetValue(t);
    Console.WriteLine($"Time: {t:F4}s, Value: {value:F6}");
}
```

<div class="waveform-visualization">
    <canvas id="analogSquareCanvas" width="800" height="300"></canvas>
</div>

#### Custom Rise/Fall Times
```csharp
// Create an analog square wave with slower transitions
var slowAnalog = new AnalogSquareOscillator(100, 1.0, 0.05, 0.08);
// 5% rise time, 8% fall time (as fraction of period)

// This simulates a slower op-amp or transistor circuit
```

#### Audio Synthesis Application
```csharp
// Square wave for audio synthesis with natural analog characteristics
var audioSquare = new AnalogSquareOscillator(220, 0.8, 0.02, 0.03);

// Apply some vibrato
var vibrato = new SinOscillator(5, 0.05);
var expressiveSquare = audioSquare.ApplyModulator(vibrato);
```

### Choosing Between Digital and Analog Square Waves

**Use DigitalSquareOscillator when:**
- You need perfect, instantaneous transitions
- Building digital clock or timing signals
- Mathematical precision is required
- Simulating ideal digital circuits
- Creating harsh, aggressive audio textures

**Use AnalogSquareOscillator when:**
- You want realistic analog circuit behavior
- Building audio applications where warmth matters
- Simulating real electronic instruments
- Need smooth transitions for visual applications
- Modeling actual hardware behavior

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

<div class="waveform-visualization">
    <canvas id="simpleModulationCanvas" width="800" height="300"></canvas>
</div>

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

### Multi-Oscillator Synthesizer with Different Wave Types
```csharp
public class MultiOscSynth
{
    private readonly IModulationSource _combinedSignal;
    
    public MultiOscSynth(double frequency)
    {
        // Main oscillators at the same frequency
        var sineOsc = new SinOscillator(frequency, 0.3);
        var digitalSquare = new DigitalSquareOscillator(frequency, 0.4);
        var analogSquare = new AnalogSquareOscillator(frequency, 0.3, 0.03, 0.04);
        
        // Sub-oscillator one octave down
        var subOsc = new AnalogSquareOscillator(frequency / 2, 0.2);
        
        // Combine all oscillators
        _combinedSignal = sineOsc
            .ApplyModulator(digitalSquare)
            .ApplyModulator(analogSquare)
            .ApplyModulator(subOsc);
    }
    
    public double GetSample(double time) => _combinedSignal.GetValue(time);
}
```

<div class="waveform-visualization">
    <canvas id="multiOscCanvas" width="800" height="300"></canvas>
</div>

### Percussion Synthesis with Square Waves
```csharp
public class DrumSynth
{
    public static IModulationSource CreateKick(double frequency = 60)
    {
        // Use analog square for body, digital square for click
        var body = new AnalogSquareOscillator(frequency, 1.0, 0.1, 0.2);
        var click = new DigitalSquareOscillator(frequency * 4, 0.3);
        
        // Pitch envelope - frequency sweeps down quickly
        var pitchEnv = new SinOscillator(0.5, frequency * 0.5); // Slow sweep
        
        return body.ApplyModulator(click).ApplyModulator(pitchEnv);
    }
    
    public static IModulationSource CreateSnare(double frequency = 200)
    {
        // Digital square for the sharp attack
        var tone = new DigitalSquareOscillator(frequency, 0.6);
        
        // High frequency modulation for noise-like character
        var noise = new AnalogSquareOscillator(frequency * 7, 0.4, 0.005, 0.005);
        
        return tone.ApplyModulator(noise);
    }
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
    
    public static IModulationSource CreateSquareWaveTest(double frequency)
    {
        // Compare digital vs analog square waves
        var digital = new DigitalSquareOscillator(frequency, 0.5);
        var analog = new AnalogSquareOscillator(frequency, 0.5);
        
        // Modulate between them slowly
        var selector = new SinOscillator(0.1, 0.25); // Very slow fade
        
        return digital.ApplyModulator(analog).ApplyModulator(selector);
    }
}
```

This library provides a solid foundation for building more complex modulation systems while maintaining simplicity and performance.

## Sample CLI and Visualizations

All examples in this documentation can be generated and visualized using the included sample CLI application:

```bash
# Navigate to the source directory
cd src

# Generate waveform data for any example
dotnet run --project ModulatorSampleCLI basic-sine output.json
dotnet run --project ModulatorSampleCLI vibrato vibrato.json
dotnet run --project ModulatorSampleCLI multi-osc multi-osc.json

# List all available examples
dotnet run --project ModulatorSampleCLI list
```

<script>
document.addEventListener('DOMContentLoaded', function() {
    // Create visualizers for all the examples on this page
    const examples = [
        { id: 'basicSineWaveCanvas', file: 'basic-sine.json' },
        { id: 'lfoCanvas', file: 'lfo.json' },
        { id: 'digitalSquareCanvas', file: 'digital-square.json' },
        { id: 'analogSquareCanvas', file: 'analog-square.json' },
        { id: 'simpleModulationCanvas', file: 'simple-modulation.json' },
        { id: 'multiOscCanvas', file: 'multi-osc.json' }
    ];
    
    examples.forEach(example => {
        const canvas = document.getElementById(example.id);
        if (canvas) {
            const visualizer = new WaveformVisualizer(example.id, {
                width: 800, height: 300
            });
            visualizer.loadData('/assets/samples/' + example.file);
        }
    });
});
</script>