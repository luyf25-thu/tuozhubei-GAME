# 2D World-Switching Platformer – Design Document

## 1. Game Overview

- **Genre**: 2D Side-Scrolling Platformer / Puzzle
- **Player Count**: Single-player
- **Camera**: Fixed 2D side view
- **Core Mechanic**: Instant world switching during movement

This game is a 2D side-scrolling platformer focused on **rule-based spatial reasoning** rather than pure reaction speed or mechanical precision.

The player continuously moves and jumps through levels while freely switching between two parallel worlds with different rules, using world switching to construct valid paths, avoid hazards, and solve spatial puzzles.

---

## 2. Core Design Philosophy

> **Replace traditional platformer reaction pressure with spatial rule understanding and world-switching decisions.**

- The game does NOT emphasize:
  - Frame-perfect input
  - High-speed reaction tests
  - Punitive failure loops
- The game DOES emphasize:
  - Understanding spatial rules
  - Reasoning about world differences
  - Making correct switching decisions during motion

World switching is a thinking tool, not an emergency button.

---

## 3. World System Overview

### 3.1 World Count

- The game contains **two parallel worlds**:
  - World A
  - World B
- Only **one player entity** exists at any time.
- The player is never duplicated across worlds.

### 3.2 Spatial Mapping

- The two worlds share the **same coordinate space**.
- A given `(x, y)` position in World A maps directly to the same `(x, y)` position in World B.
- Level geometry may differ between worlds, but spatial alignment is preserved.

---

## 4. World Switching System

### 4.1 Switching Rules

The player may switch between World A and World B at any time, subject to the following constraints:

- **Zero cost**
  - No resource consumption
  - No cooldown
- **Zero latency**
  - Switching is instantaneous
- **Non-interruptive**
  - Switching does not cancel or reset player actions

World switching is treated as a state change, not an animation.

### 4.2 State Continuity

When switching worlds, the following player states MUST remain continuous:

- Position
- Velocity
- Acceleration
- Movement state (running, jumping, falling, etc.)

No state reset or reinitialization occurs during switching.

---

## 5. Movement & Gameplay Loop

- The player is expected to:
  - Move continuously
  - Jump across platforms
  - Make world-switching decisions mid-action
  - Use wall climbing and wall jumping to traverse vertical routes
- Levels are designed such that:
  - Certain paths are impossible in a single world
  - Valid solutions emerge only through correct world switching

The game loop assumes that world switching is used frequently and safely.

### 5.1 Wall Climbing & Wall Jumping

- **Wall Cling**: When the player presses toward a near-vertical wall in midair, they can cling and climb.
- **Limited Climb**: Climb speed decays with climbed distance until it reaches zero; then the player begins sliding down.
- **Wall Jump**: Jumping while clinging launches away from the wall. Jump height decreases as climb distance increases and can reach zero.
- **Input Priority**: While clinging, a jump input triggers wall jump immediately even if the player is still holding the climb direction.
- **Same-Wall Lockout**: After leaving a wall (jumping off, losing contact, or letting go), that same wall is locked out.
  - Lock clears by touching a different wall first, or by landing, or after a short timer.
  - This prevents infinite re-climb loops on a single wall.

---

## 6. Gameplay Purpose of World Switching

World switching enables the player to:

- **Construct valid traversal paths**
  - Platforms or geometry may exist in only one world
- **Avoid hazards**
  - Dangerous elements may differ between worlds
- **Solve spatial puzzles**
  - Puzzle logic is based on rule differences between worlds

Switching is a proactive mechanic, not a fail-safe.

---

## 7. World Rules (Placeholder)

> ⚠️ The specific gameplay rules of World A and World B are intentionally left undefined at this stage.

This section is reserved for future expansion, including but not limited to:

- Collision rules
- Gravity behavior
- Physics modifiers
- Hazard behavior
- Environmental interactions

Any world-specific behavior MUST respect the following invariant:

- World switching preserves player motion continuity.

---

## 8. Design Constraints (Hard Rules)

The following constraints MUST be respected in implementation:

1. World switching must be frame-safe and reversible.
2. Player control scheme must remain identical across worlds.
3. No world may be strictly “better” than the other.
4. Single-world traversal should often be insufficient.
5. World switching must not introduce artificial delays or penalties.

---

## 9. Open Extension Points

The following systems are intentionally left open for future design:

- World-specific physics rules
- Visual differentiation between worlds
- Audio feedback for switching
- Level scripting logic
- Tutorial and onboarding logic

These systems should be designed to integrate cleanly with the world-switching core.

---

## 10. Summary

This game is built around a single core mechanic:

**Instant, cost-free switching between two parallel worlds with different rules, used to solve spatial problems during continuous movement.**

All systems should reinforce this mechanic.
