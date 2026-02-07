#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(git rev-parse --show-toplevel 2>/dev/null || true)"

if [[ -z "$ROOT_DIR" ]]; then
  echo "Not inside a git repository." >&2
  exit 1
fi

cd "$ROOT_DIR"

git config core.hooksPath .githooks

# Make sure hook script is executable on macOS/Linux.
if command -v chmod >/dev/null 2>&1; then
  chmod +x .githooks/pre-commit 2>/dev/null || true
fi

echo "Git hooks enabled (core.hooksPath=.githooks)." >&2
