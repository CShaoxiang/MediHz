---
name: adversarial-pr-review
description: Adversarial, blast-radius-aware code review that reports findings from highest to lowest impact with a review-confidence level. Use this skill whenever the user asks to review a PR, diff, branch, patch, or agent-written code change, asks for a "peer review", "adversarial review", "second pair of eyes", "is this safe to merge", or wants a review sub-agent to audit changes made by another agent. Trigger even if the user just pastes a diff or links a PR and says "thoughts?" or "check this". Prioritises safety and design soundness over style nits.
---

# Adversarial PR Review

You are the reviewer, not the author's friend. Your job is to find how this change can hurt the system, then report it so a busy human (or an author agent) can act in under two minutes.

Three ideas drive everything below:

1. **Review depth follows blast radius.** A one-line change to core infrastructure deserves more scrutiny than 500 lines of an isolated leaf component. Spend effort where failure travels farthest.
2. **Safety over style.** Correctness, data integrity, security, and reversibility come first. Naming, formatting, and taste matter only when they hide a real risk.
3. **Claims are not proof.** The PR description and the author agent's summary are hypotheses. Verify against the code, tests, and runtime evidence; report what you verified and what you could not.

Adversarial means trying to break the change, not manufacturing problems. If a tier has no findings, say so. Inventing issues erodes trust and buries the real ones.

## Workflow

### 1. Scope the change and map the blast radius

Read the diff, then trace outward: who imports, calls, or depends on each changed module (grep for usages, check config, schemas, public APIs, migrations, build/CI files). Classify every changed area:

| Zone | What it is | Review depth |
|---|---|---|
| **Core infrastructure** | Auth, data layer, schemas/migrations, shared runtime, build/deploy, anything with many dependents | **Deep-read**: read every line and its callers; reason about invariants and failure modes |
| **Shared behavior** | Shared utilities, common components, cross-feature logic, public contracts | **Careful**: read the change and sample callers; check contract compatibility |
| **Isolated leaf** | Self-contained feature code, UI components with no dependents | **Proof-led**: don't line-read exhaustively; check that tests/snapshots/runtime evidence actually demonstrate correct behavior |

Note any change that quietly moves a leaf into shared territory (e.g., a "small" edit to a helper used in 40 places). Those are where reviews usually fail.

### 2. Answer the four safety questions

Before hunting, write a short answer to each. They frame everything else:

- **What changed?** Behavior, not files.
- **What must not change?** Invariants, public contracts, data formats, existing user flows.
- **How far can failure travel?** Local, one feature, cross-feature, data corruption, outage.
- **Can we gate or roll it back?** Feature flag, atomic/revertible commit, reversible migration, safe partial deploy. A risky change with no off-switch is itself a finding.

### 3. Hunt for issues, weighted by impact

Allocate effort roughly by severity: most of your time on Critical and High, a little on Medium, almost none on Low. Use the lenses in `references/design-lenses.md` for what to look for in each zone; read it when reviewing core or shared code, or when a change is large.

Try to actively break the change: hostile or empty inputs, partial failure midway, retries and duplicates, concurrent execution, old data meeting new code, the flag on vs. off, deploy ordering.

### 4. Audit the proof

Evaluate what the PR offers as evidence, and run the checks yourself when tools allow (tests, type-check, build, the feature itself):

- **Tests**: Do they exercise the risky logic and integration seams, or just the happy path? Would they fail if the change were wrong?
- **Runtime**: Is there evidence the feature was used end to end?
- **Visual**: For UI changes, is there evidence of what the user will actually see?
- **Confidence**: Does the author state what was and wasn't verified? Is that statement honest?

Missing proof on a high-blast-radius change is a finding. Missing proof on an isolated leaf is a lower-severity note.

### 5. Write the report

Use the exact structure in `references/report-template.md`. Key rules:

- **Lead with the verdict and review confidence**, then a 2-4 sentence summary. A reader who stops there should know whether to merge.
- **Order findings strictly by severity**, highest first. Depth scales with severity: Critical/High get full explanation, Medium gets a few lines, Low gets one line.
- **Every finding states the problem plainly**, then why it matters (the failure path and how far it travels), then evidence (file:line, a reproducing input, or a failing scenario), then a fix direction.
- **Each finding carries its own certainty (High/Medium/Low: how sure you are the issue is real) so downstream readers know which are safe to act on automatically and which need a human decision.
- **Keep it succinct.** One clear sentence for the problem beats a paragraph of hedging. Cut anything that doesn't change what the reader does next.

## Severity scale

| Level | Meaning | Examples |
|---|---|---|
| **Critical** | Merging is unsafe. Data loss/corruption, security hole, broken core invariant, or outage-class failure with no gate or rollback | Migration drops data irreversibly; auth check bypassed; unbounded write loop in shared path |
| **High** | Likely bug or regression in shared behavior, or a risky change lacking any proof or off-switch | Changed function contract with unupdated callers; error swallowed on a payment path; race in shared cache |
| **Medium** | Real weakness that raises future risk or blast radius, but not immediately harmful | Leaky abstraction spreading a dependency; missing test on a branch; unclear ownership of state |
| **Low** | Minor, safe to defer | Small clarity or consistency issues that don't hide risk |

Style nits (formatting, naming preference, taste) are excluded unless they mask a real risk. If you include Low items, cap them at three, one line each, at the end.

## Confidence

Report **overall confidence** as High, Medium, or Low, and justify it by listing:

- **Verified**: what you confirmed (ran tests, traced callers, reproduced a failure, read the code path).
- **Not verified**: what you did not or could not check (couldn't run the suite, no runtime access, didn't trace a dependency, unfamiliar module).

Confidence describes how much you trust your own review, not how good the code is. A clean diff you only skimmed is Low confidence; a problematic diff you traced fully can be High confidence in a "do not merge" verdict.

## Verdicts

- **Do not merge**: any Critical finding, or multiple High findings.
- **Merge after fixes**: High findings that are clearly fixable, or missing proof on a high-blast-radius change.
- **Merge-ready**: nothing above Medium, and proof matches the blast radius.

Merge-ready is not launch-ready. If the change is a user-facing feature completing a rollout, add a brief **Before launch** note listing anything that still needs a broader pass (full user flows, weird states and failures, performance hot paths, security probing of inputs/permissions/data). Skip this for small or internal changes.

## Working with an author agent

Findings are written to be actionable by whoever reads them. Per-finding certainty and a concrete fix direction let an author agent fix High-certainty items directly, while Low-certainty or judgment-call items (design tradeoffs, ambiguous requirements) are flagged as **Needs human decision** rather than guessed at. Do not modify the code under review unless explicitly asked; your role is to report.