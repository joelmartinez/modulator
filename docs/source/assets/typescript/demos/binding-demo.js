// Live example for the TypeScript using-the-library page. It runs the real
// library: one ModulationSystem advances a sine modulator, and two bindings
// write its value into a plain property and a Tone-style { value } parameter.
import { ModulationSystem } from '../modulator/index.js';

const container = document.getElementById('ts-binding-demo');
const lightValue = document.getElementById('ts-binding-light-value');
const lightBar = document.getElementById('ts-binding-light-bar');
const filterValue = document.getElementById('ts-binding-filter-value');

if (container && lightValue && lightBar && filterValue) {
  const modulation = new ModulationSystem();
  modulation.createModulator('pulse', { type: 'sine', frequency: 0.5 });

  const light = { intensity: 0 };
  modulation.bind({ modulator: 'pulse', target: light, path: 'intensity', min: 200, max: 800 });

  const filter = { frequency: { value: 400 } };
  modulation.bind({ modulator: 'pulse', target: filter, path: 'frequency', min: 400, max: 1200, amount: 0.5 });

  let lastTimestamp = null;
  function tick(timestamp) {
    if (lastTimestamp === null) {
      lastTimestamp = timestamp;
    }
    const deltaSeconds = Math.min((timestamp - lastTimestamp) / 1000, 0.05);
    lastTimestamp = timestamp;

    modulation.update(deltaSeconds);
    lightValue.textContent = light.intensity.toFixed(0);
    lightBar.style.width = `${((light.intensity - 200) / 600) * 100}%`;
    filterValue.textContent = filter.frequency.value.toFixed(0);
    container.dataset.liveStatus = 'ok';
    container.dataset.lightIntensity = light.intensity.toFixed(0);
    container.dataset.filterFrequency = filter.frequency.value.toFixed(0);
    requestAnimationFrame(tick);
  }
  requestAnimationFrame(tick);
}
