using UnityEngine;

namespace TD.Interfaces {
    public interface IEnemyFactory {
        GameObject Create(Vector3 position);
    }
}
