using Unity.Netcode;
using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{
    public NetworkObject gameManager;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        if (!NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.StartHost();
        }
    }

    private void OnServerStarted() { }
}
