using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public static event System.Action<Transform> OnPlayerSpawned;

    public GameObject playerPrefab;
    public GameObject gunPrefab;
    public GameObject shieldPrefab;

    private GameObject currentPlayer;

    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (playerPrefab == null)
            return;

        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        currentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        PlayerController controller = currentPlayer.GetComponent<PlayerController>();
        if (controller == null)
            controller = currentPlayer.AddComponent<PlayerController>();

        WeaponManager wm = currentPlayer.GetComponent<WeaponManager>();
        if (wm == null)
            wm = currentPlayer.AddComponent<WeaponManager>();

        controller.EnableControl();

        if (gunPrefab != null)
            AttachGuns(currentPlayer);

        if (shieldPrefab != null)
            AttachShield(currentPlayer);

        PlayerFinder.Reset();
        OnPlayerSpawned?.Invoke(currentPlayer.transform);
    }

    void AttachGuns(GameObject player)
    {
        Transform mountLeft = player.transform.Find("WeaponMounts/WeaponMountLeft");
        Transform mountRight = player.transform.Find("WeaponMounts/WeaponMountRight");

        if (mountLeft == null || mountRight == null)
            return;

        GameObject gunLeft = Instantiate(gunPrefab, mountLeft);
        gunLeft.transform.localPosition = Vector3.zero;
        gunLeft.transform.localRotation = Quaternion.identity;

        GameObject gunRight = Instantiate(gunPrefab, mountRight);
        gunRight.transform.localPosition = Vector3.zero;
        gunRight.transform.localRotation = Quaternion.identity;

        WeaponManager wm = player.GetComponent<WeaponManager>();
        if (wm != null)
            wm.RefreshGuns();
    }

    void AttachShield(GameObject player) 
    {
        Transform shieldMount = player.transform.Find("ShieldMount");
        if (shieldMount == null) 
            return;

        GameObject shield = Instantiate(shieldPrefab, shieldMount);
        shield.transform.localPosition = Vector3.zero;
        shield.transform.localRotation = Quaternion.identity;

        ShieldManager sm = player.GetComponent<ShieldManager>();
        if (sm == null)
            sm = player.AddComponent<ShieldManager>();

        sm.SetShield(shield.GetComponent<Shield>());
    }

    public void RespawnPlayer()
    {
        SpawnPlayer();
    }
}