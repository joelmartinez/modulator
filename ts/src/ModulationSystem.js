const TWO_PI = Math.PI * 2;
const DEFAULT_CONTEXT = Object.freeze({});

function clamp(value, min, max) {
  return Math.min(max, Math.max(min, value));
}

function saturate(value) {
  return clamp(value, 0, 1);
}

function lerp(start, end, amount) {
  return start + ((end - start) * amount);
}

function smoothstep(value) {
  return value * value * (3 - (2 * value));
}

function fract(value) {
  return value - Math.floor(value);
}

function toPathSegments(path) {
  if (Array.isArray(path)) {
    return path.map(String);
  }

  if (typeof path !== 'string') {
    return [];
  }

  return path.split('.').filter(Boolean);
}

function readPath(root, path, fallback = null) {
  const segments = toPathSegments(path);
  if (!segments.length) {
    return root ?? fallback;
  }

  let current = root;
  for (const segment of segments) {
    if (current == null) {
      return fallback;
    }

    current = current[segment];
  }

  return current ?? fallback;
}

function resolvePathEndpoint(root, path) {
  const segments = toPathSegments(path);
  if (!root || segments.length === 0) {
    return null;
  }

  let current = root;
  for (let index = 0; index < segments.length - 1; index += 1) {
    current = current?.[segments[index]];
    if (current == null) {
      return null;
    }
  }

  const key = segments[segments.length - 1];
  return {
    parent: current,
    key,
    value: current?.[key]
  };
}

function readNumericValue(root, path) {
  const value = path == null ? root : readPath(root, path);
  if (typeof value === 'number') {
    return value;
  }

  if (value && typeof value === 'object' && typeof value.value === 'number') {
    return value.value;
  }

  return null;
}

function writeNumericValue(root, path, nextValue) {
  const endpoint = resolvePathEndpoint(root, path);
  if (!endpoint) {
    return false;
  }

  if (endpoint.value && typeof endpoint.value === 'object' && typeof endpoint.value.value === 'number') {
    endpoint.value.value = nextValue;
    return true;
  }

  endpoint.parent[endpoint.key] = nextValue;
  return true;
}

function normalizeToUnit(value, inputRange = [-1, 1]) {
  const [start, end] = inputRange;
  if (!Number.isFinite(start) || !Number.isFinite(end) || start === end) {
    return 0;
  }

  return saturate((value - start) / (end - start));
}

function asRange(range) {
  if (Array.isArray(range) && range.length === 2) {
    return { min: range[0], max: range[1] };
  }

  if (range && Number.isFinite(range.min) && Number.isFinite(range.max)) {
    return { min: range.min, max: range.max };
  }

  return null;
}

function waveSample(type, phase) {
  const wrappedPhase = fract(phase);

  switch (type) {
    case 'triangle':
      return 1 - (4 * Math.abs(wrappedPhase - 0.5));
    case 'square':
      return wrappedPhase < 0.5 ? 1 : -1;
    case 'sawtooth':
      return (wrappedPhase * 2) - 1;
    case 'ramp':
      return 1 - (wrappedPhase * 2);
    case 'sine':
    default:
      return Math.sin(wrappedPhase * TWO_PI);
  }
}

function hashNoise(seed) {
  return (fract(Math.sin(seed * 127.1 + 311.7) * 43758.5453123) * 2) - 1;
}

function noiseSample(time, frequency = 1, seed = 0) {
  const samplePosition = (time * frequency) + (seed * 17.17);
  const leftIndex = Math.floor(samplePosition);
  const amount = smoothstep(fract(samplePosition));

  return lerp(hashNoise(leftIndex), hashNoise(leftIndex + 1), amount);
}

function createFrameState() {
  return {
    time: 0,
    deltaTime: 0,
    frame: 0,
    context: DEFAULT_CONTEXT
  };
}

export class Modulator {
  constructor(id, options = {}) {
    this.id = id;
    this.options = options;
    this.lastValue = 0;
  }

  sample(state, system) {
    const value = this._resolveValue(state, system);
    this.lastValue = value;
    return value;
  }

