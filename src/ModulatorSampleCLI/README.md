# Modulator Sample CLI

This command-line application demonstrates all the examples from the Modulator library documentation and provides JSON export functionality for waveform visualization.

## Usage

```bash
dotnet run --project ModulatorSampleCLI <example> [output_path]
```

- `<example>`: The name of the example to run (see list below)
- `[output_path]`: Optional path to save JSON waveform data for visualization

## Available Examples

### Basic Oscillators
- `basic-sine` - Basic 440Hz sine wave oscillator
- `lfo` - Low Frequency Oscillator (2.5Hz for modulation)
- `digital-square` - Clean digital square wave with instantaneous transitions
- `clock-signal` - Precise clock signal generation (10Hz, 3.3V logic)
- `analog-square` - Realistic analog square wave with smooth transitions
- `custom-analog` - Analog square wave with custom rise/fall times

### Modulation Examples
- `simple-modulation` - Basic carrier + modulator combination
- `vibrato` - Musical vibrato effect (6Hz modulation)
- `tremolo` - Amplitude modulation effect (8Hz tremolo)
- `complex-mod` - Complex soundscape with multiple modulation layers
- `chained-mod` - Multiple modulators chained together
- `modulation-networks` - Complex modulation networks with varying parameters

### Audio Synthesis
- `audio-square` - Audio synthesis with analog square wave and vibrato
- `simple-voice` - Simple synthesizer voice with vibrato and tremolo
- `multi-osc` - Multi-oscillator synthesizer combining different wave types
- `drum-kick` - Synthesized kick drum
- `drum-snare` - Synthesized snare drum

### Radio/Communications
- `am-radio` - AM radio modulation simulation
- `control-signals` - Control signal generation with frequency sweeping
- `signal-analysis` - Test signals for analysis purposes

### Test Signals
- `test-sweep` - Frequency sweep from 100Hz to 400Hz
- `test-square` - Comparison between digital and analog square waves

### Utility
- `list` - Display this help information

## Examples

### Generate and view a basic sine wave
```bash
dotnet run --project ModulatorSampleCLI basic-sine
```

### Export vibrato effect data for visualization
```bash
dotnet run --project ModulatorSampleCLI vibrato vibrato-data.json
```

### Generate multiple examples for documentation
```bash
dotnet run --project ModulatorSampleCLI basic-sine samples/basic-sine.json
dotnet run --project ModulatorSampleCLI digital-square samples/digital-square.json
dotnet run --project ModulatorSampleCLI analog-square samples/analog-square.json
dotnet run --project ModulatorSampleCLI vibrato samples/vibrato.json
```

## JSON Output Format

When an output path is specified, the CLI generates a JSON file with the following structure:

```json
{
  "Name": "Example Name",
  "Description": "Description of the example",
  "Duration": 1.0,
  "SampleRate": 1000.0,
  "Samples": [
    {
      "Time": 0.0,
      "Value": 0.0
    },
    {
      "Time": 0.001,
      "Value": 0.368125
    }
  ]
}
```

This format is designed for use with the waveform visualizer in the documentation.

## Integration with Documentation

All examples correspond directly to code snippets in the documentation:

- **docs/source/index.md** - Quick start examples
- **docs/source/library-usage/index.md** - Detailed usage examples
- **docs/source/modulation-concepts/index.md** - Conceptual examples

The generated JSON files can be used with the waveform visualizer to create interactive documentation with real waveform displays.