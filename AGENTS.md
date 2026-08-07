# Agent instructions

## Test organization

Name test files and classes after the **capability or use case** under test (e.g.
`PartialTests`, `DynamicTests`, `HelperTests`, `HandlebarsSpecCoverageTests`,
`CustomConfigurationTests`) — not after the GitHub issue that prompted them.

- Do not create `IssueNNNTests.cs` files or an `Issues/` folder.
- When a test is added because of a bug report, put it in the existing capability
  file that owns that behavior. If none fits, create a new capability-named file
  rather than an issue-numbered one.
- If the originating issue is worth referencing, link it in a `//` comment above
  the test method or class — not in the file or class name.

Issue numbers are meaningless once the bug is fixed; the capability is what
matters long-term, and grouping by capability keeps related coverage
discoverable together instead of scattered across one-off files.

## Release process

Publishing to NuGet is irreversible — a bad push can't be unpublished, only
deprecated. Don't skip the verification steps below to save time.

1. **The version comes from the GitHub Release's tag, not from any file in
   the repo.** `.github/workflows/release.yml` triggers on `release:
   published` and packs with
   `/p:version=${{ github.event.release.tag_name }}`. There is no version to
   bump in `Directory.Build.props` or any `.csproj` — creating and
   *publishing* the GitHub Release is the entire release action. A tag
   pushed without a published Release does nothing.
2. **Tag format has no `v` prefix** — `2.4.0`, `2.4.1`, not `v2.4.0`. Check
   the current version with `git tag --sort=-creatordate | head -1` or
   `gh release view --json tagName`.
3. **Before creating the release**, confirm the intended commit is actually
   on `master` (`git log --oneline -1 origin/master`) and that CI passed for
   whatever merged it — don't race a merge that's still running checks.
4. **Bump per semver** based on what's actually in the diff since the last
   tag: patch for bug fixes only, minor for additive features, major for
   breaking changes. Regression fixes for bugs introduced after the last
   release (i.e. the last release itself shipped broken) are still a patch
   bump, not a reason to skip versioning.
5. **Create and publish in one step** with
   `gh release create <tag> --title <tag> --notes-file <path>` (omit
   `--draft` — a draft does not trigger the workflow; only the `published`
   event does). Write release notes in the style of prior releases (see
   `gh release view 2.4.0 --json body`): grouped sections (Fixes / New
   features / Performance / Compatibility notes), one bullet per PR with a
   `(#NNN, fixes #NNN)` reference, a Contributors line, and a
   `Full Changelog` compare link
   (`https://github.com/Handlebars-Net/Handlebars.Net/compare/<prev>...<tag>`).
6. **After publishing**, watch the `Release` workflow run
   (`gh run list --workflow=release.yml --limit 1`) through to completion —
   it signs the assembly and pushes to NuGet via trusted OIDC publishing, and
   a failure partway through (e.g. signing) needs to be caught, not assumed
   to have succeeded because the Release object exists.
