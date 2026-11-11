// Live example for the TypeScript modulation concepts page. It runs the real
// library: every point on the curve is sampled from a Modulator created
// through a ModulationSystem, the same classes the page documents.
import { ModulationSystem, Modulator } from '../modulator/index.js';

const canvas = document.getElementById('ts-waveform-canvas');
const typeSelect = document.getElementById('ts-waveform-type');
const frequencyInput = document.getElementById('ts-waveform-frequency');
const readout = document.getElementById('ts-waveform-readout');

if (canvas && typeSelect && frequencyInput && readout) {
  const context = canvas.getContext('2d');
  const width = canvas.width;
  const height = canvas.height;
  const secondsShown = 1;

  function draw() {
    const type = typeSelect.value;
    const frequency = Number(frequencyInput.value);
    const system = new ModulationSystem();
    const modulator = new Modulator('explorer', { type, frequency });

    context.clearRect(0, 0, width, height);
    context.strokeStyle = 'rgba(255, 255, 255, 0.25)';
    context.beginPath();
    context.moveTo(0, height / 2);
    context.lineTo(width, height / 2);
    context.stroke();

    context.strokeStyle = '#d4c389';
    context.lineWidth = 2;
    context.beginPath();
    for (let x = 0; x <= width; x += 1) {
      const time = (x / width) * secondsShown;
      const value = modulator.sample({ time, deltaTime: 0, frame: 0, context: {} }, system);
      const y = (height / 2) - (value * (height / 2 - 8));
      if (x === 0) {
        context.moveTo(x, y);
      } else {
        context.lineTo(x, y);
      }
    }
    context.stroke();

    const quarter = modulator.sample({ time: 0.25, deltaTime: 0, frame: 0, context: {} }, system);
    readout.textContent = `${type} at ${frequency} Hz — value at 0.25 s: ${quarter.toFixed(3)}`;
    canvas.dataset.liveStatus = 'ok';
    canvas.dataset.liveType = type;
  }

  typeSelect.addEventListener('change', draw);
  frequencyInput.addEventListener('input', draw);
  draw();
}
