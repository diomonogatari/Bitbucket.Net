#!/usr/bin/env bash
set -euo pipefail

# Verifies (default) or fixes formatting for the repository using dotnet format.
#
# Usage:
#   bash scripts/verify-format.sh            # verify
#   bash scripts/verify-format.sh --verify   # verify
#   bash scripts/verify-format.sh --fix      # apply fixes

case "${1:-}" in
  --fix)
    MODE="fix"
    ;;
  --verify|"")
    MODE="verify"
    ;;
  -h|--help)
    cat <<'EOF'
verify-format.sh

Verifies (default) or fixes formatting using dotnet format.

Usage:
  bash scripts/verify-format.sh            # verify
  bash scripts/verify-format.sh --verify   # verify
  bash scripts/verify-format.sh --fix      # apply fixes

Environment variables:
  SKIP_DOTNET_FORMAT_WHITESPACE=1   Skip whitespace formatting checks.
EOF
    exit 0
    ;;
  *)
    echo "Unknown argument: $1" >&2
    exit 2
    ;;
esac

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SOLUTION_PATH="$ROOT_DIR/Bitbucket.Net.slnx"

if [[ ! -f "$SOLUTION_PATH" ]]; then
  echo "Solution not found at: $SOLUTION_PATH" >&2
  exit 1
fi

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet is required but was not found in PATH." >&2
  exit 1
fi

VERIFY_ARGS=()
if [[ "$MODE" == "verify" ]]; then
  VERIFY_ARGS+=(--verify-no-changes)
fi

if [[ "${SKIP_DOTNET_FORMAT_WHITESPACE:-}" != "1" ]]; then
  echo "Running: dotnet format whitespace ($MODE)" >&2
  dotnet format whitespace "$SOLUTION_PATH" "${VERIFY_ARGS[@]}"
else
  echo "Skipping: dotnet format whitespace (SKIP_DOTNET_FORMAT_WHITESPACE=1)" >&2
fi

echo "Running: dotnet format style ($MODE)" >&2
dotnet format style "$SOLUTION_PATH" --severity warn "${VERIFY_ARGS[@]}"
