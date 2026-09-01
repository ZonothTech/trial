# Exercise patches

Some sessions start from a specific change. Apply a patch only when your lab or instructor tells you to, on a new branch, never on `main`.

| Patch | Session | What it represents |
|-------|---------|--------------------|
| `s03-build-break.patch` | 3 | A teammate's unfinished change that breaks the build |
| `s06-support-lookup-pr.patch` | 5 and 6 | A colleague's feature branch, "support order lookup", ready for review |

```powershell
git switch -c <branch-name> course-start
git apply exercises/<patch-name>.patch
git status
```

To throw the change away: `git switch main` and `git branch -D <branch-name>` (after `git restore .` if files are still modified).
