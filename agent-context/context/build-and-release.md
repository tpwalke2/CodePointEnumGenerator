# Build & Release

## Local build
`bootstrap.ps1` restores the `dotnet tool` manifest (`.config/dotnet-tools.json`, pins `cake.tool` 6.3.0) then runs `dotnet cake build.cake` twice (bootstrap pass, then real run), passing `buildNumber`, `branch`, `buildPath`, `gitHubApiKey`, `nuGetApiKey`. Checks `$LASTEXITCODE` after each step and exits non-zero on failure — `gitHubApiKey`/`nuGetApiKey` may be omitted for non-release builds (see below).

## Cake pipeline (`build.cake` + `build/*.cake`)
Tasks, in dependency order: `Clean` → `Build` → `Test` → `Publish` → `Default`.

`build.cake` builds a `BuildConfig` directly off Cake's `Argument()` calls (target, buildNumber, currentRelease, branch, buildPath, API keys, verbose, maxDegreeOfParallelism, currentVersion). `Cake.ArgumentBinder` was removed (#31) — `build/BuildConfig.cake` is now a plain POCO with no binding attributes.

| Script | Role |
|---|---|
| `build/BuildConfig.cake` | Plain POCO holding parsed CLI arguments — target, buildNumber, branch, buildPath, API keys, verbose, maxDegreeOfParallelism, currentVersion |
| `build/BuildPaths.cake` | Resolves `output/`, `output/deploy/`, `output/deploy/test-results/`, `output/artifacts/` |
| `build/Utilities.cake` | `GetVersion` (`{CurrentRelease}.{BuildNumber}`), `ParallelInvoke`, `FlushDns` |
| `build/BuildOperations.cake` | `DoBuild` — parallel clean of Debug/Release, restore, `DotNetBuild` (Debug config) |
| `build/TestProject.cake` | `TestProject` record (path + target framework) |
| `build/TestOperations.cake` | `DoTest` — `dotnet test` per project, `trx` logger, into `output/deploy/test-results` |
| `build/PublishOperations.cake` | `DoPublish` — only runs `IsRelease(branch)` (branch == `main`); packs, creates a GitHub release (`Release-{version}` tag), uploads the `.nupkg` asset, pushes to NuGet |
| `build/GitHubApiModels.cake` | Request/response DTOs for the GitHub Releases REST API |

Version = `CurrentRelease` (default `1.0.1`) + `.` + `buildNumber` argument, unless `currentVersion` is overridden.

## CI (`.github/workflows/`)
Two GitHub Actions workflows, both `windows-latest` (Cake's `FlushDns` NuGet-publish workaround needs Windows), both ignore changes to `README.md` and `agent-context/**`:
- `ci.yml` — triggers on `pull_request` (any target) and `push` to `main`. Runs `dotnet tool restore` + both Cake steps directly (bypassing `bootstrap.ps1`, so a failed step fails the job natively). No secrets passed; `Publish` no-ops via `IsRelease(branch)`.
- `release.yml` — triggers on `push` to `main` only (no `pull_request`, no tags — avoids retriggering off the tag it creates). Same steps, plus `RELEASE_GITHUB_API_KEY`/`NUGET_API_KEY` repo secrets and `buildNumber=${{ github.run_number }}`.

AppVeyor is retired (`.appveyor.yml` removed).

## Release requirements
- GitHub PAT with read/write on Contents (release creation + asset upload), stored as the `RELEASE_GITHUB_API_KEY` repo secret.
- NuGet API key (push to `https://api.nuget.org/v3/index.json`), stored as the `NUGET_API_KEY` repo secret. Plan is to move to NuGet Trusted Publishing later.
- Every release on `main` creates a `Release-{version}` GitHub tag/release with auto-generated notes, and publishes the same `.nupkg` to NuGet.

---
*Last updated: 2026-09-22 | Verified against: bb6a64c*
