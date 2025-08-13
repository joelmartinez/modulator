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

## Explore the Documentation

- [Understanding Modulation](/modulation-concepts) - Learn about modulation theory and applications
- [Using the Library](/library-usage) - Detailed guide to using the library classes and methods

The source code can be found here:  
https://github.com/joelmartinez/modulator
