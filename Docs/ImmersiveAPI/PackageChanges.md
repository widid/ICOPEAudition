# com.widid.immersiveapi3 — changes required by ICOPE (based on 0.2.6)

Changes the ICOPE integration (`Assets/Scripts/Immersive`) depends on. They are **not** in this
project: apply them in `Packages_Project`, re-export the tgz into `LocalPackages/`, and point
`Packages/manifest.json` at the new version. Until then `ICOPE.Immersive` does not compile.

The exact diff is in [`immersiveapi3-local-changes.patch`](immersiveapi3-local-changes.patch),
made against the 0.2.6 tgz (paths relative to the package root: `patch -p1` or `git apply -p1`
from the package folder).

APIs ICOPE calls that 0.2.6 lacks: `TrainingRuntime.IsInitializing`,
`TrainingRuntime.GetStepBestScore`, `TrainingRuntime.GetStrategyBestScore`,
`TrainingRewardData.RestoredTotal`.

All changes are additive and backward compatible: no public member was removed or renamed, and
existing assets need no migration.

## 1. Best score per Step / Strategy

**Why:** the host game needs the best score restored from the server (`jsonStep` highscore) to show
progress and compute skills. `StepProgressController`/`StrategyProgressController` had `GetScore`,
but `TrainingRuntime` exposed neither, and `Score` is overwritten by the latest attempt.

| File | Change |
|---|---|
| `Runtime/Trainings/GameFlow/TrainingNodeProgress.cs` | New `BestScore` (private set) and `RecordScore(float)`, which sets `Score` and raises `BestScore` when beaten. |
| `Runtime/Trainings/GameFlow/Step/StepProgressController.cs` | `RecordScore` and `RestoreProgress` go through `TrainingNodeProgress.RecordScore`. New `GetBestScore(stepId)`. |
| `Runtime/Trainings/GameFlow/Strategy/StrategyProgressController.cs` | Strategy completion and `RestoreProgress` go through `RecordScore`. New `GetBestScore(strategyId)`. |
| `Runtime/Trainings/GameFlow/TrainingRuntime.cs` | New `GetStepBestScore(stepId)` and `GetStrategyBestScore(strategyId)`. |

`Score` semantics are unchanged (latest attempt, restored highscore at start), so
`RequireAllPreviousPassed` behaves as before.

## 2. Replaying a completed Strategy

**Why:** `AutomateStrategy` returned early once a strategy was `Completed`. A strategy completed
earlier in the session, or restored as completed from the server, never sent its
Progressed / Passed-or-Failed / Completed again when replayed, and its score could not improve.

`StrategyProgressController` now tracks an **attempt** per strategy:

- `Launch` opens an attempt (the set of steps completed since that launch).
- While an attempt is open, every step completion is automated even if the strategy is already
  completed. Progressed is `attempt steps / total steps`, and the completion score is the average of
  the steps completed **in this attempt**.
- The attempt closes when the strategy ends.
- With no open attempt (steps completed without `LaunchStrategy`), the previous behavior applies
  unchanged, including the early return once completed.

`Completed` is never reset, so unlocks of downstream strategies are not affected by a replay.
`SimulationProgressController` is untouched: the Simulation still completes once per session.

## 3. `TrainingRuntime.IsInitializing`

**Why:** in the Editor, `EditorSessionBootstrap` returns without starting a session when no debug
session exists, so `WhenReady` never completes. Host code could not tell "still initializing" from
"no session at all" without awaiting forever.

| File | Change |
|---|---|
| `Runtime/Trainings/GameFlow/TrainingRuntime.cs` | New `public static bool IsInitializing` (reads the existing private `initializing` flag). |

## 4. Reward restore target (TEMPORARY-STORED-PROGRESS)

**Why:** `RewardRecorder.RestoreProgress` wrote the training-wide `totalPoint` into the reward's
**Raw** reflected source. When Raw means "points awarded by this statement" (summed by the backend),
the restored total landed in the wrong field and would be re-sent as one huge award.

| File | Change |
|---|---|
| `Runtime/Trainings/GameFlow/Reward/TrainingRewardData.cs` | New serialized `RestoredTotal` (`PerformanceIntValueSource`) and `RestoreTarget` (`RestoredTotal` when enabled with a reflected source, else `Raw`). |
| `Runtime/Trainings/GameFlow/Reward/RewardRecorder.cs` | `RestoreProgress` writes into `reward.RestoreTarget`. |

Not done, to do upstream if the change is kept:

- `RewardEditorPanel` does not show `RestoredTotal` yet. ICOPE sets it from code (generator).
- Add `RestoredTotal` to the `TEMPORARY-STORED-PROGRESS` list in `Documentation~/StoredProgressBridge.md`.

## Upstream checklist

1. Apply the patch in `Packages_Project`.
2. Expose `RestoredTotal` in the Reward editor panel.
3. Document items 1–3 in `Runtime/Trainings/GameFlow/README.md` / XML docs as needed.
4. Bump the version (e.g. `0.2.7`), export the tgz to ICOPE `LocalPackages/`, and update the
   `file:` entry in `Packages/manifest.json`.
