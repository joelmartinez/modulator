/**
 * Waveform Visualizer for Modulator Library Documentation
 * Renders waveform data from JSON files using HTML5 Canvas
 */

class WaveformVisualizer {
    constructor(canvasId, options = {}) {
        this.canvas = document.getElementById(canvasId);
        if (!this.canvas) {
            throw new Error(`Canvas element with id '${canvasId}' not found`);
        }
        
        this.ctx = this.canvas.getContext('2d');
        this.data = null;
        
        // Default options
        this.options = {
            width: 800,
            height: 300,
            backgroundColor: '#ffffff',
            waveformColor: '#0066cc',
            gridColor: '#e0e0e0',
            axisColor: '#333333',
            showGrid: true,
            showAxis: true,
            showLabels: true,
            marginTop: 20,
            marginBottom: 50,
            marginLeft: 60,
            marginRight: 20,
            lineWidth: 2,
            ...options
        };
        
        this.setupCanvas();
    }
    
    setupCanvas() {
        this.canvas.width = this.options.width;
        this.canvas.height = this.options.height;
        this.canvas.style.border = '1px solid #ccc';
        this.canvas.style.borderRadius = '4px';
    }
    
    async loadData(jsonUrl) {
        try {
            const response = await fetch(jsonUrl);
            if (!response.ok) {
                throw new Error(`Failed to load data: ${response.statusText}`);
            }
            this.data = await response.json();
            this.render();
        } catch (error) {
            this.renderError(`Error loading data: ${error.message}`);
        }
    }
    
    setData(data) {
        this.data = data;
        this.render();
    }
    
    renderError(message) {
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        this.ctx.fillStyle = '#ff0000';
        this.ctx.font = '16px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText(message, this.canvas.width / 2, this.canvas.height / 2);
    }
    
    render() {
        if (!this.data || !this.data.Samples || this.data.Samples.length === 0) {
            this.renderError('No data to display');
            return;
        }
        
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Calculate drawing area
        const drawArea = {
            left: this.options.marginLeft,
            top: this.options.marginTop,
            width: this.canvas.width - this.options.marginLeft - this.options.marginRight,
            height: this.canvas.height - this.options.marginTop - this.options.marginBottom
        };
        
        // Fill background
        this.ctx.fillStyle = this.options.backgroundColor;
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Find data bounds
        const minTime = this.data.Samples[0].Time;
        const maxTime = this.data.Samples[this.data.Samples.length - 1].Time;
        const values = this.data.Samples.map(s => s.Value);
        const minValue = Math.min(...values);
        const maxValue = Math.max(...values);
        
        // Add some padding to value range
        const valueRange = maxValue - minValue;
        const paddedMinValue = minValue - valueRange * 0.1;
        const paddedMaxValue = maxValue + valueRange * 0.1;
        
        // Draw grid
        if (this.options.showGrid) {
            this.drawGrid(drawArea, minTime, maxTime, paddedMinValue, paddedMaxValue);
        }
        
        // Draw axes
        if (this.options.showAxis) {
            this.drawAxes(drawArea, minTime, maxTime, paddedMinValue, paddedMaxValue);
        }
        
        // Draw waveform
        this.drawWaveform(drawArea, minTime, maxTime, paddedMinValue, paddedMaxValue);
        
        // Draw title and labels
        if (this.options.showLabels) {
            this.drawLabels(drawArea);
        }
    }
    
    drawGrid(drawArea, minTime, maxTime, minValue, maxValue) {
        this.ctx.strokeStyle = this.options.gridColor;
        this.ctx.lineWidth = 1;
        this.ctx.setLineDash([2, 2]);
        
        // Vertical grid lines (time)
        const timeStep = (maxTime - minTime) / 10;
        for (let i = 0; i <= 10; i++) {
            const time = minTime + i * timeStep;
            const x = drawArea.left + (time - minTime) / (maxTime - minTime) * drawArea.width;
            
            this.ctx.beginPath();
            this.ctx.moveTo(x, drawArea.top);
            this.ctx.lineTo(x, drawArea.top + drawArea.height);
            this.ctx.stroke();
        }
        
        // Horizontal grid lines (value)
        const valueStep = (maxValue - minValue) / 8;
        for (let i = 0; i <= 8; i++) {
            const value = minValue + i * valueStep;
            const y = drawArea.top + drawArea.height - (value - minValue) / (maxValue - minValue) * drawArea.height;
            
            this.ctx.beginPath();
            this.ctx.moveTo(drawArea.left, y);
            this.ctx.lineTo(drawArea.left + drawArea.width, y);
            this.ctx.stroke();
        }
        
        this.ctx.setLineDash([]);
    }
    
