# modulator
Modulation Library in C#

## Installation

Install via NuGet:

```bash
dotnet add package CodeCube.Modulator
```

Or visit [NuGet.org](https://www.nuget.org/packages/CodeCube.Modulator) to download the package.

## Contributing

### Versioning and Releases

This project uses [MinVer](https://github.com/adamralph/minver) for automatic semantic versioning based on Git tags and commit history.

**How it works:**
- Each commit to `main` generates a unique pre-release version (e.g., `1.0.0-alpha.0.1`)
- Pre-release versions are automatically published to NuGet.org
- To publish a stable release, create and push a Git tag with the version number:
  ```bash
  git tag v1.0.0
  git push origin v1.0.0
  ```
- After tagging, the next commit will start versioning from that tag (e.g., `1.0.1-alpha.0.1`)

**Version format:**
- Stable: `1.0.0` (when tagged with `v1.0.0`)
- Pre-release: `1.0.0-alpha.0.2` (2 commits after the base version)

This ensures every commit produces a unique package version, preventing conflicts during publishing.
