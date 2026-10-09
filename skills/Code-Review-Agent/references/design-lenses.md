# Design Lenses

Questions to ask while hunting for issues. They are ordered roughly by how severe the findings tend to be. Use them as prompts, not a checklist to recite; only report what you actually find.

## Contents
1. Correctness and invariants
2. Data integrity and migrations
3. Security and trust boundaries
4. Failure handling and resilience
5. Concurrency and ordering
6. Contracts and compatibility
7. Gating, atomicity, and reversibility
8. Coupling and abstraction boundaries
9. Observability
10. Proof quality
11. What to skip

---

## 1. Correctness and invariants
- What must always be true before and after this code runs? Does the change preserve it on every path, including early returns and error paths?
- Off-by-one, null/empty/zero, boundary values, time zones, unicode, large inputs.
- Does behavior change for existing inputs that the author didn't mention?
- Does the code do what the PR says, or something adjacent?

## 2. Data integrity and migrations
- Can this lose, duplicate, or corrupt data? What happens on partial failure midway?
- Are migrations reversible? Safe while old code is still running (expand/contract)? Safe on large tables (locks, backfill time)?
- Is old data compatible with new code, and new data with old code (rollback scenario)?
- Are writes idempotent so retries don't double-apply?

## 3. Security and trust boundaries
- Where does untrusted input enter, and is it validated/escaped before use (injection, path traversal, SSRF, XSS, deserialization)?
- Authn/authz: is the check present on every path, at the right layer, and not bypassable by a new route or parameter?
- Secrets, tokens, PII in logs, errors, URLs, or client bundles.
- Does new dependency or permission scope widen the attack surface?

## 4. Failure handling and resilience
- Swallowed errors, broad catches, success returned on failure, errors that lose context.
- Timeouts, retries (with backoff and idempotency), unbounded loops or queues, resource leaks.
- What does the user or caller see when a dependency is down or slow?
- Failure isolation: does one bad item/tenant/request take down the rest?

## 5. Concurrency and ordering
- Shared mutable state, check-then-act races, lost updates, double submits.
- Ordering assumptions between events, deploys, or services.
- Caches: staleness, invalidation, stampede.

## 6. Contracts and compatibility
- Public API, function signatures, event/message shapes, config keys, file formats: changed without updating all consumers?
- Are callers found by search actually updated? What about callers outside the repo (clients, other services, docs)?
- Default values or semantics that silently changed.

## 7. Gating, atomicity, and reversibility
- Is the risky part behind a feature gate, and does the gate default to off/safe?
- Is the change atomic and cleanly revertible, or does it mix a risky refactor, a behavior change, and a migration in one commit?
- Is the isolated piece truly isolated, or does it already touch the shared/trunk path? Integration into shared paths should be deliberate and separate.
- Does flag-off behavior match the old behavior exactly?

## 8. Coupling and abstraction boundaries
Flag these when they raise future blast radius, not as taste.
- Does a module now depend on internals it shouldn't, or does dependency direction point the wrong way (core depending on a leaf)?
- Does one unit now have several unrelated reasons to change, so future edits risk collateral damage?
- Is a new abstraction leaking (callers must know implementation details) or premature (one use, extra indirection)?
- Duplicated logic that must stay in sync, with no single source of truth.
- Hidden global state, implicit ordering requirements, or side effects in code that looks pure.

## 9. Observability
- If this fails in production, will anyone know? Logs, metrics, alerts on the new path.
- Are errors distinguishable, and is there enough context to debug without redeploying?

## 10. Proof quality
- Would the tests fail if the change were wrong? Look for assertions that can't fail, over-mocked seams, and missing failure-path cases.
- Do integration tests cover the boundary between the new code and existing shared code?
- Snapshot tests: is the snapshot meaningful, or was it regenerated blindly?
- For UI: does visual evidence show the states users will hit (empty, loading, error, long text)?
- Is anything claimed as verified that no artifact supports?

## 11. What to skip
Do not spend report space on:
- Formatting, whitespace, import order (a linter's job)
- Naming preferences that don't cause confusion or bugs
- Alternative approaches that are equally sound
- Restating what the diff obviously does
- Speculative "what if we someday..." concerns with no realistic failure path

If something in this list does hide a risk (a misleading name that will cause misuse of a dangerous function), report it at the severity of the risk, not as a nit.