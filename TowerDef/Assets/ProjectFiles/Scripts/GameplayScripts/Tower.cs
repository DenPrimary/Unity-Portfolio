using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TD.Interfaces;
using Zenject;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace TD.Gameplay
{
    public class Tower : MonoBehaviour {
        [SerializeField] private float _range = 7f;
        [SerializeField] private float _fireInterval = 1.5f;
        [SerializeField] private LineRenderer line;

        [Inject] private IEnemyRegistry _registry;
        [Inject] private IGameState _gameState;

        private CancellationTokenSource _cts;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(_gameState.GameToken);
            FireLoopAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid FireLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested) {
                    await UniTask.Delay(TimeSpan.FromSeconds(_fireInterval), cancellationToken: token);
                    TryFire();
                }
            }

            catch (OperationCanceledException) { }
        }

        private async UniTaskVoid ShowShot(Vector3 target) {
            if (line == null) return;

            line.SetPosition(0, transform.position + Vector3.up * 2f);
            line.SetPosition(1, target);
            line.enabled = true;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            line.enabled = false;
        }

        private void TryFire() {
            Transform closest = null;
            float bestDist = _range;

            foreach (var enemy in _registry.ActiveEnemies) { 
                
                if (enemy == null) continue;

                float d = Vector3.Distance(transform.position, enemy.position);
                
                if (d < bestDist) { 
                    bestDist = d;
                    closest = enemy;
                }
            }

            if (closest != null) {
                Debug.Log($"Tower {name} fired at {closest.name}");
                var enemyComponent = closest.GetComponent<Enemy>();

                if (enemyComponent != null)
                    enemyComponent.TakeDamage(1);

                ShowShot(closest.position).Forget();
            }
        }

        private void OnDestroy() {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void OnDrawGizmosSelected() { 
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}
