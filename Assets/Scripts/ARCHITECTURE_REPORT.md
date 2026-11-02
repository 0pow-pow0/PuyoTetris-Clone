# PuyoTetris - Architecture Analysis Report

## Overview
This is a Tetris clone built in Unity using a **component-based architecture** with a central game logic controller (`TetrisLogic`) and several specialized managers handling audio, animation, input, and UI.

---

## Class Hierarchy & Architecture

### Core Game Loop
```
GameManager (empty MonoBehaviour)
    └── TetrisLogic (Main Game Controller)
            ├── Grid System (2D arrays)
            ├── Tetramino Management (Piece spawning, movement, rotation)
            ├── Collision Detection
            ├── Line Clearing Algorithm
            ├── Input Handling (Keyboard + Touch)
            └── Event System (UnityEvents)
```

### Manager Classes (Singleton Pattern)
All managers use the **Singleton pattern** with static `Get()` accessors:

| Manager | Responsibility |
|---------|----------------|
| `TetrisLogic` | Core gameplay, grid, pieces, collision, line clear |
| `UtilityShit` | Utility functions (logging, hierarchy search) |
| `FileManager` | Resource loading (tetramino prefabs) |
| `TouchManager` | Touch input processing (swipes, long press) |
| `PowUtilityU` | Coroutine helpers (delayed actions, timed loops) |
| `FieldAudioManager` | SFX/Music playback (DOTween for fade) |
| `FieldAnimationManager` | Line clear block animations (DOTween) |

### UI Controllers
| UI Script | Responsibility |
|-----------|----------------|
| `UIInGame` | Next piece preview, scoreboard |
| `UIGameOver` | Game over panel, retry/exit |
| `UIMainMenu` | Main menu navigation |
| `UIPausePanel` | Pause menu, volume sliders |
| `UIControls` | Auto-hiding controls tutorial |

### Data Classes
- `Tetramino` - Piece data (blocks list, rotation pivot, grid position, type)
- `GridMapValues` - Enum for grid cell states (EMPTY, WALL, TETRAMINO, INVALID)
- `TetraminoTypes` - Enum for 7 tetromino types (I, J, L, O, S, T, Z mirrored)
- `DetectedTouchInput` - Enum for touch gesture types

---

## Key Algorithms

### 1. **Grid Coordinate System** (TetrisLogic:78-237)
- **Bottom-left origin** for grid, **top-left origin** for world Y
- Bidirectional conversion: `GridSquareBottomLeftToWorldCoords` / `WorldCoordsToGridX/Y`
- Handles grid offset from world origin

### 2. **7-Bag Randomizer (Tetris Pouch System)** (TetrisLogic:694-751)
```
Algorithm:
1. Shuffle all 7 tetromino types → creates "bag"
2. Add 8th piece (random from bag) as preview
3. When index reaches 7:
   - Keep 8th piece as first of new bag
   - Generate new shuffled bag of 7
   - Append new 8th preview piece
```
**Purpose**: Fair distribution, prevents droughts/floods, enables next-piece preview

### 3. **Collision Detection** (TetrisLogic:856-950)
- **Predictive**: Checks *proposed* position before applying movement
- **Per-block**: Iterates all 4 blocks of tetramino
- **Axis-separated**: X and Y checked independently in `MoveActivePiece`
- **Boundary checks**: Grid bounds = collision
- **Occupancy checks**: `gridMap[y,x] == TETRAMINO` = collision
- Returns tuple: `(generalCollision, onTetraminoCollision)`

### 4. **Line Clear Algorithm** (TetrisLogic:294-351, 360-449)
```
Clear Phase:
1. CheckGridLineFill(y) - O(width) scan per affected row
2. DeleteLineFromCollisionMap(y) - Mark cells EMPTY, collect block refs
3. Fire onLineClear event (animation plays)
4. After animation delay:
   - DeleteLinesFromReferenceMap - Destroy block GameObjects
   - TranslateDownMultiple(sortedIndices) - Gravity drop
```

**TranslateDown** (TetrisLogic:294-340):
- For each column, from cleared line upward:
  - Swap gridMap values (row ↔ row-1)
  - Move GameObject to new world position
  - Swap blocksGridMapRef references
- **Optimization**: Only checks rows that received new blocks

### 5. **Piece Placement Conditions** (TetrisLogic:1050-1218)
- Checked every fall tick (`VerifyAndExecuteTetraminoPlaceConditions`)
- Conditions (OR logic):
  1. Any block at `gridHeightInSquares - 2` (bottom row)
  2. Any block has `TETRAMINO` directly below it
- On placement: updates both `gridMap` and `blocksGridMapRef`
- Triggers line check only for rows that received blocks (deduplicated)

### 6. **Rotation with Wall Kick (Basic)** (TetrisLogic:591-647)
- Rotates `rotationPivot` ±90° locally
- Checks collision at **same grid position**
- If collision → reverts rotation
- **No SRS wall kicks** - simple reject

