---
name: dotnet-security-review
description: Security review of existing .NET code (whole repository or given paths). Use when asked for a repository-wide or path-scoped security review. Read-only.
argument-hint: "[paths or 'repo'] (default: repo)"
allowed-tools: Read, Grep, Glob
disable-model-invocation: true
---

# .NET security review (starter)

<!--
Session 5 lab: copy this file to .claude/skills/dotnet-security-review/SKILL.md in TrailShop.
If .claude/skills/ did not exist when your Claude Code session started, run /reload-skills now and again after each later edit to this file in the same session (Claude Code does not watch a skills folder created mid-session).
Then run: /dotnet-security-review src
After your triage, fill in the two TODO sections below and run it again.
-->

Scope: $ARGUMENTS (if empty, review the whole repository under `src/`).

You are reviewing an ASP.NET Core application for exploitable security defects. Do not edit any file.

## Method

1. List the entry points first: every controller action, Razor page handler and middleware. For each, note the
   authorization attribute (or its absence) and the input it accepts.
2. Follow untrusted input from each entry point to where it is used: SQL, HTML output, file paths,
   outbound URLs, deserializers, logs.
3. For each candidate issue, find the exact lines and the path an attacker would use. If you cannot describe
   the path, mark the finding "unconfirmed".

## Team rules

TODO (lab task 5): add at least one rule your team wants every review to check, written so a reviewer can
verify it. Example shape: "Every controller or action declares X or Y."

## Known non-issues (do not report)

TODO (lab task 5): add the false positives you confirmed during triage, each with a one-line reason.

## Output

A Markdown table with one row per finding, highest severity first:

| # | Title | File:line | Category (CWE) | Entry point | Attack path | Severity | Confidence | Suggested fix | Test that would prove the fix |

Severity: Critical / High / Medium / Low. Confidence: High / Medium / Low. Then a short list of
"Checked and not a problem" items with one-line reasons. Do not report style issues with no attack path.
