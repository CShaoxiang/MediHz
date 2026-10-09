# Report Template

Use this structure exactly. Omit a severity section only if it has no findings, and say "None found" for Critical and High so the reader knows you looked.

---

```markdown
# PR Review: <short title of the change>

**Verdict:** Do not merge | Merge after fixes | Merge-ready
**Review confidence:** High | Medium | Low (trust in this review, not a rating of the code)

## Summary
<2-4 sentences. What the change does, the single biggest risk, and why the verdict follows. A reader who stops here should know what to do.>

## Blast radius
| Area | Zone | Depth applied | Note |
|---|---|---|---|
| `path/or/module` | Core / Shared / Leaf | Deep-read / Careful / Proof-led | <one line: dependents, or why it's isolated> |

**Safety questions**
- **What changed:** <behavior in one line>
- **What must not change:** <invariants/contracts>
- **How far failure travels:** <local / feature / cross-feature / data / outage>
- **Gate / rollback:** <yes/no and how>

## Findings

### Critical
#### C1. <Problem in one plain sentence> `[Certainty: High|Medium|Low]`
- **Where:** `file:line`
- **Why it matters:** <failure path and how far it travels>
- **Evidence:** <reproducing input, failing scenario, or traced call path>
- **Fix direction:** <concrete next step>
- **Decision owner:** Author agent | Human

### High
#### H1. ...  (same fields)

### Medium
- **M1. <Problem>** `[Certainty: X]` — `file:line`. <Why it matters, 1-2 sentences.> Fix: <direction>.

### Low
- **L1.** <One line.> `file:line` (max 3)

## Proof audit
| Proof | Status | Note |
|---|---|---|
| Tests | Adequate / Partial / Missing | <what's covered vs. the risky logic> |
| Runtime (end to end) | Shown / Not shown | |
| Visual (UI only) | Shown / Not shown / N/A | |
| Author's confidence claim | Honest / Overstated / Absent | |

## Review confidence basis
- **Verified:** <what you ran, traced, or reproduced>
- **Not verified:** <what you could not or did not check, and why>

## Needs human decision
<Only ambiguous tradeoffs or requirement questions. Omit if none.>

## Before launch
<Only for user-facing feature completions. Brief list across features, bugs/weird states, performance, security. Omit otherwise.>
```

---

## Style of the write-up

- **Problem sentence first.** State what is wrong, not what you looked at. Prefer "`retry()` re-sends non-idempotent charges on timeout" over "There may be an issue with retry handling."
- **Show the failure path.** "When X happens, Y breaks, affecting Z." Concrete beats abstract.
- **Scale depth with severity.** Critical and High get the full block. Medium gets a bullet. Low gets a line.
- **No hedging filler.** If certainty is low, say so in the tag and in "Not verified"; don't soften the prose.
- **No praise padding.** One clause of what's solid is fine if it affects the verdict; otherwise skip it.

## Example (abbreviated)

```markdown
# PR Review: Add bulk-invite endpoint

**Verdict:** Do not merge
**Review confidence:** High

## Summary
Adds `POST /invites/bulk` and refactors the shared `sendEmail()` helper to batch sends. The refactor drops per-recipient error handling, so one bad address silently aborts the rest of the batch on every caller, including password reset. No flag or rollback path exists for the helper change.

## Blast radius
| Area | Zone | Depth applied | Note |
|---|---|---|---|
| `lib/email/send.ts` | Shared | Careful | 14 callers incl. auth reset flow |
| `api/invites/bulk.ts` | Leaf | Proof-led | New endpoint, no dependents |

**Safety questions**
- **What changed:** Email sending now batches; new bulk endpoint.
- **What must not change:** Password reset and single-invite delivery.
- **How far failure travels:** Cross-feature (auth flows).
- **Gate / rollback:** No flag; revertible commit only.

## Findings

### Critical
None found.

### High
#### H1. `sendEmail()` aborts the whole batch on the first failed recipient `[certainty: High]`
- **Where:** `lib/email/send.ts:58`
- **Why it matters:** The `try/catch` now wraps the loop instead of each send, so one invalid address stops all later sends without surfacing an error. Password-reset emails share this path.
- **Evidence:** Traced `auth/reset.ts:31` to `sendEmail([a, b])`; with `a` invalid, `b` is never sent and the function returns success.
- **Fix direction:** Catch per recipient; return per-recipient results; add a test with one invalid address.
- **Decision owner:** Author agent

### Medium
- **M1. Bulk endpoint has no size cap** `[certainty: Medium]` — `api/invites/bulk.ts:12`. Large payloads could exhaust the mail queue. Fix: cap batch size and reject above it.

### Low
- **L1.** Unused import `chunk`. `api/invites/bulk.ts:2`

## Proof audit
| Proof | Status | Note |
|---|---|---|
| Tests | Partial | Happy path only; no failed-recipient case on the shared helper |
| Runtime (end to end) | Not shown | |
| Visual (UI only) | N/A | |
| Author's verification claim | Overstated | "Fully tested" but shared helper failure path untested |

## Review confidence basis
- **Verified:** Traced all 14 callers; ran the unit suite (passes); reproduced H1 with a two-recipient script.
- **Not verified:** Real SMTP behavior; production queue limits.
```