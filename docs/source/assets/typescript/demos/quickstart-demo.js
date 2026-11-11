// Live example for the TypeScript install and section pages. It runs the
// README quickstart against the real library: a sine modulator bound to a
// light's intensity, stepped in 0.05 s increments and looped once per second.
import { ModulationSystem } from '../modulator/index.js';

const container = document.getElementById('ts-quickstart-demo');
const valueOut = document.getElementById('ts-quickstart-value');
const timeOut = document.getElementById('ts-quickstart-time');
const bar = document.getElementById('ts-quickstart-bar');

if (container && valueOut && timeOut && bar) {
  const modulation = new ModulationSystem();
  modulation.createModulator('pulse', { type: 'sine', frequency: 1 });

  const light = { intensity: 0 };
  modulation.bind({ modulator: 'pulse', target: light, path: 'intensity', min: 200, max: 800 });

  function step() {
    modulation.update(0.05);
    const elapsed = modulation.getState().time;
    valueOut.textContent = light.intensity.toFixed(0);
    timeOut.textContent = elapsed.toFixed(2);
    bar.style.width = `${((light.intensity - 200) / 600) * 100}%`;
    container.dataset.liveStatus = 'ok';
    container.dataset.lightIntensity = light.intensity.toFixed(0);
    if (elapsed >= 0.999) {
      modulation.reset();
    }
  }

  step();
  setInterval(step, 120);
}
