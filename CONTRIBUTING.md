## Contributing

Thanks for your interest in contributing! This repository enforces a consistent
C# code style via `.editorconfig`, a pre-commit hook (opt-in), and CI checks.

## Prerequisites

- Git
- .NET SDK 10.x
- Bash (macOS/Linux: built-in; Windows: Git for Windows ships Git Bash)

## Enable the pre-commit hook (recommended)

Git hooks are not transferred by default when you clone a repository. This repo
keeps hooks in `.githooks/` and uses `core.hooksPath` so they can be versioned.

Run one of the setup scripts from the repo root:

```powershell
./scripts/setup-githooks.ps1
```

```bash
bash scripts/setup-githooks.sh
```

This sets:

- `git config core.hooksPath .githooks`

After that, every `git commit` will run the formatting verification.

### Bypass (when you really need to)

- One-off bypass:

```bash
git commit --no-verify
```

- Disable the formatting hook for a command/session:

```bash
SKIP_DOTNET_FORMAT=1 git commit
```

## Formatting: verify vs fix

The hook (and CI) run formatting in **verify** mode:

- `dotnet format whitespace ... --verify-no-changes`
- `dotnet format style ... --severity warn --verify-no-changes`

To run the same checks manually:

```powershell
./scripts/verify-format.ps1
```

```bash
bash scripts/verify-format.sh --verify
```

To apply fixes locally:

```powershell
./scripts/verify-format.ps1 -Fix
```

```bash
bash scripts/verify-format.sh --fix
```

## Tests

Please ensure tests pass before submitting a PR.

```bash
dotnet test ./test/Bitbucket.Net.Tests/Bitbucket.Net.Tests.csproj
```
