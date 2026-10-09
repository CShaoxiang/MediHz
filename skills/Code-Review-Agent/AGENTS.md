# Reference documents

- Refer to README.md for information about the team's processes, the project, and its structure.

# Peer Review skills :
# PR Review: <short title of the change>

**Verdict:** Do not merge | Merge after fixes | Merge-ready
**Overall confidence:** High | Medium | Low

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
#### C1. <Problem in one plain sentence> `[confidence: High|Medium|Low]`
- **Where:** `file:line`
- **Why it matters:** <failure path and how far it travels>
- **Evidence:** <reproducing input, failing scenario, or traced call path>
- **Fix direction:** <concrete next step>
- **Decision owner:** Author agent | Human

### High
#### H1. ...  (same fields)

### Medium
- **M1. <Problem>** `[confidence: X]` — `file:line`. <Why it matters, 1-2 sentences.> Fix: <direction>.

### Low
- **L1.** <One line.> `file:line` (max 3)

## Proof audit
| Proof | Status | Note |
|---|---|---|
| Tests | Adequate / Partial / Missing | <what's covered vs. the risky logic> |
| Runtime (end to end) | Shown / Not shown | |
| Visual (UI only) | Shown / Not shown / N/A | |
| Author's confidence claim | Honest / Overstated / Absent | |

## Confidence basis
- **Verified:** <what you ran, traced, or reproduced>
- **Not verified:** <what you could not or did not check, and why>

## Needs human decision
<Only ambiguous tradeoffs or requirement questions. Omit if none.>

## Before launch
<Only for user-facing feature completions. Brief list across features, bugs/weird states, performance, security. Omit otherwise.>