using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using TD.Interfaces;
using UnityEngine;
using Zenject;

namespace TD.Gameplay
{
    public class Enemy : MonoBehaviour
    {
        [Inject] private IEnemyRegistry _registry;
        [Inject] private IGameState _gameState;

        private Transform[] _waypoints;
        private CancellationTokenSource _cts;

        [SerializeField] private int _maxHP = 2; 
        private int _hp;

        public void Initialize(Transform[] waypoints) { 
            _waypoints = waypoints;
            _hp = _maxHP;
            Debug.Log($"[Enemy] Initialize. waypoints null? {_waypoints == null}, length: {_waypoints?.Length}");
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log($"[Enemy] Start. waypoints null? {_waypoints == null}, length: {_waypoints?.Length}, pos: {transform.position}");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(_gameState.GameToken);
            _registry.Register(transform);
            MoveAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid MoveAsync(CancellationToken token)
        {
            try
            {
                for (int i = 0; i < _waypoints.Length; i++)
                {
                    var target = _waypoints[i].position;
                    while (Vector3.Distance(transform.position, target) > 0.05f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, target,
                            Time.deltaTime * 2f);
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                    }
                }

                _gameState.LoseLife();
                Destroy(gameObject);
            }
            catch (OperationCanceledException) { }
        }

        private void OnDestroy() { 
            _registry?.Unregister(transform);
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void TakeDamage(int damage) { 
            if (_hp <=0 ) return;

            _hp -= damage;

            if (_hp <= 0) {
                _registry.Unregister(transform);
                _cts?.Cancel();
                Destroy(gameObject);
            }
        }
     }
}