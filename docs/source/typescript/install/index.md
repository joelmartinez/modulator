---
template: Default
title: Install — Modulator in TypeScript
subtitle: Getting the @codecube/modulator package and running the quickstart.
---

# Install

The TypeScript package is `@codecube/modulator`, version 0.1.0, and it lives in this repository under the `ts/` folder. It has no dependencies and no build step: the published files are plain ES modules with explicit `.js` import extensions, so what you install is exactly the source you can read in the repository.

Honest status, stated plainly: the package is not on npm yet, and npm publishing happens only when the owner decides it does. Until then, the supported routes are the ones below — a folder dependency for local work, which is how the CodeCube projects consume sibling libraries while they are pre-publish, and the repository itself as the source of truth.

## Folder install (available now)

Copy or check out this repository beside your project, then point your `package.json` at the `ts/` folder inside it:

```json
{
  "dependencies": {
    "@codecube/modulator": "file:../modulator/ts"
  }
}
```

Run `npm install` in your project, and the package is available under its real name, so imports read exactly as they will after npm publishing:

```js
import { ModulationSystem } from '@codecube/modulator';
```

## What it needs

The package needs Node 18 or newer for Node-side use, or any browser with ES module support for browser use, and nothing else — there are no packages to fetch for the library itself, and no bundler, transpiler, or build step is required on either side. In a browser page without a bundler, load the module files directly (an import map can give them the package name), which is exactly how the live examples on this site run the library.

## The quickstart

Install the package, then import it as an ES module:

```js
import { ModulationSystem } from '@codecube/modulator';

const modulation = new ModulationSystem();
modulation.createModulator('pulse', { type: 'sine', frequency: 1 });

const light = { intensity: 0 };
modulation.bind({ modulator: 'pulse', target: light, path: 'intensity', min: 200, max: 800 });

modulation.update(0.25);
console.log(light.intensity); // 800 — a quarter second in, the sine is at its peak
```

The demo below runs that same quickstart against the real library on this page, stepping time in 0.05 second increments and looping once per second, so the value a reader would see in their console at the quarter-second mark — 800, the top of the binding's range — sweeps past twice per loop.

<div id="ts-quickstart-demo" class="live-example">
    <p>Live: <code>light.intensity</code> is <strong id="ts-quickstart-value">—</strong> at t = <span id="ts-quickstart-time">0.00</span> s (range 200–800, looping once per second).</p>
    <div style="background: rgba(255,255,255,0.08); height: 12px; max-width: 800px;"><div id="ts-quickstart-bar" style="background: #d4c389; height: 12px; width: 0%;"></div></div>
</div>

<script type="module" src="../../assets/typescript/demos/quickstart-demo.js"></script>

Next: [modulation concepts](../modulation-concepts/) explains what modulators and bindings are, and [using the library](../library-usage/) walks the whole API.
