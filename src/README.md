# CodeCube.Modulator

A .NET 8 library for audio/signal modulation with support for oscillators and modulator chaining.

## Features

- **IModulationSource Interface**: Base interface for all modulation sources
- **SinOscillator**: Sine wave oscillator with configurable rate (frequency) and amplitude
- **Modulator Chaining**: Apply modulators to any source, including other oscillators
- **Composite Modulation**: Support for complex modulation scenarios

## Quick Start

### Basic Sine Oscillator

```csharp
using CodeCube.Modulator;

// Create a 440Hz sine oscillator (A4 note) with amplitude 0.8
var oscillator = new SinOscillator(440.0, 0.8);

// Get value at time 0.25 seconds
double value = oscillator.GetValue(0.25);
```

### Applying Modulation

```csharp
// Create base oscillator
var carrier = new SinOscillator(440.0, 0.8);

// Create modulator (vibrato effect)
var vibrato = new SinOscillator(6.0, 0.05);

// Apply modulation
var modulatedCarrier = carrier.ApplyModulator(vibrato);

// Get modulated value
double modulatedValue = modulatedCarrier.GetValue(1.0);
```

### Complex Modulation Chains

```csharp
// Create multiple oscillators
var baseOscillator = new SinOscillator(100.0, 1.0);
var modulator1 = new SinOscillator(10.0, 0.3);
var modulator2 = new SinOscillator(5.0, 0.1);

// Chain modulators
var result = baseOscillator
    .ApplyModulator(modulator1)
    .ApplyModulator(modulator2);

// The final output is the sum of all three oscillators
double complexValue = result.GetValue(2.0);
```

## API Reference

### IModulationSource

Base interface for all modulation sources.

- `double GetValue(double time)`: Gets the modulated value at the specified time
- `IModulationSource ApplyModulator(IModulationSource modulator)`: Applies a modulator to this source

### SinOscillator

Sine wave oscillator implementation.

- `SinOscillator(double rate, double amplitude)`: Constructor
- `double Rate`: Gets the frequency in Hz
- `double Amplitude`: Gets the amplitude (peak value)

### ModulatedSource

Represents a source with an applied modulator. Created automatically when calling `ApplyModulator()`.

- `IModulationSource BaseSource`: The original source
- `IModulationSource Modulator`: The applied modulator

## Testing

The library includes comprehensive unit tests and integration tests covering:

- Oscillator parameter validation
- Sine wave mathematical correctness
- Modulation chaining behavior
- Edge cases and error conditions

Run tests with:
```bash
dotnet test
```