# Changelog

All notable changes to Modulator (TypeScript) are recorded in this file. The format follows Keep a Changelog, and the project follows Semantic Versioning: the version in package.json is the single source of truth, every change that will ship is written into this file's Unreleased section in the same commit that makes it, a release renames that section to the new version with its date before publishing, and breaking changes to the public exports in src/index.js are called out in plain words (a major version bump once the package is past 1.0, and a minor bump with a clear breaking note while it is still 0.x). Nothing is published to npm except on the owner's explicit word in an interactive session.

## [Unreleased]

Nothing yet. New work that will ship goes here as it lands.

## [0.1.0] — not yet published (prepared 2026-11-11)

The first package-ready build, prepared for the owner's publish gate. It has not been published to npm; until the owner says the word, consumers use the GitHub repo or a local checkout, as the README describes.

### Added

- The standalone library extracted from codecube.net's Static-Site-Generator (roadmap E1 and E2): the ModulationSystem, Modulator, and ModulationBinding classes, the wave, noise, value, and context modulator types, relative clocks through clockRef with divisor or multiplier, bindings that write into plain objects and Tone-style parameters, the frame-context helper, and the clamp, lerp, normalizeToUnit, readPath, and distance3D helpers, packaged as @codecube/modulator under the MIT license with no runtime dependencies.
- The package's own test suite (roadmap D2), migrated home from the website repo, covering wave sampling, both binding target shapes, the getter/setter limit and its setter-callback path, relative clocks, and the frame context.
- Package readiness (roadmap P1): a finished exports map, hand-written type declarations in src/index.d.ts that match the real public surface in src/index.js and were compiled against a test consumer, a README quickstart verified from the packed tarball in a clean folder, and this changelog.

### Changed

- Decoupling (roadmap D1 and D2): the frame-context helper's primary name is now createFrameContext, because the helper needs no Photonic scene — any object with the same optional methods works. The website-era name createPhotonicFrameContext stays exported as an alias for the same function, in the Modulation namespace too, so existing callers keep working. The Sonance injection contract (createModulator, getModulator, bind, update, dispose), the scene modulator data shape, and both binding target shapes are documented in the README as the stable contracts they are.

### Removed

- The website-era consumption routes are not part of this package: no window globals are set on import, and the website's webpack bundles are not a way to consume the library. Consumers import the ES module instead.
