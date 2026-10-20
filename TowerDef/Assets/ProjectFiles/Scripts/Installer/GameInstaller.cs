using TD.Gameplay;
using TD.Interfaces;
using UnityEngine;
using Zenject;
using UnityEngine.AddressableAssets;

namespace TD.Installer {
    public class GameInstaller : MonoInstaller {
        [SerializeField] private AssetReferenceGameObject _enemyPrefabRef;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _enemyParent;
        [SerializeField] private Transform[] _waypoints;

        public override void InstallBindings() {
            Container.BindInterfacesTo<EnemyRegistry>().AsSingle();
            Container.BindInterfacesTo<GameState>().AsSingle();

            Container.Bind<AssetReferenceGameObject>()
                .FromInstance(_enemyPrefabRef)
                .AsSingle();

            Container.Bind<Transform[]>()
                .WithId("Waypoints")
                .FromInstance(_waypoints)
                .AsSingle();

            Container.Bind<IEnemyFactory>()
                .To<EnemyFactory>()
                .AsSingle()
                .WithArguments(Container.Resolve<IGameState>());

            Container.Bind<Transform>().WithId("SpawnPoint").FromInstance(_spawnPoint);
            Container.Bind<Transform>().WithId("EnemyParent").FromInstance(_enemyParent);

            Container.BindInterfacesTo<WaveManager>().AsSingle();
        }
    }
}
