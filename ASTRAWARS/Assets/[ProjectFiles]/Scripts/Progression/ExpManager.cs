using UnityEngine;

public class ExpManager
{
    public static void GiveExp(int amount, Vector3 worldPosition) {
        var progress = ProgressionManager.Instance?.PlayerProgress;
        if (progress == null) return;

        progress.AddExp(amount);

        FloatingFloatsSpawner.Instance?.Spawn($"+ {amount}", worldPosition);
    }
}
