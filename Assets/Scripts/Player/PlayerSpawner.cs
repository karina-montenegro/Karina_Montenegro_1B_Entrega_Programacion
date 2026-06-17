using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Puntos de Spawn")]
    [Tooltip("Arrastrá aquí 2 GameObjects vacíos ubicados donde querés que aparezca cada jugador")]
    [SerializeField] private Transform[] _spawnPoints;

    // Lleva la cuenta de cuántos jugadores ya se conectaron, para asignar el spawn point correspondiente
    private int _siguienteIndiceSpawn = 0;

    private void Start()
    {
        StartCoroutine(SuscribirseCuandoExistaNetworkManager());
    }

    private IEnumerator SuscribirseCuandoExistaNetworkManager()
    {
        // Esperar hasta que el NetworkManager.Singleton esté listo
        while (NetworkManager.Singleton == null)
            yield return null;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // ejecutarse en el servidor/host
        if (!NetworkManager.Singleton.IsServer) return;

        if (_spawnPoints == null || _spawnPoints.Length == 0)
            return;

        // Busca el NetworkObject del jugador que se acaba de conectar
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
            return;

        NetworkObject playerObject = client.PlayerObject;
        if (playerObject == null)
            return;

        // Elegir el spawn point según el orden de conexión
        int indice = _siguienteIndiceSpawn % _spawnPoints.Length;
        Transform spawnPoint = _spawnPoints[indice];

        playerObject.transform.position = spawnPoint.position;
        playerObject.transform.rotation = spawnPoint.rotation;

        _siguienteIndiceSpawn++;
    }
}