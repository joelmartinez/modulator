---
template: Default
title: Modulation Concepts — TypeScript
subtitle: Modulators, waveforms, bindings, and relative clocks in @codecube/modulator.
---

# Modulation Concepts (TypeScript)

Modulation is the practice of letting one changing value steer another: a slow wave raises and lowers a light's brightness, a noise signal wobbles a filter, a scene's camera distance opens a gate. The TypeScript library breaks that into three pieces with plain names — a modulator makes a value, a binding writes a value somewhere useful, and a modulation system advances everything together, one update at a time.

## Modulators

A modulator is a named source of numbers, created with `createModulator(id, options)`. Its `type` option picks where the numbers come from: the waveforms `sine`, `triangle`, `square`, `sawtooth`, and `ramp`; `noise`, a smooth random-feeling signal built from interpolated hash noise, so it wanders instead of jumping; `value`, a fixed number; and `context`, which reads a number out of the per-frame context object the host passes to `update`, by dotted path or by a selector function. The shaping options are the same for every kind: `frequency` sets how fast the signal moves, `phase` shifts where in its cycle it starts, `amplitude` scales its output, and `offset` shifts it, while `range` can map the output onto a numeric range and `clamp` can bound it outright.

The explorer below runs the real library on this page: every point on the curve is sampled from a `Modulator` of the chosen type and frequency, drawn over one second, so changing the controls redraws the actual signal the library produces.

<div class="live-example">
    <p>
        <label>Waveform <select id="ts-waveform-type">
            <option value="sine" selected>sine</option>
            <option value="triangle">triangle</option>
            <option value="square">square</option>
            <option value="sawtooth">sawtooth</option>
            <option value="ramp">ramp</option>
            <option value="noise">noise</option>
        </select></label>
        <label>Frequency <input id="ts-waveform-frequency" type="range" min="1" max="4" step="1" value="1" /></label>
    </p>
    <canvas id="ts-waveform-canvas" class="waveform-visualization" width="800" height="300"></canvas>
    <p id="ts-waveform-readout">Loading the live example…</p>
</div>

<script type="module" src="../../assets/typescript/demos/waveform-explorer.js"></script>

## Bindings

A modulator on its own only produces numbers when asked; a binding is what makes those numbers matter. `bind(options)` connects one modulator to one target — a plain object property named by a dotted `path`, or a Tone.js-style parameter object with a numeric `value` — and on every `update`, the binding computes the modulator's current value and writes it into the target. The write modes cover the common shapes: with `min` and `max` the signal is mapped onto an output range, with `range` it is added around the target's current value, with `scale` it multiplies the current value, and with `amount` any of these can be blended part-way instead of applied in full.

## Relative clocks

Modulators can keep time together. A modulator created with a `clockRef` option names another modulator as its clock, and then `divisor` runs it at a fraction of the clock's rate — a divisor of 2 is half speed, the classic sub-oscillator — while `multiplier` runs it at a multiple. This is how a scene keeps a slow swell and its faster shimmer locked in step even when the base rate changes: change the clock's frequency and every relative modulator follows, because the rate is derived at sample time rather than copied at creation.

## Context and the frame

Hosts that run a frame loop — a game, a scene renderer, an audio engine — usually already know things a modulator should react to, such as where the camera is. Those hosts pass a context object into `update(deltaTime, context)`, and `context` modulators read from it, so the same library drives a light from a waveform on one binding and from the camera's position on another. The `createFrameContext(scene, frameState)` helper builds that context object from any scene that offers the camera, player, and input methods it looks for, each one optional; the [using the library](../library-usage/) page documents the exact contract.
