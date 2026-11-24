using Unity.Netcode;
using UnityEngine;

public class GoalTrigger : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!other.CompareTag("Ball")) return;

        if (PenaltyGameManager.Instance != null)
            PenaltyGameManager.Instance.OnBallEnteredGoal();
    }
}
