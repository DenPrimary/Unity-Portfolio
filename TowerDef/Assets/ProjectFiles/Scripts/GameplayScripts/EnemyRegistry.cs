using System.Collections.Generic;
using TD.Interfaces;
using UnityEngine;

namespace TD.Gameplay
{
    public class EnemyRegistry : IEnemyRegistry
    {
        private readonly List<Transform> _enemies = new();
        public IReadOnlyList<Transform> ActiveEnemies => _enemies;
        public void Register(Transform enemy) => _enemies.Add(enemy);
        public void Unregister(Transform enemy) => _enemies.Remove(enemy);
    }
}
