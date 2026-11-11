---
template: Default
title: Modulator in TypeScript
subtitle: The TypeScript half of the Modulator library — dependency-free modulation for browsers and Node.
---

# Modulator in TypeScript

The Modulator library also ships as a TypeScript-era JavaScript package, `@codecube/modulator`: the same modulation ideas as the C# library on the rest of this site, in a small dependency-free ES module library that runs in browsers and in Node. A modulator produces a changing value over time — a waveform, a smooth noise signal, a fixed value, or a value read out of a per-frame context — and a binding writes that value into something else, such as a light's intensity, a filter's frequency, or a Tone.js-style parameter object.

The library imports nothing at all: no packages, no DOM, and no build step, so the files are consumed exactly as written. It was extracted from the frontend of codecube.net, where the modulation code was shared informally by the site's audio engine and its 3D pages, and it now lives in this repository under the `ts/` folder, next to the C# library.

## The quickstart, running live

The quickstart below is the first code most readers run, and the demo under it runs that same code against the real library on this page — the number is a binding's output, written by a `ModulationSystem` stepping a sine modulator in 0.05 second increments, not a picture of one.

```js
import { ModulationSystem } from '@codecube/modulator';

const modulation = new ModulationSystem();
modulation.createModulator('pulse', { type: 'sine', frequency: 1 });

const light = { intensity: 0 };
modulation.bind({ modulator: 'pulse', target: light, path: 'intensity', min: 200, max: 800 });

modulation.update(0.25);
console.log(light.intensity); // 800 — a quarter second in, the sine is at its peak
```

<div id="ts-quickstart-demo" class="live-example">
    <p>Live: <code>light.intensity</code> is <strong id="ts-quickstart-value">—</strong> at t = <span id="ts-quickstart-time">0.00</span> s (range 200–800, looping once per second).</p>
    <div style="background: rgba(255,255,255,0.08); height: 12px; max-width: 800px;"><div id="ts-quickstart-bar" style="background: #d4c389; height: 12px; width: 0%;"></div></div>
</div>

<script type="module" src="../assets/typescript/demos/quickstart-demo.js"></script>

## Explore the TypeScript documentation

- [Install](install/) — how to get the package and what it needs to run.
- [Modulation concepts](modulation-concepts/) — modulators, waveforms, bindings, and relative clocks, with a live waveform explorer.
- [Using the library](library-usage/) — the full API walkthrough, with a live binding demo.
