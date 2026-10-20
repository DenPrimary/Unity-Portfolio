using Cysharp.Threading.Tasks;
using System;
using TD.Interfaces;
using UnityEngine;
using Zenject;

namespace TD.Gameplay {
    public class WaveManager : IInitializable, IDisposable {
        private readonly IEnemyFactory _factory;
        private readonly IGameState _gameState;
        private readonly Transform _spawnPoint;
        private readonly Transform _enemyParent;

        private const int TotalWaves = 3;
        private const int EnemiesPerWave = 4;

        public WaveManager(
            IEnemyFactory factory,
            IGameState gameState,
            [Inject(Id = "SpawnPoint")] Transform spawnPoint,
            [Inject(Id = "EnemyParent")] Transform enemyParent) {
            _factory = factory;
            _gameState = gameState;
            _spawnPoint = spawnPoint;
            _enemyParent = enemyParent;
        }

        public void Initialize() {
            RunWavesAsync(_gameState.GameToken).Forget();
        }

        private async UniTaskVoid RunWavesAsync(System.Threading.CancellationToken token) {
            try {
                for (int wave = 1; wave <= TotalWaves; wave++) {
                    Debug.Log($"Wave {wave} started");

                    for (int i = 0; i < EnemiesPerWave; i++) {
                        _factory.Create(_spawnPoint.position);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.7f), cancellationToken: token);
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);
                }

                Debug.Log("All waves spawned");
            }
            catch (OperationCanceledException) {
                Debug.Log("WaveManager cancelled");
            }
        }

        public void Dispose() { }
    }
}
