---
template: Default
title: Modulator Library
subtitle: A C# library for generating and manipulating modulated waveforms.
---

# Welcome to the Modulator Library!

The **Modulator** library is a C# .NET library designed for generating and manipulating modulated waveforms. Whether you're working on audio synthesis, signal processing, or communications applications, this library provides the fundamental building blocks for creating complex modulated signals.

## What is Modulation?

Modulation is the process of varying one signal (the carrier) based on another signal (the modulator). This technique is fundamental to many applications:

- **Audio Synthesis**: Creating rich, dynamic sounds by modulating oscillators
- **Radio Communications**: Encoding information onto carrier waves for transmission
- **Signal Processing**: Creating complex waveforms for various applications

## Key Features

- **Simple Interface**: Clean, intuitive API with the `IModulationSource` interface
- **Composable Design**: Easily chain and combine multiple modulators
- **High Performance**: Efficient mathematical operations for real-time applications
- **Extensible**: Build custom modulation sources by implementing the interface

## Installation

Install the Modulator library via NuGet:

```bash
dotnet add package CodeCube.Modulator
```

Or use the NuGet Package Manager in Visual Studio:

```
Install-Package CodeCube.Modulator
```

You can also browse and download the package from [NuGet.org](https://www.nuget.org/packages/CodeCube.Modulator).

## Quick Start

```csharp
using CodeCube.Modulator;

// Create a 440Hz sine wave oscillator
var carrier = new SinOscillator(440, 1.0);

// Create a 5Hz modulation oscillator for vibrato
var modulator = new SinOscillator(5, 0.1);

// Apply the modulation
var modulatedSignal = carrier.ApplyModulator(modulator);

// Or try the new square wave oscillators
var digitalSquare = new DigitalSquareOscillator(220, 1.0);     // Clean digital square wave
var analogSquare = new AnalogSquareOscillator(220, 1.0);      // Realistic analog square wave

// Get values over time
for (double time = 0; time < 1.0; time += 0.01)
{
    double sineValue = modulatedSignal.GetValue(time);
    double digitalValue = digitalSquare.GetValue(time);
    double analogValue = analogSquare.GetValue(time);
    
    Console.WriteLine($"Time: {time:F2}, Sine: {sineValue:F3}, Digital: {digitalValue:F3}, Analog: {analogValue:F3}");
}
```

### Visualizations

You can generate waveform data and visualize these examples using the sample CLI:

```bash
# Generate data for visualization
dotnet run --project ModulatorSampleCLI basic-sine output.json
```

<div class="waveform-demo">
    <h4>Basic Sine Wave (440Hz)</h4>
    <canvas id="basicSineCanvas" class="waveform-visualization" data-sample="basic-sine.json" width="800" height="300"></canvas>
    
    <h4>Simple Modulation (440Hz carrier + 5Hz modulator)</h4>
    <canvas id="simpleModCanvas" class="waveform-visualization" data-sample="simple-modulation.json" width="800" height="300"></canvas>
</div>



## Explore the Documentation

- [Understanding Modulation](/modulation-concepts) - Learn about modulation theory and applications
- [Using the Library](/library-usage) - Detailed guide to using the library classes and methods

The source code can be found here:  
https://github.com/joelmartinez/modulator
