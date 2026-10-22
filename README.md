# Unity Portfolio — Denis Zabiiakin

C# programming. 3+ years of teaching Unity and Roblox to kids and teenagers.  
Portfolio for showcases and Unity projects.

## Projects

### [2D Pong](./Pong)
Classic Pong game built from scratch in Unity.
- Ball physics
- AI opponent
- Score system
- UI menu
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
