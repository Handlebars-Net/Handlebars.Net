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
