# .github/workflows/ci.yml:25-29

- On pushes to main, this workflow still runs the default Cake target, which reaches Publish; because this job passes no API keys, DoGitHubRelease/DoNuGetPublish throw and the CI job fails (while the release workflow is running in parallel). Run the Test target for both commands here, or otherwise exclude main from this workflow.

# agent-context/README.md:1-3

- This overview still says CI runs on AppVeyor, but this PR deletes .appveyor.yml and adds GitHub Actions workflows. Agents using this context will be directed to the retired CI system; update the sentence to describe GitHub Actions.
- This issue also appears in the following locations of the same file:
  - line 11
  - line 22

# agent-context/context/architecture.md:10-13

- The architecture snapshot still identifies AppVeyor and .appveyor.yml as the CI implementation, but that file is removed and CI is now in .github/workflows/. Update this current-state row so the architecture documentation does not direct readers to a nonexistent pipeline.