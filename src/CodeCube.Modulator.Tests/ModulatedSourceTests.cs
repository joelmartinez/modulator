namespace CodeCube.Modulator.Tests;

public class ModulatedSourceTests
{
    [Fact]
    public void Constructor_WithValidSources_SetsProperties()
    {
        // Arrange
        var baseSource = new SinOscillator(1.0, 1.0);
        var modulator = new SinOscillator(2.0, 0.5);

        // Act
        var modulatedSource = new ModulatedSource(baseSource, modulator);

        // Assert
        Assert.Equal(baseSource, modulatedSource.BaseSource);
        Assert.Equal(modulator, modulatedSource.Modulator);
    }

    [Fact]
    public void Constructor_WithNullBaseSource_ThrowsArgumentNullException()
    {
        // Arrange
        var modulator = new SinOscillator(2.0, 0.5);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ModulatedSource(null!, modulator));
    }

    [Fact]
    public void Constructor_WithNullModulator_ThrowsArgumentNullException()
    {
        // Arrange
        var baseSource = new SinOscillator(1.0, 1.0);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ModulatedSource(baseSource, null!));
    }

    [Fact]
    public void GetValue_AddsBaseSourceAndModulatorValues()
    {
        // Arrange
        var baseSource = new SinOscillator(1.0, 2.0); // 2Hz, amplitude 2
        var modulator = new SinOscillator(1.0, 0.5);  // 1Hz, amplitude 0.5
        var modulatedSource = new ModulatedSource(baseSource, modulator);
        double time = 0.25; // Quarter period for 1Hz

        // Act
        double modulatedValue = modulatedSource.GetValue(time);
        double expectedBaseValue = baseSource.GetValue(time);
        double expectedModulatorValue = modulator.GetValue(time);

        // Assert
        Assert.Equal(expectedBaseValue + expectedModulatorValue, modulatedValue, precision: 10);
    }

    [Fact]
    public void ApplyModulator_CreatesNestedModulatedSource()
    {
        // Arrange
        var baseSource = new SinOscillator(1.0, 1.0);
        var firstModulator = new SinOscillator(2.0, 0.5);
        var secondModulator = new SinOscillator(3.0, 0.25);
        var firstModulated = new ModulatedSource(baseSource, firstModulator);

        // Act
        var doubleModulated = firstModulated.ApplyModulator(secondModulator);

        // Assert
        Assert.IsType<ModulatedSource>(doubleModulated);
        var doubleModulatedSource = doubleModulated as ModulatedSource;
        Assert.Equal(firstModulated, doubleModulatedSource!.BaseSource);
        Assert.Equal(secondModulator, doubleModulatedSource.Modulator);
    }

    [Fact]
    public void GetValue_WithMultipleModulators_ComputesCorrectly()
    {
        // Arrange
        var baseOscillator = new SinOscillator(1.0, 1.0);
        var modulator1 = new SinOscillator(2.0, 0.3);
        var modulator2 = new SinOscillator(3.0, 0.2);
        
        var singleModulated = baseOscillator.ApplyModulator(modulator1);
        var doubleModulated = singleModulated.ApplyModulator(modulator2);
        
        double time = 0.1;

        // Act
        double doubleModulatedValue = doubleModulated.GetValue(time);
        
        // Calculate expected value manually
        double baseValue = baseOscillator.GetValue(time);
        double mod1Value = modulator1.GetValue(time);
        double mod2Value = modulator2.GetValue(time);
        double expectedValue = baseValue + mod1Value + mod2Value;

        // Assert
        Assert.Equal(expectedValue, doubleModulatedValue, precision: 10);
    }
}