using System.Collections.Generic;
using UnityEngine;

namespace TD.Interfaces { 
    public interface IEnemyRegistry {
        IReadOnlyList<Transform> ActiveEnemies { get; }
        void Register(Transform enemy);
        void Unregister(Transform enemy);
    }
}