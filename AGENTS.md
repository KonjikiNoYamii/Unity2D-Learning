# FirstProject — Agent Guidance

Unity 6 (6000.3.7f1) 2D action-platformer built from the URP 2D template. Single scene (`GameScene.unity`), no scene management yet.

## Architecture

- **No namespaces** — all classes in global namespace. Follow this convention.
- **No asmdefs** — single `Assembly-CSharp` default assembly.
- **No tests** — `com.unity.test-framework` is installed but no test files exist.
- **No `.editorconfig`** — no code style enforcement.

## Critical gotchas

- **Input System package is installed but UNUSED.** All scripts use legacy `Input.GetAxis`, `Input.GetMouseButtonDown`, `Input.GetKeyDown`. Do NOT migrate to InputSystem without explicit request.
- **Unity 6 API:** use `rb.linearVelocity` (not `rb.velocity`).
- **Animation events drive combat:** `PlayerAttack.EnableHit()`, `DisableHit()`, `EndAttack()` are called from animation clips, not from code.
- **`EnemyAI` finds player via `GameObject.FindGameObjectWithTag("Player")` in `Start()`** — fragile, won't work if player is instantiated later.
- **EditorBuildSettings still references `SampleScene.unity`** (stale template default). Actual scene is `GameScene.unity`. Fix `EditorBuildSettings.asset` if adding scenes to build.
- **Player is NOT a prefab** — constructed directly in the scene.
- **`InventorySlot` class exists but is completely unused** — planned future feature.
- **Debug messages are in Indonesian** — keep consistent with existing code.
- **`HitStopManager` is a singleton** — `HitStopManager.Instance`.

## Code conventions

- PascalCase public fields exposed directly (no `[SerializeField] private` pattern).
- `[Header]` attributes used for Inspector organization.
- `OnDrawGizmosSelected` used for hitbox visualization.
- Coroutines for timed sequences (hurt stun, hitstop, respawn delay).
- `ScriptableObject`-based data: `AttackData` (`Combat/Attack Data`), `ItemData` (`Items/Item Data`).

## Project state

- `.gitignore` ignores `*.csproj` and `*.slnx` but these ARE committed. Do not re-add them if removed.
- No CI, no pre-commit hooks, no build scripts.
- Respawn hardcoded to `Vector2.zero` (no checkpoint system).
- No persistent `DontDestroyOnLoad` managers.
