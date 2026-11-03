# ImmersiveAPI 3 integration

| | |
|---|---|
| Training asset | `Assets/Resources/ImmersiveAPI/Trainings/ICOPE_Audition.asset` (remote training 162) |
| Runtime code | `Assets/Scripts/Immersive` (assembly `ICOPE.Immersive`) |
| Generator | `Assets/Scripts/Immersive/Editor` (assembly `ICOPE.Immersive.Editor`) |
| Package changes | [PackageChanges.md](PackageChanges.md) |

## Mapping

| xAPI | Game | Id |
|---|---|---|
| Strategy | Patient (folder in `StreamingAssets/Levels`) | `1_3_denise` |
| Step | `AlgoStep` at index *i* | `1_3_denise.s03_otoscopy` |
| Interaction (answered question) | Diagnostic / action phase | `1_3_denise.s03_otoscopy.diagnostic` |
| Performance (Strategy scope) | Errors, steps right first time, success rate, duration | `1_3_denise.success_rate` |
| Performance (Strategy scope) | Total time on the patient and times handled, over every attempt (finished or left), synced with the server | `1_3_denise.time_spent`, `1_3_denise.times_handled` |
| Performance (Simulation scope) | Coin balance, synced with the server | `coins` |
| Performance (Simulation scope) | Total patients handled (all attempts), synced with the server | `patients_handled` |
| Performance (Simulation scope) | Reminder ("Rappel") button presses, synced with the server | `reminder_openings` |
| Performance (Simulation scope) | Global success rate: average best score of every completed patient | `global_success_rate` |
| Skill | `SkillCatalog` (6 skills by step type) | `skill.otoscopy` |
| Reward | Average best score of every patient (unplayed = 0), 0-1000 | `reward.score` |
| Badge | Every case of a level completed (Facile, Moyen, Difficile) | slots 1-3 (`LevelBadgeCatalog`) |

Ids are built in one place: `FlowIds`. Strategies are chained in manifest order, and steps in
case order. Step score is binary (1 = every phase right first time) with a step passing score of 1;
strategies and the Simulation have a passing score of 0.

TEMPORARY-STORED-PROGRESS: every finished step sends `passed` (with its real score), never
`failed`, because the backend marks a step succeeded, and restores it, only on `passed`.

## Regenerating the Training

Run **ICOPE > ImmersiveAPI > Generate Training From Levels** after any change to the patient
files (added patient, added/reordered step, phase added or removed). It:

1. reads `StreamingAssets/Levels/manifest.json` and every patient JSON,
2. rebuilds Strategies/Steps and installs them into the Training,
3. authors Interactions, Performances, Skills and the Reward,
4. creates the value assets read at send time under `Assets/Resources/ImmersiveAPI/Values`,
5. writes `Docs/ImmersiveAPI/ICOPE_Audition.gameflow.json`.

**Upload that JSON to training 162 on the backend.** Stored progress is matched by these ids, so a
regenerated flow that isn't uploaded restores nothing.

The generator also rewrites the Training's Editor graph
(`Assets/Editor/ImmersiveAPI/Trainings/GameFlow_ICOPE_Audition.asset`): one Strategy node per
patient and one Step node per AlgoStep, carrying the generated ids. Edits made in the graph keep
the generated Interactions/Performances, but the next generation replaces the whole graph. Close
the GameFlow graph window before generating.

## Runtime

- `GameManager` creates the services (`ImmersiveServicesFactory`) once `TrainingRuntime` has
  restored the stored progress. Play stays disabled until then.
- `StepManager` / `GameManager` call `IClinicalCaseTracker` at case start, step start, each answer,
  step end and when leaving a case. `ImmersiveCaseTracker` turns those into statements, and
  Strategy and Simulation outcomes follow automatically from the last `CompleteStep`.
- `LevelSelector` reads unlocks from `IPatientProgressProvider` (server progress), replacing the
  former `PlayerPrefs` "MaxLevel".
- Coins are the `coins` Performance (Sync, Apply and Send on close on). The package writes the
  stored balance into `CoinLedger.balance` at startup and sends it before `terminated`;
  `SyncedCoinWallet` also scores it after every earn (`round(successRate / 4)` per case) and every
  shop purchase. Purchased items stay local (`PlayerPrefs` `items`). With a debug option on, or
  without a session, the balance is a session-only `CoinWallet`.
- The `reward.score` Reward is sent after each case with `AverageScoreReward`: the average of every
  patient's best strategy score (itself the average of its step scores), × 1000. The dashboard
  shows the last score of each reward id, so it always shows the current average.
- Badges are sent by the package as soon as their condition holds: `LevelCompletionBoard` sets
  `ProgressBoard.levelsCompleted` from the completed patients, at startup and after each case.
- The value assets in `Resources/ImmersiveAPI/Values` are reset when play starts
  (`TrackingValuesReset`), so values left over from an earlier Editor play session are never sent.
- Debug options `alwaysRight` and `canAccessAllLevel` disable statement sending (a warning is
  logged). `MainScene` currently has `alwaysRight` on.
- In the Editor without a debug session (WiDiD_API > ImmersiveAPI > Debug Session), every patient
  is open and nothing is sent. A build only runs when launched from WiDiD Immersive.
