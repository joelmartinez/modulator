---
template: Default
title: Using the Library — TypeScript
subtitle: Complete guide to the @codecube/modulator classes, bindings, and contracts.
---

# Using the Library (TypeScript)

Everything in the package is imported from its single entry point, and named imports cover the whole public surface — `ModulationSystem`, `Modulator`, `ModulationBinding`, the combined `Modulation` namespace object, and the helpers `clamp`, `lerp`, `normalizeToUnit`, `readPath`, `distance3D`, `createFrameContext`, and its compatibility alias `createPhotonicFrameContext`:

```js
import { ModulationSystem, createFrameContext, distance3D } from '@codecube/modulator';
```

## The system

One `ModulationSystem` owns a set of modulators and bindings and advances them together. Its main methods are `createModulator(id, options)`, `getModulator(id)`, and `removeModulator(id)` for modulators; `bind(options)`, `getBinding(id)`, and `removeBinding(id)` for bindings; `update(deltaTime, context)` to advance one frame or tick, which samples every modulator and then applies every binding; `sample(id)` to read one modulator's current value; `getState()` and `getValues()` to inspect the frame state and the latest sampled values; and `reset()` and `dispose()` to rewind the clock or tear everything down. Time only moves when `update` is called, so a host can drive the system from a render loop, an audio callback, a test, or a plain timer with equal ease.

## Modulator options

A modulator's options object chooses its kind and shape: `type` is one of `sine`, `triangle`, `square`, `sawtooth`, `ramp`, `noise`, `value`, or `context`; `frequency`, `phase`, `amplitude`, and `offset` shape the signal; `range` maps the output onto a numeric range and `clamp` bounds it. A `context` modulator reads from the object passed to `update`, by dotted `path` or by a `selector` function, with `fallback` when the value is missing. A `getValue` function can take over a modulator's output entirely, a `transform` function can reshape any modulator's output, and `clockRef` with `divisor` or `multiplier` runs the modulator relative to another modulator's rate, as the [concepts page](../modulation-concepts/) explains.

```js
modulation.createModulator('swell', { type: 'sine', frequency: 0.25 });
modulation.createModulator('shimmer', { type: 'triangle', clockRef: 'swell', multiplier: 4 });
modulation.createModulator('gate', {
  type: 'context',
  path: 'camera.position',
  selector: (frame) => distance3D(frame.context.camera.position),
  range: [0, 1]
});
```

## Bindings, live

A binding connects one modulator to one target, and the demo below runs the real thing on this page: one sine modulator drives two bindings at once — a plain `intensity` property mapped onto 200–800, and a Tone-style `{ value }` frequency parameter mapped onto 400–1200 at half strength — both updated by the same `update` calls in an animation loop.

```js
const light = { intensity: 0 };
modulation.bind({ modulator: 'pulse', target: light, path: 'intensity', min: 200, max: 800 });

const filter = { frequency: { value: 400 } };
modulation.bind({ modulator: 'pulse', target: filter, path: 'frequency', min: 400, max: 1200, amount: 0.5 });
```

<div id="ts-binding-demo" class="live-example">
    <p>Live: <code>light.intensity</code> is <strong id="ts-binding-light-value">—</strong> (range 200–800).</p>
    <div style="background: rgba(255,255,255,0.08); height: 12px; max-width: 800px;"><div id="ts-binding-light-bar" style="background: #d4c389; height: 12px; width: 0%;"></div></div>
    <p>Live: <code>filter.frequency.value</code> is <strong id="ts-binding-filter-value">—</strong> (range 400–1200, amount 0.5).</p>
</div>

<script type="module" src="../../assets/typescript/demos/binding-demo.js"></script>

Beyond the `min`/`max` mapping in the demo, a binding with a numeric `range` adds the signal scaled around the target's current value, a binding with `scale` multiplies the current value, and a `setter` callback receives each computed value directly — the recommended write path for parameters implemented with getter and setter accessors, because a direct-path write into such a parameter can replace the accessor with a plain number. Bindings read their base value from the target once, when they first apply, so they compose with values the host sets by other means between updates. A binding's target can also come from a `modulationTargets` map, the name hosts such as Sonance use for the plain map of node parameters a binding may target.

## Scene modulator data

Scene files and preset data can describe modulators as plain data instead of code: an entry is an object with an `id`, a `type`, and the modulator options as its remaining properties, and a host applies a scene by creating one modulator per entry, forwarding the remaining properties as that modulator's options. The option keys — including `clockRef`, `divisor`, and `multiplier` for relative clocks — are a stable data contract, because renaming one would break scene files that no import check would catch.

## Using Modulator with Sonance

Sonance's decoupled engine takes a modulation system only through injection: the caller constructs a `ModulationSystem` and passes it to the engine as `options.modulation`, and the engine never imports this package itself. The contract the engine calls is five methods — `createModulator(id, options)`, `getModulator(id)`, `bind(options)`, `update(deltaTime, context)`, and `dispose()` — and `ModulationSystem` implements all five with those signatures, so a plain `new ModulationSystem()` is the whole wiring.

## The frame context

`createFrameContext(scene, frameState)` builds the per-frame context object a host passes to `update`: the caller's frame state spread in first, then `scene`, `camera: { position, target }`, `player: { position, active }`, and `input`. It reads the scene through five optional methods — `getCameraPosition()`, `getCameraTarget()`, `getPlayerPosition()`, `isFPSControlsEnabled()`, and `getInputState()` — and every access is optional-chained, so a partial scene yields null fields for the missing pieces instead of throwing, `null` works too, and when the scene reports no player position, `player.position` falls back to the camera position. The helper's original website-era name, `createPhotonicFrameContext`, is the same function exported as a compatibility alias.
