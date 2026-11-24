# Unity Portfolio — Denis Zabiiakin

C# programming. 3+ years of teaching Unity and Roblox to kids and teenagers.  
Portfolio for showcases and Unity projects.

## Projects

PenaltyDuel

- 3D football penalty game. Choose a target zone, control shot power and timing, score goals and outsmart an adaptive goalkeeper.
- Adaptive goalkeeper AI — analyzes the last 5 shots using a ring buffer and recency-weighted probabilities; adapts its decisions to the player's shooting patterns
- Weighted probability model — recency weights (1.0, 0.7, 0.5, 0.35, 0.25), Perfect Zone priority bonus (×1.5) and Laplace smoothing (α=1) for goalkeeper decision-making
- Shot mechanics — 9 target zones, charge-based shot power and timing; player Accuracy and Composure attributes (50–100) influence shot execution
- Scoring and progression — Perfect Zone goals award 300 points, regular goals 100; 5-shot sessions with 1–3 star rating based on final score
- Perfect Zone system — randomly selects a target zone for each session, rewarding players for accurate shots
- Server-authoritative gameplay — Unity Netcode RPC-based shot submission and server-side game logic
- Physics-driven ball movement — shot execution and goal detection integrated with the ball's movement
- Built with Unity 6, Netcode for GameObjects
- **[View project →](./PenaltyDuel)**

### [2D Pong](./Pong)
Classic Pong game built from scratch in Unity.
- **Event-driven scoring** (static ScoreManager events: OnScoreChanged, OnPlayerScoredEvent, OnEnemyScoredEvent) — UI subscribes without referencing game logic; updates only on change, no per-frame polling
- **Separation of concerns** — BallController (input/physics), BallAccelerator (speed state), AIController (movement), GameManager (flow), GameTimeManager (pause) — each owns one responsibility
- **Ball acceleration** (BallAccelerator) — speed state separated from Rigidbody2D; velocity normalized after every boost (paddle +0.75, wall +0.25) with hard cap, prevents trajectory drift
- **Anti-stuck physics guard** — minimum |velocity.x| enforced each FixedUpdate, prevents near-vertical ball loops without breaking legitimate angles
- **Coroutine sequencing with re-entry guard** — goal → popup → paddle Lerp reset → pause → ball reset, all gated by isProcessingGoal to prevent overlapping routines
- **Dual-time pause model** (GameTimeManager) — Time.timeScale freezes physics, realtimeSinceStartup drives pause duration, animations run on unscaledDeltaTime
- **"Draw" mechanic** (DrawZoneController) — Vector2.Angle detects horizontal state, counts consecutive paddle hits, declares draw at threshold
- **Self-playing demo mode** (DemoGameManager) — AI vs AI match in the main menu reusing the same BallController; goal routing branches by manager
- Built with Unity 2022.3 LTS (2D)
- **[View project →](./Pong)**

### [ASTRAWARS](./ASTRAWARS)
3D top-down shooter. Upgrade your ship, survive as long as you can.
- **Object pooling** (ObjectPool<T>) for bullets and asteroids — eliminates GC allocations and micro-stutters in endless mode
- **Polymorphic asteroid hierarchy** (Asteroid base class) — replaces type checks with virtual methods, so new asteroid types don't require changing existing code
- **Progression system** (XP, levels, upgrades) — split into PlayerProgress (data), ExpCalc (formulas), LvlUpManager (logic) for single responsibility
- **Singleton managers** (ProgressionManager, BulletPool, HighlightManager) — single point of access for global systems
- **System.Action events** for UI — updates only when stats change, no per-frame polling
- **LINQ** in AsteroidPath — declarative filtering and sorting of asteroids by distance to player
- **DOTween** animations with DOKill on pool return — prevents tween accumulation and conflicts
- Built with Unity 6000.5.2f1 (URP)
- **[View project →](./ASTRAWARS)**

### [TowerDef (Showcase)](./TowerDef (Showcase))
- Single CancellationToken from IGameState — enemies, towers and WaveManager link via CreateLinkedTokenSource; game over cancels all async loops at once
- Interfaces (IEnemyRegistry, IEnemyFactory, IGameState) — systems never reference each other concretely, testable in isolation
- Zenject DI — constructor injection for plain classes, field injection for MonoBehaviour; no global state
- Addressables (InstantiateAsync + InjectGameObject) — async spawning: instantiate, inject, initialize; assets load on demand
- UniTask over coroutines — cancellable, allocation-free; async loops live exactly as long as the object
- SOLID in practice — EnemyRegistry stores, EnemyFactory creates, GameState owns lives and token, WaveManager spawns waves
- LINQ-free hot path — manual foreach with one Vector3.Distance per enemy in the firing loop, zero allocations
- Built with Unity 6000.5.2f1, URP
- **[View project →](./TowerDef (Showcase))**

### Udemy Course Project 3D 1st person Shooter (recovery in progress)
Restoring projects from "Complete C# Unity Game Developer 3D" (GameDev.tv / Udemy, winter 2023).

---

## Contact
- Telegram: [@DANPRIMARY](https://t.me/DANPRIMARY)
- Email: dansergeevichprimary@gmail.com
