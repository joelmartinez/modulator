// Modulator public API types. Hand-written to match src/index.js at runtime.
// Options objects carry an index signature where the runtime forwards extra
// keys (scene modulator data is a stable contract), so hosts are not forced
// to cast plain data entries.

export interface Vector3Like {
  x?: number;
  y?: number;
  z?: number;
  [key: string]: unknown;
}

export type ModulatorType = 'sine' | 'triangle' | 'square' | 'sawtooth' | 'ramp' | 'noise' | 'value' | 'context' | string;

export interface ModulatorFrame {
  time: number;
  deltaTime: number;
  frame: number;
  context: Record<string, unknown>;
  system?: ModulationSystem;
  modulator?: Modulator;
  [key: string]: unknown;
}

export interface ModulatorOptions {
  type?: ModulatorType;
  frequency?: number;
  phase?: number;
  amplitude?: number;
  offset?: number;
  value?: number;
  path?: string | string[] | null;
  selector?: ((frame: ModulatorFrame) => unknown) | null;
  fallback?: number;
  getValue?: ((frame: ModulatorFrame) => number) | null;
  transform?: ((value: number, frame: ModulatorFrame) => number) | null;
  range?: [number, number] | { min: number; max: number } | null;
  clamp?: [number, number] | { min: number; max: number } | null;
  seed?: number;
  clockRef?: string | null;
  divisor?: number | null;
  multiplier?: number | null;
  [key: string]: unknown;
}

export interface BindingFrame {
  sample: number;
  baseValue: number;
  amount: number;
  state: FrameState;
  system: ModulationSystem;
  binding: ModulationBinding;
  target: unknown;
  [key: string]: unknown;
}

export interface ModulationBindingOptions {
  id?: string;
  source?: string | Modulator | ((frame: ModulatorFrame & { binding: ModulationBinding }) => number) | null;
  modulator?: string | Modulator | ((frame: ModulatorFrame & { binding: ModulationBinding }) => number) | null;
  target?: unknown | ((frame: ModulatorFrame & { binding: ModulationBinding }) => unknown);
  path?: string | null;
  property?: string | null;
  amount?: number;
  influence?: number;
  min?: number;
  max?: number;
  inputRange?: [number, number];
  range?: number;
  scale?: number;
  baseValue?: number | ((frame: BindingFrame & { path: string | null }) => number);
  setter?: ((value: number, frame: BindingFrame) => void) | null;
  transform?: ((frame: BindingFrame) => number) | null;
  [key: string]: unknown;
}

export interface FrameState {
  time: number;
  deltaTime: number;
  frame: number;
  context: Record<string, unknown>;
}

export interface FrameScene {
  getCameraPosition?: () => Vector3Like | null;
  getCameraTarget?: () => Vector3Like | null;
  getPlayerPosition?: () => Vector3Like | null;
  isFPSControlsEnabled?: () => boolean;
  getInputState?: () => unknown;
  [key: string]: unknown;
}

export interface FrameContext {
  scene: FrameScene | null;
  camera: { position: Vector3Like | null; target: Vector3Like | null };
  player: { position: Vector3Like | null; active: boolean };
  input: unknown;
  [key: string]: unknown;
}

export declare class Modulator {
  constructor(id: string, options?: ModulatorOptions);
  readonly id: string;
  options: ModulatorOptions;
  lastValue: number;
  sample(state: FrameState, system?: ModulationSystem): number;
}

export declare class ModulationBinding {
  constructor(system: ModulationSystem, id: string, options?: ModulationBindingOptions);
  readonly system: ModulationSystem;
  readonly id: string;
  options: ModulationBindingOptions;
  lastValue: number | null;
  apply(state: FrameState): number;
  dispose(): void;
}

export declare class ModulationSystem {
  constructor();
  createModulator(id: string, options?: ModulatorOptions): Modulator;
  getModulator(id: string): Modulator | undefined;
  removeModulator(id: string): void;
  bind(options?: ModulationBindingOptions): ModulationBinding;
  getBinding(id: string): ModulationBinding | undefined;
  removeBinding(id: string): void;
  update(deltaTime: number, context?: Record<string, unknown>): FrameState;
  sample(modulatorOrId: string | Modulator | { sample: (state: FrameState, system: ModulationSystem) => number } | null | undefined, fallback?: number): number;
  getState(): FrameState;
  getValues(): Record<string, number>;
  reset(): void;
  dispose(): void;
}

export declare function clamp(value: number, min: number, max: number): number;
export declare function lerp(start: number, end: number, amount: number): number;
export declare function normalizeToUnit(value: number, inputRange?: [number, number]): number;
export declare function readPath(root: unknown, path: string | string[] | null, fallback?: unknown): unknown;
export declare function distance3D(left: Vector3Like | null | undefined, right?: Vector3Like): number;
export declare function createFrameContext(scene: FrameScene | null | undefined, frameState?: Record<string, unknown>): FrameContext;
export declare const createPhotonicFrameContext: typeof createFrameContext;

export declare const Modulation: {
  ModulationSystem: typeof ModulationSystem;
  Modulator: typeof Modulator;
  ModulationBinding: typeof ModulationBinding;
  clamp: typeof clamp;
  createFrameContext: typeof createFrameContext;
  createPhotonicFrameContext: typeof createPhotonicFrameContext;
  distance3D: typeof distance3D;
  lerp: typeof lerp;
  normalizeToUnit: typeof normalizeToUnit;
  readPath: typeof readPath;
};
