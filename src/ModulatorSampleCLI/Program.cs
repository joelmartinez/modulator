using CodeCube.Modulator;
using System.Text.Json;

namespace ModulatorSampleCLI;

/// <summary>
/// Data model for waveform sample data export
/// </summary>
public class WaveformData
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Duration { get; set; }
    public double SampleRate { get; set; }
    public List<WaveformSample> Samples { get; set; } = new();
}

/// <summary>
/// Individual sample point in a waveform
/// </summary>
public class WaveformSample
{
    public double Time { get; set; }
    public double Value { get; set; }
}

/// <summary>
/// Main CLI application for demonstrating the Modulator library
/// </summary>
class Program
{
    private const double DefaultDuration = 1.0; // 1 second
    private const double DefaultSampleRate = 1000.0; // 1000 samples per second for visualization
    
    static void Main(string[] args)
    {
        Console.WriteLine("Modulator Library Sample CLI");
        Console.WriteLine("============================");
        
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }
        
        string command = args[0].ToLower();
        string? outputPath = args.Length > 1 ? args[1] : null;
        
        try
        {
            RunExample(command, outputPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
    
    static void ShowHelp()
    {
        Console.WriteLine("Usage: ModulatorSampleCLI <example> [output_path]");
        Console.WriteLine("");
        Console.WriteLine("Available examples:");
        Console.WriteLine("  basic-sine         - Basic sine wave oscillator");
        Console.WriteLine("  lfo               - Low Frequency Oscillator");
        Console.WriteLine("  digital-square    - Digital square wave");
        Console.WriteLine("  clock-signal      - Clock signal generation");
        Console.WriteLine("  analog-square     - Analog square wave");
        Console.WriteLine("  custom-analog     - Custom rise/fall analog square");
        Console.WriteLine("  audio-square      - Audio synthesis with analog square");
        Console.WriteLine("  simple-modulation - Simple modulation example");
        Console.WriteLine("  vibrato           - Vibrato effect");
        Console.WriteLine("  tremolo           - Tremolo effect");
        Console.WriteLine("  complex-mod       - Complex soundscape");
        Console.WriteLine("  chained-mod       - Chaining multiple modulators");
        Console.WriteLine("  modulation-networks - Creating modulation networks");
        Console.WriteLine("  am-radio          - AM radio simulation");
        Console.WriteLine("  control-signals   - Control signal generation");
        Console.WriteLine("  signal-analysis   - Signal analysis");
        Console.WriteLine("  simple-voice      - Simple synthesizer voice");
        Console.WriteLine("  multi-osc         - Multi-oscillator synthesizer");
        Console.WriteLine("  drum-kick         - Drum synthesis - kick");
        Console.WriteLine("  drum-snare        - Drum synthesis - snare");
        Console.WriteLine("  test-sweep        - Test signal sweep");
        Console.WriteLine("  test-square       - Square wave test comparison");
        Console.WriteLine("  list              - List all examples");
        Console.WriteLine("");
        Console.WriteLine("If output_path is specified, waveform data will be saved as JSON.");
    }
    
    static void RunExample(string command, string? outputPath)
    {
        if (command == "list")
        {
            ShowHelp();
            return;
        }
        
        WaveformData? data = command switch
        {
            "basic-sine" => BasicSineExample(),
            "lfo" => LFOExample(),
            "digital-square" => DigitalSquareExample(),
            "clock-signal" => ClockSignalExample(),
            "analog-square" => AnalogSquareExample(),
            "custom-analog" => CustomAnalogExample(),
            "audio-square" => AudioSquareExample(),
            "simple-modulation" => SimpleModulationExample(),
            "vibrato" => VibratoExample(),
            "tremolo" => TremoloExample(),
            "complex-mod" => ComplexSoundscapeExample(),
            "chained-mod" => ChainedModeExample(),
            "modulation-networks" => ModulationNetworksExample(),
            "am-radio" => AMRadioExample(),
            "control-signals" => ControlSignalsExample(),
            "signal-analysis" => SignalAnalysisExample(),
            "simple-voice" => SimpleVoiceExample(),
            "multi-osc" => MultiOscExample(),
            "drum-kick" => DrumKickExample(),
            "drum-snare" => DrumSnareExample(),
            "test-sweep" => TestSweepExample(),
            "test-square" => TestSquareExample(),
            _ => throw new ArgumentException($"Unknown example: {command}")
        };
        
        if (data == null) return;
        
        Console.WriteLine($"Generated {data.Name}");
        Console.WriteLine($"Description: {data.Description}");
        Console.WriteLine($"Duration: {data.Duration}s, Samples: {data.Samples.Count}");
        
        if (outputPath != null)
        {
            SaveToJson(data, outputPath);
            Console.WriteLine($"Data saved to: {outputPath}");
        }
        else
        {
            // Show first few samples
            Console.WriteLine("Sample values (first 10):");
            for (int i = 0; i < Math.Min(10, data.Samples.Count); i++)
            {
                var sample = data.Samples[i];
                Console.WriteLine($"  t={sample.Time:F3}s: {sample.Value:F6}");
            }
        }
    }
    
    static void SaveToJson(WaveformData data, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        string json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }
    
    static WaveformData GenerateWaveformData(IModulationSource source, string name, string description, 
                                           double duration = DefaultDuration, double sampleRate = DefaultSampleRate)
    {
        var data = new WaveformData
        {
            Name = name,
            Description = description,
            Duration = duration,
            SampleRate = sampleRate
        };
        
        int sampleCount = (int)(duration * sampleRate);
        for (int i = 0; i < sampleCount; i++)
        {
            double time = (double)i / sampleRate;
            double value = source.GetValue(time);
            data.Samples.Add(new WaveformSample { Time = time, Value = value });
        }
        
        return data;
    }
    
    // Basic Oscillator Examples
    static WaveformData BasicSineExample()
    {
        // Create a 440Hz sine wave with amplitude 1.0
        var oscillator = new SinOscillator(440, 1.0);
        
        return GenerateWaveformData(oscillator, "Basic Sine Wave", 
            "A simple 440Hz sine wave with amplitude 1.0");
    }
    
    static WaveformData LFOExample()
    {
        // Create a slow oscillator for modulation purposes
        var lfo = new SinOscillator(2.5, 0.5); // 2.5Hz at 50% amplitude
        
        return GenerateWaveformData(lfo, "Low Frequency Oscillator", 
            "A 2.5Hz LFO with 50% amplitude for modulation purposes", 2.0);
    }
    
    static WaveformData DigitalSquareExample()
    {
        // Create a 1kHz digital square wave
        var digitalSquare = new DigitalSquareOscillator(100, 1.0);
        
        return GenerateWaveformData(digitalSquare, "Digital Square Wave", 
            "A clean 100Hz digital square wave with instantaneous transitions", 0.1);
    }
    
    static WaveformData ClockSignalExample()
    {
        // Generate a precise clock signal
        var clockSignal = new DigitalSquareOscillator(10, 3.3); // 10Hz, 3.3V logic level
        
        return GenerateWaveformData(clockSignal, "Clock Signal", 
            "A 10Hz clock signal at 3.3V logic level", 1.0);
    }
    
    static WaveformData AnalogSquareExample()
    {
        // Create an analog square wave with default rise/fall times
        var analogSquare = new AnalogSquareOscillator(100, 1.0);
        
        return GenerateWaveformData(analogSquare, "Analog Square Wave", 
            "A 100Hz analog square wave with realistic transitions and characteristics", 0.1);
    }
    
    static WaveformData CustomAnalogExample()
    {
        // Create an analog square wave with slower transitions
        var slowAnalog = new AnalogSquareOscillator(50, 1.0, 0.05, 0.08);
        // 5% rise time, 8% fall time (as fraction of period)
        
        return GenerateWaveformData(slowAnalog, "Custom Analog Square", 
            "A 50Hz analog square wave with custom rise (5%) and fall (8%) times", 0.2);
    }
    
    static WaveformData AudioSquareExample()
    {
        // Square wave for audio synthesis with natural analog characteristics
        var audioSquare = new AnalogSquareOscillator(220, 0.8, 0.02, 0.03);
        
        // Apply some vibrato
        var vibrato = new SinOscillator(5, 0.05);
        var expressiveSquare = audioSquare.ApplyModulator(vibrato);
        
        return GenerateWaveformData(expressiveSquare, "Audio Square with Vibrato", 
            "A 220Hz analog square wave with 5Hz vibrato for audio synthesis", 2.0);
    }
    
    // Modulation Examples
    static WaveformData SimpleModulationExample()
    {
        var carrier = new SinOscillator(440, 1.0);     // 440Hz carrier
        var modulator = new SinOscillator(5, 0.2);     // 5Hz modulator with 0.2 amplitude
        
        var modulated = carrier.ApplyModulator(modulator);
        
        return GenerateWaveformData(modulated, "Simple Modulation", 
            "440Hz sine wave modulated by 5Hz oscillator", 2.0);
    }
    
    static WaveformData VibratoExample()
    {
        // 440Hz A note (carrier)
        var note = new SinOscillator(440, 1.0);
        
        // 6Hz vibrato modulation
        var vibrato = new SinOscillator(6, 5); // 5Hz frequency deviation
        
        var vibratoNote = note.ApplyModulator(vibrato);
        
        return GenerateWaveformData(vibratoNote, "Vibrato Effect", 
            "440Hz note with 6Hz vibrato effect", 2.0);
    }
    
    static WaveformData TremoloExample()
    {
        // Base signal
        var carrier = new SinOscillator(440, 1.0);
        
        // Tremolo modulation (amplitude varies)
        var tremolo = new SinOscillator(8, 0.3); // 30% amplitude modulation at 8Hz
        
        var tremoloSignal = carrier.ApplyModulator(tremolo);
        
        return GenerateWaveformData(tremoloSignal, "Tremolo Effect", 
            "440Hz sine wave with 8Hz tremolo (amplitude modulation)", 2.0);
    }
    
    static WaveformData ComplexSoundscapeExample()
    {
        var baseSignal = new SinOscillator(110, 1.0);
        var mod1 = new SinOscillator(0.5, 10);  // Slow, deep modulation
        var mod2 = new SinOscillator(7, 2);     // Faster, lighter modulation
        
        var complex = baseSignal.ApplyModulator(mod1).ApplyModulator(mod2);
        
        return GenerateWaveformData(complex, "Complex Soundscape", 
            "110Hz base with multiple modulation layers creating evolving texture", 4.0);
    }
    
    static WaveformData ChainedModeExample()
    {
        var baseOsc = new SinOscillator(440, 1.0);
        var mod1 = new SinOscillator(5, 0.1);      // Slow vibrato
        var mod2 = new SinOscillator(0.5, 2);      // Very slow, deep modulation
        
        // Chain modulators
        var complex = baseOsc
            .ApplyModulator(mod1)
            .ApplyModulator(mod2);
        
        return GenerateWaveformData(complex, "Chained Modulators", 
            "440Hz sine with chained modulation (base + mod1 + mod2)", 4.0);
    }
    
    static WaveformData ModulationNetworksExample()
    {
        // Create a vibrato that itself is modulated
        var vibratoRate = new SinOscillator(6, 1.0);          // 6Hz vibrato
        var vibratoDepth = new SinOscillator(0.2, 0.05);      // Varying depth
        var vibrato = vibratoRate.ApplyModulator(vibratoDepth);
        
        // Apply the complex vibrato to a carrier
        var note = new SinOscillator(440, 1.0);
        var expressiveNote = note.ApplyModulator(vibrato);
        
        return GenerateWaveformData(expressiveNote, "Modulation Networks", 
            "Complex vibrato with varying depth applied to 440Hz carrier", 4.0);
    }
    
    // Radio Communication Examples
    static WaveformData AMRadioExample()
    {
        // Audio signal (1 kHz tone)
        var audio = new SinOscillator(100, 0.5);  // Using 100Hz for visualization
        
        // RF carrier (simulated as lower frequency for example)
        var rfCarrier = new SinOscillator(1000, 1.0);
        
        // AM modulation (conceptual - actual AM requires multiplication)
        var amSignal = rfCarrier.ApplyModulator(audio);
        
        return GenerateWaveformData(amSignal, "AM Radio Simulation", 
            "1000Hz RF carrier modulated by 100Hz audio signal", 0.1);
    }
    
    static WaveformData ControlSignalsExample()
    {
        // Generate a swept frequency signal for testing
        var baseFreq = new SinOscillator(100, 1.0);  // 100Hz base for visualization
        var sweep = new SinOscillator(0.5, 50); // Slow sweep ±50Hz
        
        var sweptSignal = baseFreq.ApplyModulator(sweep);
        
        return GenerateWaveformData(sweptSignal, "Control Signals", 
            "100Hz signal with 0.5Hz sweep modulation (±50Hz)", 4.0);
    }
    
    static WaveformData SignalAnalysisExample()
    {
        // Test signal with known modulation characteristics
        var testCarrier = new SinOscillator(500, 1.0);
        var testMod = new SinOscillator(25, 0.2); // 25Hz modulation at 20%
        
        var testSignal = testCarrier.ApplyModulator(testMod);
        
        return GenerateWaveformData(testSignal, "Signal Analysis", 
            "500Hz test signal with 25Hz modulation for analysis purposes", 0.2);
    }
    
    // Synthesizer Examples  
    static WaveformData SimpleVoiceExample()
    {
        var carrier = new SinOscillator(440, 0.8);
        var vibrato = new SinOscillator(5, 440 * 0.01); // 1% vibrato
        var tremolo = new SinOscillator(3, 0.1);         // 10% tremolo
        
        var voice = carrier.ApplyModulator(vibrato).ApplyModulator(tremolo);
        
        return GenerateWaveformData(voice, "Simple Synthesizer Voice", 
            "440Hz voice with vibrato (5Hz, 1%) and tremolo (3Hz, 10%)", 3.0);
    }
    
    static WaveformData MultiOscExample()
    {
        // Main oscillators at the same frequency
        var sineOsc = new SinOscillator(220, 0.3);
        var digitalSquare = new DigitalSquareOscillator(220, 0.4);
        var analogSquare = new AnalogSquareOscillator(220, 0.3, 0.03, 0.04);
        
        // Sub-oscillator one octave down
        var subOsc = new AnalogSquareOscillator(110, 0.2);
        
        // Combine all oscillators
        var combinedSignal = sineOsc
            .ApplyModulator(digitalSquare)
            .ApplyModulator(analogSquare)
            .ApplyModulator(subOsc);
        
        return GenerateWaveformData(combinedSignal, "Multi-Oscillator Synthesizer", 
            "Combined sine, digital square, analog square oscillators with sub-oscillator", 2.0);
    }
    
    // Drum Synthesis Examples
    static WaveformData DrumKickExample()
    {
        // Use analog square for body, digital square for click
        var body = new AnalogSquareOscillator(60, 1.0, 0.1, 0.2);
        var click = new DigitalSquareOscillator(240, 0.3);
        
        // Pitch envelope - frequency sweeps down quickly
        var pitchEnv = new SinOscillator(2, 30); // Faster sweep for kick
        
        var kick = body.ApplyModulator(click).ApplyModulator(pitchEnv);
        
        return GenerateWaveformData(kick, "Drum Kick Synthesis", 
            "Synthesized kick drum using analog square body with digital click", 1.0);
    }
    
    static WaveformData DrumSnareExample()
    {
        // Digital square for the sharp attack
        var tone = new DigitalSquareOscillator(200, 0.6);
        
        // High frequency modulation for noise-like character
        var noise = new AnalogSquareOscillator(1400, 0.4, 0.005, 0.005);
        
        var snare = tone.ApplyModulator(noise);
        
        return GenerateWaveformData(snare, "Drum Snare Synthesis", 
            "Synthesized snare drum with digital tone and analog noise", 0.5);
    }
    
    // Test Signal Examples
    static WaveformData TestSweepExample()
    {
        double startFreq = 100;
        double endFreq = 400;
        double duration = 2.0;
        
        double sweepRate = (endFreq - startFreq) / duration;
        var baseOsc = new SinOscillator(startFreq, 1.0);
        var sweepMod = new SinOscillator(1.0 / duration, sweepRate);
        
        var sweep = baseOsc.ApplyModulator(sweepMod);
        
        return GenerateWaveformData(sweep, "Test Signal Sweep", 
            $"Frequency sweep from {startFreq}Hz to {endFreq}Hz over {duration}s", duration);
    }
    
    static WaveformData TestSquareExample()
    {
        // Compare digital vs analog square waves
        var digital = new DigitalSquareOscillator(100, 0.5);
        var analog = new AnalogSquareOscillator(100, 0.5);
        
        // Modulate between them slowly
        var selector = new SinOscillator(0.25, 0.25); // Very slow fade
        
        var comparison = digital.ApplyModulator(analog).ApplyModulator(selector);
        
        return GenerateWaveformData(comparison, "Square Wave Comparison", 
            "Digital vs analog square wave comparison with slow crossfade", 4.0);
    }
}
