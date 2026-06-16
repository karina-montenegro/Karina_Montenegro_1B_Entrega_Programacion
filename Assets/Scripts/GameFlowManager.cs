using Unity.Netcode;
using UnityEngine;
public class GameFlowManager : NetworkBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    public NetworkVariable<bool> GameStarted = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        Instance = this;
        Debug.Log("[GameFlow] Awake, Instance seteado.");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"[GameFlow] OnNetworkSpawn IsServer={IsServer} IsClient={IsClient}");

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            CheckPlayerCount();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log($"[GameFlow] Cliente conectado: {clientId}");
        CheckPlayerCount();
    }

    private void CheckPlayerCount()
    {
        if (!IsServer) return;
        if (GameStarted.Value) return;

        int count = NetworkManager.Singleton.ConnectedClientsList.Count;
        Debug.Log($"[GameFlow] Chequeando cantidad de jugadores: {count}");

        if (count >= 2)
        {
            GameStarted.Value = true;
            Debug.Log("[GameFlow] GameStarted = true");
        }
    }
}
