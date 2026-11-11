# Modulator (TypeScript)

Modulator is a small modulation library with no dependencies. A modulator produces a changing value over time — a sine, triangle, square, sawtooth, or ramp wave, a smooth noise signal, a fixed value, or a value read out of a per-frame context — and a binding writes that value into something else: a plain object property named by a dotted path, or a Tone.js-style parameter object with a numeric `value`. One `ModulationSystem` owns the modulators and bindings and advances them together, one `update(deltaTime, context)` call per frame or tick.

This package is `@codecube/modulator`, the TypeScript half of the Modulator library. The C# half (NuGet package CodeCube.Modulator) lives in the same repository, [joelmartinez/modulator](https://github.com/joelmartinez/modulator); this package lives under that repo's `ts/` folder, and the files here are laid out exactly as they land there. It was extracted from joelmartinez/Static-Site-Generator, the repo behind codecube.net, where the modulation code lived inside the site's frontend and was shared informally by the site's audio engine (Sonance) and its 3D pages (Photonic). Sonance is the first planned consumer: its decoupled engine takes a modulation system as an optional injected dependency, and this package is that system.

## What was extracted

The two files under `src/` are the complete modulation tree from `frontend/src/modulation/` in the source repo, pulled byte-identical at source commit 03b4f6647bb5c3d9438726bc07c4ed3a40ad7e72 and verified file by file against the source repo's git blob hashes (`ModulationSystem.js`: ca848c0ea6297504ee9d1d46fabacad57e7aadcb; `index.js`: 8a7c9b3a49da621a59ff3312e0070ecfe78b6731). The library imports nothing at all: no packages, no DOM, no `window` or `document`, and no build step — the files are plain ES modules with explicit `.js` import extensions, so they run in Node and in browsers unchanged. The full extraction record and coupling inventory live in the control repo's `extraction-notes/EXTRACTION.md`, not in this package.

## Layout

`src/` holds the library itself, with `src/index.js` as the entry point named by package.json; `ModulationSystem.js` holds the whole implementation and `index.js` re-exports its surface plus a combined `Modulation` namespace object. `tests/` holds the package's own test suite, migrated home from the website repo during decoupling; it is not part of the published file set. `LICENSE` is the MIT license, word for word the same as the C# library's license in the repository root (Copyright (c) 2024 Joel Martinez). There are no other runtime files and no dependencies to install.

## Quickstart

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

This exact code was run against this scaffold in Node with no DOM present for roadmap item E2: it prints 800, the top of the binding's range. Named imports cover the whole public surface — `ModulationSystem`, `Modulator`, `ModulationBinding`, the `Modulation` namespace object, and the helpers `clamp`, `lerp`, `normalizeToUnit`, `readPath`, `distance3D`, `createFrameContext`, and `createPhotonicFrameContext`.

## Modulators and bindings

A modulator's options choose its kind and shape: `type` is one of `sine`, `triangle`, `square`, `sawtooth`, `ramp`, `noise`, `value`, or `context`; `frequency`, `phase`, `amplitude`, and `offset` shape the signal; `range` maps the output onto a numeric range and `clamp` bounds it. A modulator can also run relative to another one — set `clockRef` to a second modulator's id, with `divisor` to run at a fraction of its rate or `multiplier` to run at a multiple. A `context` modulator reads a value out of the object passed to `update`, by dotted `path` or by a `selector` function, and a `getValue` function or a `transform` function can take over or reshape any modulator's output.

A binding connects one modulator to one target. With `min` and `max` it maps the signal onto that output range; with `range` it adds the signal scaled around the target's current value; with `scale` it multiplies the current value; and a `setter` function or a Tone-style `{ value }` target object are both supported write paths. Bindings read their base value from the target once, so they compose with values the host sets by other means between updates.

## Binding targets

A binding writes into one of two target shapes. The first is a plain numeric property on a plain object, reached by the binding's dotted `path` — for example, a binding with path `intensity` writes a number into `light.intensity`, and a path like `filter.frequency` walks nested objects to the leaf property. The second is a Tone.js-style parameter object: when the value at the end of the path is an object with a numeric `value` property, the binding updates that `value` in place instead of replacing the object, so a `{ value: 400 }` parameter stays the same object while its number changes.

One limit matters for the second shape: a parameter implemented with getter and setter accessors (the Tone.js Signal shape) may be overwritten as a plain number by a direct-path write, losing the accessor. For accessor-based parameters, give the binding a `setter` callback instead — the library calls it with each computed value, and that callback writes the value wherever the host needs it. A binding's target can also come from a `modulationTargets` map, the name hosts such as Sonance use for the plain map of node parameters a binding may target; the map is the host's own property, and the values inside it are written in the two shapes above.

## Scene modulator data