  _resolveValue(state, system) {
    const {
      type = 'sine',
      frequency = 1,
      phase = 0,
      amplitude = 1,
      offset = 0,
      value = 0,
      path = null,
      selector = null,
      fallback = 0,
      getValue = null,
      transform = null,
      range = null,
      clamp: clampRange = null,
      seed = 0
    } = this.options;

    const frame = {
      ...state,
      system,
      modulator: this
    };

    // Resolve effective frequency for relative modulators.
    // When clockRef is set, derive frequency from the referenced modulator.
    //   divisor:    run at 1/N of the clock rate (e.g. divisor:2 = half speed). Takes priority over multiplier.
    //   multiplier: run at N× the clock rate (e.g. multiplier:2 = double speed). Ignored when divisor is set.
    // If both are provided, divisor takes precedence.
    let effectiveFrequency = frequency;
    const {
      clockRef = null,
      divisor = null,
      multiplier: relativeMultiplier = null
    } = this.options;
    if (clockRef && system) {
      const clockMod = typeof system.getModulator === 'function'
        ? system.getModulator(clockRef)
        : null;
      if (clockMod) {
        const clockFreq = clockMod.options.frequency ?? 1;
        if (typeof divisor === 'number' && divisor !== 0) {
          effectiveFrequency = clockFreq / divisor;
        } else if (typeof relativeMultiplier === 'number' && relativeMultiplier !== 0) {
          // multiplier: run at N× the clock rate. Zero is a no-op so we skip it.
          effectiveFrequency = clockFreq * relativeMultiplier;
        } else {
          effectiveFrequency = clockFreq;
        }
      }
    }

    let nextValue;
    if (typeof getValue === 'function') {
      nextValue = getValue(frame);
    } else {
      switch (type) {
        case 'value':
          nextValue = value;
          break;
        case 'context':
          nextValue = typeof selector === 'function'
            ? selector(frame)
            : readPath(state.context, path, fallback);
          break;
        case 'noise':
          nextValue = noiseSample(state.time, effectiveFrequency, seed);
          break;
        case 'triangle':
        case 'square':
        case 'sawtooth':
        case 'ramp':
        case 'sine':
          nextValue = waveSample(type, (state.time * effectiveFrequency) + phase);
          break;
        default:
          console.warn(`[Modulation] Unknown modulator type '${type}'. Falling back to 0.`);
          nextValue = 0;
          break;
      }
    }

    if (typeof transform === 'function') {
      nextValue = transform(nextValue, frame);
    }

    nextValue = (nextValue * amplitude) + offset;

    const mappedRange = asRange(range);
    if (mappedRange) {
      nextValue = lerp(mappedRange.min, mappedRange.max, normalizeToUnit(nextValue, [-1, 1]));
    }

    const boundedRange = asRange(clampRange);
    if (boundedRange) {
      nextValue = clamp(nextValue, boundedRange.min, boundedRange.max);
    }

    return nextValue;
  }
}

export class ModulationBinding {
  constructor(system, id, options = {}) {
    this.system = system;
    this.id = id;
    this.options = options;
    this.lastValue = null;
    this._cachedBaseValue = 0;
    this._hasCachedBase = false;
  }

  apply(state) {
    const sample = this._resolveSample(state);
    const targetRoot = this._resolveTarget(state);
    const path = this.options.path ?? this.options.property ?? null;
    const amount = this.options.amount ?? this.options.influence ?? 1;
    const baseValue = this._resolveBaseValue(targetRoot, path, state);

    const bindingFrame = {
      sample,
      baseValue,
      amount,
      state,
      system: this.system,
      binding: this,
      target: targetRoot
    };

    let nextValue;
    if (typeof this.options.transform === 'function') {
      nextValue = this.options.transform(bindingFrame);
    } else if (Number.isFinite(this.options.min) && Number.isFinite(this.options.max)) {
      const inputRange = this.options.inputRange ?? [-1, 1];
      const mappedValue = lerp(
        this.options.min,
        this.options.max,
        normalizeToUnit(sample, inputRange)
      );
      nextValue = lerp(baseValue, mappedValue, amount);
    } else if (Number.isFinite(this.options.range)) {
      nextValue = baseValue + (sample * this.options.range * amount);
    } else if (Number.isFinite(this.options.scale)) {
      nextValue = baseValue * (1 + (sample * this.options.scale * amount));
    } else {
      nextValue = baseValue + (sample * amount);
    }

    if (typeof this.options.setter === 'function') {
      this.options.setter(nextValue, bindingFrame);
    } else if (targetRoot && path) {
      writeNumericValue(targetRoot, path, nextValue);
    }

    this.lastValue = nextValue;
    return nextValue;
  }

  dispose() {
    this.system.removeBinding(this.id);
  }

  _resolveSample(state) {
    const source = this.options.source ?? this.options.modulator;
    if (typeof source === 'function') {
      return source({
        ...state,
        system: this.system,
        binding: this
      });
    }

    if (source && typeof source.sample === 'function') {
      return source.sample(state, this.system);
    }

    return this.system.sample(source);
  }

