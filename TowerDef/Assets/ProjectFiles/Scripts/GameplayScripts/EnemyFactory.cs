using System.Threading;
using Cysharp.Threading.Tasks;
using TD.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TD.Gameplay {
    public class EnemyFactory : IEnemyFactory {
        [Inject] private AssetReferenceGameObject _prefabRef;
        [Inject] private DiContainer _container;

        private readonly Transform _parent;
        private readonly Transform[] _waypoints;
        private readonly IGameState _gameState;

        public EnemyFactory(
            [Inject(Id = "EnemyParent")] Transform parent,
            [Inject(Id = "Waypoints")] Transform[] waypoints,
            IGameState gameState) {
            _parent = parent;
            _waypoints = waypoints;
            _gameState = gameState;
        }

        public GameObject Create(Vector3 position) {
            Spawn(position, _gameState.GameToken).Forget();
            return null;
        }

        private async UniTaskVoid Spawn(Vector3 position, CancellationToken token) {
            try {
                var handle = _prefabRef.InstantiateAsync(position, Quaternion.identity, _parent);
                var go = await handle.ToUniTask(cancellationToken: token);

                _container.InjectGameObject(go);

                var enemy = go.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.Initialize(_waypoints);
            }
            catch (System.OperationCanceledException) { }
        }
    }
}