Scene files and preset data can describe modulators as plain data instead of code. A modulator entry is an object with an `id`, a `type`, and the modulator options as its remaining properties — `{ id, type, ...options }` — and a host applies a scene by creating one modulator per entry, forwarding the remaining properties as that modulator's options. Relative-clock entries use the same option keys as code does: `clockRef` names the modulator to follow, and `divisor` or `multiplier` sets its rate against that clock. This shape is a data contract, not just a habit: renaming an option key would break scene files that no import check would catch, so the keys are kept stable and any change to them is a deliberate breaking change.

## Using Modulator with Sonance

Sonance's decoupled engine takes a modulation system only through injection: the caller constructs a `ModulationSystem` and passes it to the engine as `options.modulation`, and the engine never imports this package itself. The contract the engine calls is five methods on the injected system — `createModulator(id, options)`, `getModulator(id)`, `bind(options)`, `update(deltaTime, context)`, and `dispose()` — and `ModulationSystem` implements all five with those signatures, so a plain `new ModulationSystem()` is the whole wiring. When no system is injected the engine's modulation methods report that clearly and its scene manager skips a scene's modulator section with a warning while still applying the rest of the scene; declaring this package as Sonance's optional dependency and wiring the default instance is planned work that follows this package's publish gate.

## The frame context

`createFrameContext(scene, frameState)` builds the per-frame context object a host passes to `update`: the caller's frame state spread in first, then `scene`, `camera: { position, target }`, `player: { position, active }`, and `input`. It reads the scene through a duck-typed contract of five optional methods — `getCameraPosition()`, `getCameraTarget()`, `getPlayerPosition()`, `isFPSControlsEnabled()`, and `getInputState()` — and every access is optional-chained, so any plain object with some or all of those methods works, a partial scene yields null fields for the missing pieces instead of throwing, and `null` works too. A Photonic scene satisfies this contract, which is where the shape comes from, but nothing in the helper imports or needs Photonic, and any object with the same methods is equally valid. When the scene reports no player position, `player.position` falls back to the camera position. The helper's original website-era name, `createPhotonicFrameContext`, is kept as a compatibility alias: it is exported alongside the generic name, in the `Modulation` namespace too, and both names are the same function. A `context` modulator typically reads this object by path — for example, a path of `camera.position` with a selector that measures its distance from the origin.

## Status

Extraction (roadmap item E1), this package scaffold (E2), the decoupling audit (D1), and the decoupling work itself (D2) are done: the package is named `@codecube/modulator` at version 0.1.0, carries the MIT license, and declares no dependencies because the code needs none. Decoupling severed the website-era ties: the library imports nothing and sets no global of any kind (no `window.Sonance`, `window.Photonic`, or `window.PhotonicModules`), it needs no bundler or build step and is consumed as the ES modules it already is, its test suite lives in this package and runs with `npm test`, the frame-context helper carries the generic name `createFrameContext` with its website-era name kept as an alias, and the Sonance injection contract, the scene modulator data shape, and both binding target shapes are documented above as the contracts they are. A standalone-consumption test proves the boundary: a fresh folder holding only this package and a consumer whose sole dependency is the package runs an example in Node and in a browser with no website files present. The TypeScript section of the repo's Chloroplast docs site (DOCS) is built and staged in the control repo's `docs-staging/` folder, with live in-page examples that run this library, ready to land in the repo's `docs/` tree at the same publish gate. Package readiness (P1) is done too: the exports map covers the root entry and `./package.json`, hand-written type declarations in `src/index.d.ts` match all eleven runtime exports and compile against a strict TypeScript consumer, `CHANGELOG.md` records the 0.1.0 build, the packed tarball holds exactly seven files (the two source files, the declarations, the README, the changelog, the license, and package.json — no tests and no docs staging), and the quickstart above runs from that tarball in a clean folder and prints 800. Nothing remains before the publish gate. The package lands in joelmartinez/modulator under `ts/` only at the publish gate Joel approves, and npm publishing happens only on Joel's word.

Not supported, stated plainly so the boundary is a documented fact: (1) the website's photonics pages `audio-demo.js` and `synth-designer.js` running against the standalone package — they are parked website examples, and rebuilding them as Sonance examples is Sonance roadmap item X1's work; (2) the `window.Sonance`, `window.Photonic`, and `window.PhotonicModules` script-tag consumption model and the website's `.gen.js` webpack bundles as a way to consume this package — a host page that wants a global sets it itself from the imported module; (3) the host test infrastructure — the website's jest and babel configuration and its shared test setup — none of which this package's own minimal jest setup replaces or needs; and (4) direct-path binding writes into getter/setter-implemented parameters, which may be overwritten as plain numbers — accessor-based targets are supported through a binding's `setter` callback instead.