    drawAxes(drawArea, minTime, maxTime, minValue, maxValue) {
        this.ctx.strokeStyle = this.options.axisColor;
        this.ctx.lineWidth = 2;
        this.ctx.font = '12px Arial';
        this.ctx.fillStyle = this.options.axisColor;
        
        // X-axis
        this.ctx.beginPath();
        this.ctx.moveTo(drawArea.left, drawArea.top + drawArea.height);
        this.ctx.lineTo(drawArea.left + drawArea.width, drawArea.top + drawArea.height);
        this.ctx.stroke();
        
        // Y-axis
        this.ctx.beginPath();
        this.ctx.moveTo(drawArea.left, drawArea.top);
        this.ctx.lineTo(drawArea.left, drawArea.top + drawArea.height);
        this.ctx.stroke();
        
        // Time labels (X-axis)
        this.ctx.textAlign = 'center';
        const timeStep = (maxTime - minTime) / 5;
        for (let i = 0; i <= 5; i++) {
            const time = minTime + i * timeStep;
            const x = drawArea.left + (time - minTime) / (maxTime - minTime) * drawArea.width;
            const label = time.toFixed(2) + 's';
            this.ctx.fillText(label, x, drawArea.top + drawArea.height + 20);
        }
        
        // Value labels (Y-axis)
        this.ctx.textAlign = 'right';
        const valueStep = (maxValue - minValue) / 4;
        for (let i = 0; i <= 4; i++) {
            const value = minValue + i * valueStep;
            const y = drawArea.top + drawArea.height - (value - minValue) / (maxValue - minValue) * drawArea.height;
            const label = value.toFixed(2);
            this.ctx.fillText(label, drawArea.left - 10, y + 4);
        }
        
        // Axis labels
        this.ctx.textAlign = 'center';
        this.ctx.font = '14px Arial';
        this.ctx.fillText('Time (seconds)', drawArea.left + drawArea.width / 2, this.canvas.height - 10);
        
        this.ctx.save();
        this.ctx.translate(15, drawArea.top + drawArea.height / 2);
        this.ctx.rotate(-Math.PI / 2);
        this.ctx.fillText('Amplitude', 0, 0);
        this.ctx.restore();
    }
    
    drawWaveform(drawArea, minTime, maxTime, minValue, maxValue) {
        this.ctx.strokeStyle = this.options.waveformColor;
        this.ctx.lineWidth = this.options.lineWidth;
        this.ctx.lineJoin = 'round';
        this.ctx.lineCap = 'round';
        
        this.ctx.beginPath();
        
        let firstPoint = true;
        for (const sample of this.data.Samples) {
            const x = drawArea.left + (sample.Time - minTime) / (maxTime - minTime) * drawArea.width;
            const y = drawArea.top + drawArea.height - (sample.Value - minValue) / (maxValue - minValue) * drawArea.height;
            
            if (firstPoint) {
                this.ctx.moveTo(x, y);
                firstPoint = false;
            } else {
                this.ctx.lineTo(x, y);
            }
        }
        
        this.ctx.stroke();
    }
    
    drawLabels(drawArea) {
        // Title
        this.ctx.font = 'bold 16px Arial';
        this.ctx.fillStyle = this.options.axisColor;
        this.ctx.textAlign = 'center';
        this.ctx.fillText(this.data.Name, this.canvas.width / 2, 16);
        
        // Description
        if (this.data.Description) {
            this.ctx.font = '12px Arial';
            this.ctx.fillStyle = '#666666';
            this.ctx.fillText(this.data.Description, this.canvas.width / 2, drawArea.top - 5);
        }
        
        // Info text
        this.ctx.font = '10px Arial';
        this.ctx.textAlign = 'right';
        const info = `Duration: ${this.data.Duration}s, Sample Rate: ${this.data.SampleRate}Hz, Samples: ${this.data.Samples.length}`;
        this.ctx.fillText(info, this.canvas.width - 10, this.canvas.height - 5);
    }
}

// Utility function to create a visualizer with a simple interface
function createWaveformVisualizer(canvasId, jsonUrl, options = {}) {
    const visualizer = new WaveformVisualizer(canvasId, options);
    if (jsonUrl) {
        visualizer.loadData(jsonUrl);
    }
    return visualizer;
}

// Export for use in modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { WaveformVisualizer, createWaveformVisualizer };
}

// Make available globally
window.WaveformVisualizer = WaveformVisualizer;
window.createWaveformVisualizer = createWaveformVisualizer;