  _resolveTarget(state) {
    if (typeof this.options.target === 'function') {
      return this.options.target({
        ...state,
        system: this.system,
        binding: this
      });
    }

    return this.options.target ?? null;
  }

  _resolveBaseValue(targetRoot, path, state) {
    const { baseValue } = this.options;

    if (typeof baseValue === 'function') {
      return baseValue({
        ...state,
        system: this.system,
        binding: this,
        target: targetRoot,
        path
      });
    }

    if (Number.isFinite(baseValue)) {
      return baseValue;
    }

    if (!this._hasCachedBase) {
      const currentValue = readNumericValue(targetRoot, path);
      this._cachedBaseValue = currentValue ?? 0;
      this._hasCachedBase = true;
    }

    return this._cachedBaseValue;
  }
}

export class ModulationSystem {
  constructor() {
    this._state = createFrameState();
    this._modulators = new Map();
    this._bindings = new Map();
    this._values = new Map();
    this._bindingCount = 0;
  }

  createModulator(id, options = {}) {
    const modulator = new Modulator(id, options);
    this._modulators.set(id, modulator);
    return modulator;
  }

  getModulator(id) {
    return this._modulators.get(id);
  }

  removeModulator(id) {
    this._modulators.delete(id);
    this._values.delete(id);
  }

  bind(options = {}) {
    const id = options.id ?? `binding-${++this._bindingCount}`;
    const binding = new ModulationBinding(this, id, options);
    this._bindings.set(id, binding);
    return binding;
  }

  getBinding(id) {
    return this._bindings.get(id);
  }

  removeBinding(id) {
    this._bindings.delete(id);
  }

  update(deltaTime, context = DEFAULT_CONTEXT) {
    const safeDelta = Number.isFinite(deltaTime) && deltaTime > 0 ? deltaTime : 0;

    this._state = {
      time: this._state.time + safeDelta,
      deltaTime: safeDelta,
      frame: this._state.frame + 1,
      context: context && typeof context === 'object' ? context : DEFAULT_CONTEXT
    };

    this._values.clear();
    for (const [id, modulator] of this._modulators.entries()) {
      this._values.set(id, modulator.sample(this._state, this));
    }

    for (const binding of this._bindings.values()) {
      binding.apply(this._state);
    }

    return this._state;
  }

  sample(modulatorOrId, fallback = 0) {
    if (!modulatorOrId) {
      return fallback;
    }

    if (typeof modulatorOrId === 'string') {
      if (this._values.has(modulatorOrId)) {
        return this._values.get(modulatorOrId);
      }

      const modulator = this._modulators.get(modulatorOrId);
      if (!modulator) {
        return fallback;
      }

      const nextValue = modulator.sample(this._state, this);
      this._values.set(modulatorOrId, nextValue);
      return nextValue;
    }

    if (typeof modulatorOrId.sample === 'function') {
      return modulatorOrId.sample(this._state, this);
    }

    return fallback;
  }

  getState() {
    return this._state;
  }

  getValues() {
    return Object.fromEntries(this._values.entries());
  }

  reset() {
    this._state = createFrameState();
    this._values.clear();
  }

  dispose() {
    this._bindings.clear();
    this._modulators.clear();
    this._values.clear();
    this._state = createFrameState();
  }
}

export function distance3D(left, right = { x: 0, y: 0, z: 0 }) {
  if (!left || !right) {
    return 0;
  }

  const dx = (left.x ?? 0) - (right.x ?? 0);
  const dy = (left.y ?? 0) - (right.y ?? 0);
  const dz = (left.z ?? 0) - (right.z ?? 0);

  return Math.sqrt((dx * dx) + (dy * dy) + (dz * dz));
}

export function createFrameContext(scene, frameState = {}) {
  const cameraPosition = scene?.getCameraPosition?.() ?? null;
  const cameraTarget = scene?.getCameraTarget?.() ?? null;
  const playerPosition = scene?.getPlayerPosition?.() ?? cameraPosition;

  return {
    ...frameState,
    scene,
    camera: {
      position: cameraPosition,
      target: cameraTarget
    },
    player: {
      position: playerPosition,
      active: Boolean(scene?.isFPSControlsEnabled?.())
    },
    input: scene?.getInputState?.() ?? null
  };
}

export const createPhotonicFrameContext = createFrameContext;

export { clamp, lerp, normalizeToUnit, readPath };