### 7. **Input Handling** (TetrisLogic:529-589)
- **DAS-like**: Holding key adds tiny timer increment (0.001f) to bypass `INPUT_DELAY`
- **Separate axes**: X movement on delay timer, Y (gravity) on separate fall timer
- **Soft drop**: S key / long touch → faster fall speed
- **Touch gestures**: Swipe detection with tolerance threshold

### 8. **Gravity / Fall Timer** (TetrisLogic:1404-1417)
```
tetraminoFallTimer += Time.deltaTime
if (tetraminoFallTimer >= activeTetraminoFallSpeed):
    if (!TryPlacePiece()):
        MoveActivePiece(0, 1)  // Move down 1 cell
    tetraminoFallTimer = 0
```

---

## Programming Concepts & Patterns

### Design Patterns
| Pattern | Usage |
|---------|-------|
| **Singleton** | All managers (`UtilityShit`, `FileManager`, `TouchManager`, `PowUtilityU`) |
| **Observer/Event** | `UnityEvent` for decoupled gameplay→UI/audio/animation communication |
| **Component Composition** | `Tetramino` holds list of block GameObjects + pivot |
| **State Machine (Implicit)** | `hasGameOver` flag, `activePiece` null check for game states |

### Data Structures
- **2D Arrays**: `GridMapValues[,] gridMap`, `GameObject[,] blocksGridMapRef` - O(1) cell access
- **List<T>**: Piece pouch, blocks list, line clear indices
- **Tuples**: `(GameObject, int, int)` for line clear block references
- **Dictionary**: `FileManager.tetraminos` for prefab lookup by type

### Unity-Specific Concepts
- **Coroutine-based async**: `PowUtilityU.DelayAction`, `DoActionUntil`, `RepeatActionForFrame`
- **DOTween**: All animations (fade, scale, move, sequences)
- **New Input System**: `Keyboard.current`, `EnhancedTouchSupport`
- **ScriptableObject-like**: Prefabs loaded from Resources at runtime
- **SerializeField + Properties**: Inspector exposure with encapsulation

### Error Handling / Defensive Programming
- Bounds checking on all grid accesses (returns `INVALID`/`null`)
- Null checks before destruction
- Debug logging with color-coded `UtilityShit.Log`
- Early returns on invalid states

### Performance Considerations
- **Line check deduplication**: Only checks unique rows affected by placed piece
- **Sorted line clear indices**: Top-down to avoid double-drop bugs
- **Object pooling**: Not implemented (Instantiate/Destroy each piece)
- **GridMap as source of truth**: Avoids repeated GetComponent searches

---

## Architecture Strengths

1. **Separation of Concerns**: Game logic separate from audio/animation/UI
2. **Event-Driven**: Gameplay fires events; consumers react (audio, animation, UI)
3. **Grid as Authority**: Single source of truth for occupied cells
4. **Predictive Collision**: Prevents invalid states rather than correcting
5. **Pouch System**: Proper Tetris randomization per guideline
6. **Touch + Keyboard**: Unified input abstraction via `TouchManager`

---

## Areas for Improvement

| Issue | Location | Suggestion |
|-------|----------|------------|
| No wall kicks (SRS) | `RotateLeft/Right` | Implement Super Rotation System |
| No object pooling | `CreateNewPiece`, `DeleteLineFromReferenceMap` | Pool tetraminos/blocks |
| Hardcoded tetramino list | `tetraminosTypes` in inspector | Data-driven via ScriptableObject |
| `UtilityShit` naming | Class name | Rename to `GameUtilities` |
| `GameManager` empty | `GameManager.cs` | Remove or use as bootstrapper |
| No hold piece | Missing feature | Add hold queue |
| No ghost piece | Missing feature | Add preview of landing position |
| Line clear scoring | `UIInGame.UpdateScoreboard` | Standard Tetris scoring (1/3/5/8 lines) |

---

## File Dependency Graph

```
TetrisLogic (Central Hub)
    ├── References: tetraminosPiecesWrapper, gridOrigin, tetraminoSpawnPoint
    ├── Events → UIInGame, FieldAudioManager, FieldAnimationManager, UIGameOver
    ├── Uses: UtilityShit.Log, PowUtilityU.DelayAction, TouchManager.thisFrameTouchInput
    └── Instantiates: Tetramino prefabs (from tetraminosTypes list)

Tetramino
    └── Data container (blocks[], rotationPivot, gridPosX/Y, type)

UIInGame ← TetrisLogic.onPieceCreated, onLineClear
FieldAudioManager ← TetrisLogic.onPieceMove, onPieceRotate, onPieceJustPlaced, onLineClear
FieldAnimationManager ← TetrisLogic.onLineClear
UIGameOver ← TetrisLogic.onMatchGameOver
UIPausePanel ← Time.timeScale (global)
TouchManager → TetrisLogic.RetrieveInput()
FileManager → Resources.Load (prefabs)
PowUtilityU → Coroutine helpers for TetrisLogic
```