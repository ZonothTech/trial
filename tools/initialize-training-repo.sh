#!/usr/bin/env bash
# Creates the TrailShop training Git history (macOS / Linux / Git Bash).
#
# The history deliberately contains a SYNTHETIC SMTP password that is added in one
# commit and removed in the next. Session 7 uses it to practise finding secrets in
# history. The value is fake and has never worked anywhere.
#
# It also creates a local bare repository next to this folder and registers it as
# "origin" (Claude Code's /security-review compares your branch with origin's
# default branch, so it needs one). Pass --no-origin to skip that step, for example
# when you will push to GitHub and use that as origin instead.
#
# Run once from anywhere:   bash tools/initialize-training-repo.sh [--no-origin]
# Result: a repo on branch "main" with three commits and the tag "course-start".
set -euo pipefail

CREATE_ORIGIN=1
if [ "${1:-}" = "--no-origin" ]; then
  CREATE_ORIGIN=0
fi

cd "$(dirname "$0")/.."

if [ -e .git ]; then
  echo "A .git folder already exists in $(pwd). Refusing to touch existing history." >&2
  exit 1
fi

# Refuse to run inside another repository (for example a clone of the course's student
# repository): the history and the local origin must live in TrailShop's own folder.
if OUTER="$(git rev-parse --show-toplevel 2>/dev/null)"; then
  echo "This folder is inside another Git repository ($OUTER)." >&2
  echo "Copy the TrailShop folder somewhere outside it (for example ~/src/TrailShop) and run this script there." >&2
  exit 1
fi

DEV_SETTINGS="src/TrailShop.Api/appsettings.Development.json"

commit() {
  local date="$1"; shift
  GIT_AUTHOR_DATE="$date" GIT_COMMITTER_DATE="$date" \
    git -c user.name="TrailShop Trainer" -c user.email="trainer@trailshop.example" \
        -c commit.gpgsign=false commit --quiet "$@"
}

git init --quiet --initial-branch=main
git add --all
commit "2026-09-01T10:00:00Z" -m "Initial TrailShop import"

# Commit 2: a developer adds SMTP settings, including a (synthetic) password.
cat > "$DEV_SETTINGS" <<'JSON'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Smtp": {
    "Host": "smtp.trailshop.invalid",
    "Port": 587,
    "Username": "orders@trailshop.example",
    "Password": "Wint3r-Trail-2026!"
  }
}
JSON
git add "$DEV_SETTINGS"
commit "2026-09-08T16:20:00Z" -m "Configure SMTP for order confirmation emails"

# Commit 3: the settings are removed again. The password is still in history.
cat > "$DEV_SETTINGS" <<'JSON'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
JSON
git add "$DEV_SETTINGS"
commit "2026-09-09T09:05:00Z" -m "Remove SMTP settings from appsettings"

git tag course-start

if [ "$CREATE_ORIGIN" -eq 1 ]; then
  ORIGIN_DIR="$(cd .. && pwd)/$(basename "$(pwd)")-origin.git"
  if [ -e "$ORIGIN_DIR" ]; then
    echo "Skipping local origin: $ORIGIN_DIR already exists." >&2
  else
    git init --quiet --bare --initial-branch=main "$ORIGIN_DIR"
    git remote add origin "$ORIGIN_DIR"
    git push --quiet --set-upstream origin main
    git push --quiet origin course-start
    git remote set-head origin main
    echo "Local origin created at $ORIGIN_DIR"
  fi
fi

echo "TrailShop training history created:"
git log --oneline --decorate
